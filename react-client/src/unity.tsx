import { Fragment, useEffect, useRef, useState } from 'react';
import { Unity, useUnityContext } from 'react-unity-webgl/distribution/exports';
import { AptosClient, CoinClient } from 'aptos';
import { ethers } from 'ethers';
const fcl = require('@blocto/fcl');

let aptosClient: AptosClient;
let coinClient: CoinClient;

fcl
  .config()
  .put('accessNode.api', 'https://rest-testnet.onflow.org')
  .put('discovery.wallet.method', 'POP/RPC')
  .put('challenge.handshake', 'https://flow-wallet-testnet.blocto.app/authn');

let savedUnityInstance: any;
let selectedWalletName: string;
let selectedNetwork: any;
let selectedBlockchain: string;
let connectReqId: string | null;

const NETWORK_MAPPING = {
  ETHEREUM_MAINNET: 'mainnet',
};
function MatrixUnityPlayer() {
  const {
    unityProvider,
    addEventListener,
    removeEventListener,
    UNSAFE__unityInstance,
  } = useUnityContext({
    codeUrl: `/unity-build/sample-build.wasm`,
    dataUrl: `/unity-build/sample-build.data`,
    frameworkUrl: `/unity-build/sample-build.framework.js`,
    loaderUrl: `/unity-build/sample-build.loader.js`,
    webglContextAttributes: {
      preserveDrawingBuffer: true,
    },
  });
  if (UNSAFE__unityInstance) {
    savedUnityInstance = UNSAFE__unityInstance;
  }

  const [user, setUser]: any = useState(null);
  useEffect(() => {
    if (user?.addr) {
      // flow
      savedUnityInstance.SendMessage(
        'Canvas',
        'ShowConnectionSuccess',
        JSON.stringify({
          invocationId: connectReqId,
          address: user.addr,
        })
      );
      connectReqId = null;
    }
    if (user?.address) {
      // aptos
      savedUnityInstance.SendMessage(
        'Canvas',
        'ShowConnectionSuccess',
        JSON.stringify({
          invocationId: connectReqId,
          address: user.address,
        })
      );
      connectReqId = null;
    }
  }, [user]);

  const web3Connect = async (
    invocationId: string,
    walletConnectionReq: string
  ): Promise<any> => {
    connectReqId = invocationId;
    const reqData = JSON.parse(walletConnectionReq);
    const walletName = reqData.walletName;
    selectedWalletName = walletName;
    selectedBlockchain = reqData.blockchain;
    //@ts-ignore
    selectedNetwork = NETWORK_MAPPING[reqData.network];

    if (walletName === 'METAMASK' && window.ethereum) {
      const provider = new ethers.providers.Web3Provider(window.ethereum);
      await window.ethereum.request({ method: 'eth_requestAccounts' });
      const accounts = await provider.listAccounts();
      savedUnityInstance.SendMessage(
        'Canvas',
        'ShowConnectionSuccess',
        JSON.stringify({
          invocationId,
          address: accounts[0],
        })
      );
      connectReqId = null;
    } else if (walletName === 'BLOCTO') {
      // flow code
      fcl
        .config()
        .put('accessNode.api', 'https://rest-testnet.onflow.org')
        .put('discovery.wallet.method', 'POP/RPC')
        .put(
          'challenge.handshake',
          'https://flow-wallet-testnet.blocto.app/authn'
        );
      fcl.authenticate();
      fcl.currentUser().subscribe(setUser);
    } else if (walletName === 'PETRA' && window.aptos) {
      aptosClient = new AptosClient(
        'https://fullnode.devnet.aptoslabs.com/v1/'
      );
      coinClient = new CoinClient(aptosClient);

      window.aptos
        .connect()
        .then((u: any) => {
          setUser(u);
        })
        .catch((e: any) => {
          savedUnityInstance.SendMessage(
            'Canvas',
            'ErrorListener',
            JSON.stringify({
              invocationId,
              message: 'Not able to connect to Aptos!',
            })
          );
          connectReqId = null;
        });
    } else if (walletName === 'SUI_WALLET') {
      // PENDING IMPLEMENTATION
    } else {
      console.log('No wallet');
      connectReqId = null;
    }
  };

  const fetchWalletBalance = async (
    invocationId: string,
    walletBalReq: string
  ): Promise<any> => {
    const reqData = JSON.parse(walletBalReq);
    const address = reqData.address;
    if (selectedWalletName === 'METAMASK') {
      const provider = ethers.getDefaultProvider(selectedNetwork);
      const balance = await provider.getBalance(address);
      const balanceInEth = ethers.utils.formatEther(balance);
      savedUnityInstance.SendMessage(
        'Canvas',
        'UpdateWalletBalance',
        JSON.stringify({
          bal: balanceInEth,
          invocationId,
        })
      );
    } else if (selectedWalletName === 'BLOCTO') {
      const result = await fcl.account(address);
      const bal = result.balance / Math.pow(10, 8);
      savedUnityInstance.SendMessage(
        'Canvas',
        'UpdateWalletBalance',
        JSON.stringify({
          bal,
          invocationId,
        })
      );
    } else if (selectedWalletName === 'PETRA') {
      aptosClient.getAccount(address).then(async (acc: any) => {
        const bal = await coinClient.checkBalance(address);
        savedUnityInstance.SendMessage(
          'Canvas',
          'UpdateWalletBalance',
          JSON.stringify({
            bal: Number(bal) * Math.pow(10, -8),
            invocationId,
          })
        );
      });
    } else if (selectedWalletName === 'SUI_WALLET') {
      // TODO - Implementation
    }
  };

  // const handleChangeNetwork = async (newNetworkName: string) => {
  //   console.log('Handle Change Network to: ', newNetworkName);

  //   if (newNetworkName === 'polygon_mainnet') {
  //     const chainId = 137; // Polygon Mainnet

  //     if (window.ethereum.networkVersion !== chainId) {
  //       try {
  //         await window.ethereum.request({
  //           method: 'wallet_switchEthereumChain',
  //           params: [{ chainId: web3.utils.toHex(chainId) }],
  //         });
  //         console.log(web3.currentProvider.selectedAddress);
  //         savedUnityInstance.SendMessage(
  //           'Canvas',
  //           'ShowConnectionSuccess',
  //           web3.currentProvider.selectedAddress
  //         );
  //       } catch (err: any) {
  //         // This error code indicates that the chain has not been added to MetaMask
  //         if (err.code === 4902) {
  //           await window.ethereum.request({
  //             method: 'wallet_addEthereumChain',
  //             params: [
  //               {
  //                 chainName: 'Polygon Mainnet',
  //                 chainId: web3.utils.toHex(chainId),
  //                 nativeCurrency: {
  //                   name: 'MATIC',
  //                   decimals: 18,
  //                   symbol: 'MATIC',
  //                 },
  //                 rpcUrls: ['https://polygon-rpc.com/'],
  //               },
  //             ],
  //           });
  //           console.log(web3.currentProvider.selectedAddress);
  //           savedUnityInstance.SendMessage(
  //             'Canvas',
  //             'ShowConnectionSuccess',
  //             web3.currentProvider.selectedAddress
  //           );
  //         }
  //       }
  //     }
  //   }
  //   if (newNetworkName === 'ethereum_mainnet') {
  //     const chainId = 1; // Ethereum Mainnet

  //     if (window.ethereum.networkVersion !== chainId) {
  //       try {
  //         await window.ethereum.request({
  //           method: 'wallet_switchEthereumChain',
  //           params: [{ chainId: web3.utils.toHex(chainId) }],
  //         });
  //         console.log(web3.currentProvider.selectedAddress);
  //         savedUnityInstance.SendMessage(
  //           'Canvas',
  //           'ShowConnectionSuccess',
  //           web3.currentProvider.selectedAddress
  //         );
  //       } catch (err: any) {
  //         // This error code indicates that the chain has not been added to MetaMask
  //         if (err.code === 4902) {
  //           await window.ethereum.request({
  //             method: 'wallet_addEthereumChain',
  //             params: [
  //               {
  //                 chainName: 'Ethereum Mainnet',
  //                 chainId: web3.utils.toHex(chainId),
  //                 nativeCurrency: {
  //                   name: 'ETH',
  //                   decimals: 18,
  //                   symbol: 'ETH',
  //                 },
  //                 rpcUrls: ['https://api.securerpc.com/v1'],
  //               },
  //             ],
  //           });
  //           console.log(web3.currentProvider.selectedAddress);
  //           savedUnityInstance.SendMessage(
  //             'Canvas',
  //             'ShowConnectionSuccess',
  //             web3.currentProvider.selectedAddress
  //           );
  //         }
  //       }
  //     }
  //   }
  //   if (newNetworkName === 'aptos_mainnet') {
  //     aptosClient = new AptosClient(
  //       'https://fullnode.mainnet.aptoslabs.com/v1'
  //     );
  //     coinClient = new CoinClient(aptosClient);
  //     await window.aptos.disconnect();
  //     window.aptos.connect().then((u: any) => {
  //       setUser(u);
  //       console.log('aptos user', u);
  //     });
  //   }
  //   if (newNetworkName === 'aptos_devnet') {
  //     aptosClient = new AptosClient(
  //       'https://fullnode.devnet.aptoslabs.com/v1/'
  //     );
  //     coinClient = new CoinClient(aptosClient);

  //     await window.aptos.disconnect();
  //     window.aptos.connect().then((u: any) => {
  //       setUser(u);
  //       console.log('aptos user', u);
  //     });
  //   }
  //   if (newNetworkName === 'flow_devnet') {
  //     fcl.unauthenticate();
  //     fcl
  //       .config()
  //       .put('accessNode.api', 'https://rest-testnet.onflow.org')
  //       .put('discovery.wallet.method', 'POP/RPC')
  //       .put(
  //         'challenge.handshake',
  //         'https://flow-wallet-testnet.blocto.app/authn'
  //       );
  //     fcl.authenticate();
  //   }
  //   if (newNetworkName === 'flow_mainnet') {
  //     fcl.unauthenticate();
  //     fcl
  //       .config()
  //       .put('accessNode.api', 'https://flow-access-mainnet.portto.io')
  //       .put('discovery.wallet.method', 'POP/RPC')
  //       .put('challenge.handshake', 'https://flow-wallet.blocto.app/authn');
  //     fcl.authenticate();
  //   }
  // };

  useEffect(() => {
    addEventListener('Web3Connect', web3Connect);
    addEventListener('FetchWalletBalance', fetchWalletBalance);
    addEventListener('SendFlowQuery', sendFlowQuery);
    addEventListener('SendEthereumTransaction', sendEthereumTransaction);
    addEventListener('SendAptosTransaction', sendAptosTransaction);
    addEventListener('Web3SignMessage', signMessage);
    return () => {
      removeEventListener('Web3Connect', web3Connect);
      removeEventListener('FetchWalletBalance', fetchWalletBalance);
      removeEventListener('SendFlowQuery', sendFlowQuery);
      removeEventListener('SendEthereumTransaction', sendEthereumTransaction);
      removeEventListener('SendAptosTransaction', sendAptosTransaction);
      removeEventListener('Web3SignMessage', signMessage);
    };
  }, [addEventListener, removeEventListener]);

  const canvasRef = useRef<HTMLCanvasElement>(null);

  const sendFlowQuery = async (invocationId: string, txReqObj: string) => {
    const reqData = JSON.parse(txReqObj);
    const queryRes = await fcl.query({
      cadence: reqData.cadenceScript,
      args: (arg: any, t: any) => [
        arg(reqData.args[0].val, t[reqData.args[0].type]),
      ],
    });

    savedUnityInstance.SendMessage(
      'Canvas',
      'TransactionResponseListener',
      JSON.stringify({
        invocationId,
        txData: JSON.stringify(queryRes),
      })
    );
  };

  const sendEthereumTransaction = async (
    invocationId: string,
    txReqObj: string
  ) => {
    let txObj = JSON.parse(txReqObj);
    txObj = JSON.parse(txObj.rawTransaction);
    const provider = new ethers.providers.Web3Provider(window.ethereum, 'any');
    // get a signer wallet!
    const signer = provider.getSigner();
    // Creating a transaction param
    const tx: any = {
      ...txObj,
      value: ethers.utils.formatBytes32String(txObj.value),
      nonce: await provider.getTransactionCount(txObj.from, 'latest'),
    };
    try {
      const res = await signer.sendTransaction(tx);
      savedUnityInstance.SendMessage(
        'Canvas',
        'TransactionResponseListener',
        JSON.stringify({
          invocationId,
          txData: JSON.stringify(res),
        })
      );
    } catch (error) {
      savedUnityInstance.SendMessage(
        'Canvas',
        'ErrorListener',
        JSON.stringify({
          invocationId,
          error,
        })
      );
    }

    // web3.eth.sendTransaction(
    //   { to, from, value },
    //   function (err: any, res: any) {
    //     if (err) {
    //       savedUnityInstance.SendMessage(
    //         'Canvas',
    //         'ErrorListener',
    //         JSON.stringify({
    //           invocationId,
    //           error: 'Not able to connect to Aptos!',
    //         })
    //       );
    //     } else {
    //       savedUnityInstance.SendMessage(
    //         'Canvas',
    //         'TransactionResponseListener',
    //         JSON.stringify({
    //           invocationId,
    //           txData: JSON.stringify(res),
    //         })
    //       );
    //     }
    //   }
    // );
  };

  const sendAptosTransaction = async (invocationId: string, txReqObj: any) => {
    try {
      let reqData = JSON.parse(txReqObj);
      reqData = JSON.parse(reqData.rawTransaction);

      const pendingTransaction = await (
        window as any
      ).aptos.signAndSubmitTransaction(reqData);

      savedUnityInstance.SendMessage(
        'Canvas',
        'TransactionResponseListener',
        JSON.stringify({
          invocationId,
          txData: JSON.stringify(pendingTransaction),
        })
      );
    } catch (e) {
      savedUnityInstance.SendMessage(
        'Canvas',
        'ErrorListener',
        JSON.stringify({
          invocationId,
          e,
        })
      );
    }
  };

  const signMessage = async (invocationId: string, txReqObj: string) => {
    try {
      const reqData = JSON.parse(txReqObj);
      if (selectedBlockchain === 'APTOS') {
        const response = await window.aptos.signMessage({
          message: reqData?.message,
          nonce: reqData?.nonce,
        });
        savedUnityInstance.SendMessage(
          'Canvas',
          'TransactionResponseListener',
          JSON.stringify({
            invocationId,
            txData: JSON.stringify(response),
          })
        );
      }
      if (selectedBlockchain === 'ETHEREUM') {
        let messageHash = ethers.utils.id(reqData?.message);
        // let messageHash = ethers.utils.id('Hello World');
        let messageHashBytes = ethers.utils.arrayify(messageHash);
        const provider = new ethers.providers.Web3Provider(
          window.ethereum,
          'any'
        );
        // get a signer wallet!
        const signer = provider.getSigner();
        // Sign the binary data
        let flatSig = await signer.signMessage(messageHashBytes);
        // For Solidity, we need the expanded-format of a signature
        let sig = ethers.utils.splitSignature(flatSig);
        savedUnityInstance.SendMessage(
          'Canvas',
          'TransactionResponseListener',
          JSON.stringify({
            invocationId,
            txData: JSON.stringify(sig),
          })
        );
      }
      if (selectedBlockchain === 'FLOW') {
        const MSG = Buffer.from(reqData?.message).toString('hex');
        const signedMessage = await fcl.currentUser.signUserMessage(MSG);
        savedUnityInstance.SendMessage(
          'Canvas',
          'TransactionResponseListener',
          JSON.stringify({
            invocationId,
            txData: JSON.stringify(signedMessage),
          })
        );
      }
      if (selectedBlockchain === 'SUI') {
        // TODO - Implementation
      }
    } catch (e) {
      savedUnityInstance.SendMessage(
        'Canvas',
        'ErrorListener',
        JSON.stringify({
          invocationId,
          e,
        })
      );
    }
  };

  return (
    <Fragment>
      <div className="container">
        <Unity
          unityProvider={unityProvider}
          style={{ height: 768, width: 1024 }}
          devicePixelRatio={window.devicePixelRatio}
          ref={canvasRef}
        />
      </div>
    </Fragment>
  );
}

export { MatrixUnityPlayer };
