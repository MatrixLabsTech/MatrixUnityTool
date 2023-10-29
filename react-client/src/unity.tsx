import { Fragment, useEffect, useRef, useState } from 'react';
import { Unity, useUnityContext } from 'react-unity-webgl/distribution/exports';
import { AptosClient, CoinClient } from 'aptos';
import { ethers } from 'ethers';
import { ChainType, NETWORK_MAPPING, WalletType } from './constants';
import { WalletFactory } from './wallets/WalletFactory';
const fcl = require('@blocto/fcl');

let aptosClient: AptosClient;
let coinClient: CoinClient;

fcl
  .config()
  .put('accessNode.api', 'https://rest-testnet.onflow.org')
  .put('discovery.wallet.method', 'POP/RPC')
  .put('challenge.handshake', 'https://flow-wallet-testnet.blocto.app/authn');

let savedUnityInstance: any;
let selectedWalletName: WalletType;
let selectedNetwork: any;
let selectedBlockchain: ChainType;
let connectReqId: string | null;

function MatrixUnityPlayer() {
  const {
    unityProvider,
    addEventListener,
    removeEventListener,
    UNSAFE__unityInstance,
  } = useUnityContext({
    codeUrl: `/unity-build/build-2.wasm`,
    dataUrl: `/unity-build/build-2.data`,
    frameworkUrl: `/unity-build/build-2.framework.js`,
    loaderUrl: `/unity-build/build-2.loader.js`,
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
        'Canvas/MatrixUnitySDK',
        'MatrixUnityToolkitResponse',
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
        'Canvas/MatrixUnitySDK',
        'MatrixUnityToolkitResponse',
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
    const walletName = reqData.walletName as WalletType;
    selectedWalletName = walletName;
    selectedBlockchain = reqData.blockchain as ChainType;

    selectedNetwork = reqData.network;
    console.log(reqData);
    console.log(walletName, selectedBlockchain);

    const walletInstance = WalletFactory.getWallet(
      selectedBlockchain,
      selectedWalletName,
      selectedNetwork
    );
    console.log(walletInstance);
    await walletInstance?.connect();
    savedUnityInstance.SendMessage(
      'Canvas/MatrixUnitySDK',
      'MatrixUnityToolkitResponse',
      JSON.stringify({
        isSuccess: true,
        invocationId,
        resData: JSON.stringify({
          address: walletInstance?.getAddress(),
        }),
      })
    );

    connectReqId = null;

    // if (walletName === WalletType.Metamask && window.ethereum) {
    //   const provider = new ethers.providers.Web3Provider(window.ethereum);
    //   await window.ethereum.request({ method: 'eth_requestAccounts' });
    //   const accounts = await provider.listAccounts();
    //   savedUnityInstance.SendMessage(
    //     'Canvas',
    //     'MatrixUnityToolkitResponse',
    //     JSON.stringify({
    //       invocationId,
    //       address: accounts[0],
    //     })
    //   );
    //   connectReqId = null;
    // } else if (walletName === WalletType.Blocto) {
    //   // flow code
    //   fcl
    //     .config()
    //     .put('accessNode.api', 'https://rest-testnet.onflow.org')
    //     .put('discovery.wallet.method', 'POP/RPC')
    //     .put(
    //       'challenge.handshake',
    //       'https://flow-wallet-testnet.blocto.app/authn'
    //     );
    //   fcl.authenticate();
    //   fcl.currentUser().subscribe(setUser);
    // } else if (walletName === WalletType.PetraWallet && window.aptos) {
    //   aptosClient = new AptosClient(
    //     'https://fullnode.devnet.aptoslabs.com/v1/'
    //   );
    //   coinClient = new CoinClient(aptosClient);

    //   window.aptos
    //     .connect()
    //     .then((u: any) => {
    //       setUser(u);
    //     })
    //     .catch((e: any) => {
    //       savedUnityInstance.SendMessage(
    //         'Canvas',
    //         'ErrorListener',
    //         JSON.stringify({
    //           invocationId,
    //           message: 'Not able to connect to Aptos!',
    //         })
    //       );
    //       connectReqId = null;
    //     });
    // } else if (walletName === WalletType.PeraWallet) {
    //   const walletInstance = WalletFactory.getWallet(
    //     ChainType.ALGORAND,
    //     selectedWalletName,
    //     selectedNetwork
    //   );
    //   await walletInstance?.connect();
    //   savedUnityInstance.SendMessage(
    //     'Canvas',
    //     'MatrixUnityToolkitResponse',
    //     JSON.stringify({
    //       invocationId,
    //       address: walletInstance?.getAddress(),
    //     })
    //   );

    //   connectReqId = null;
    // } else {
    //   console.log('No wallet');
    //   connectReqId = null;
    // }
  };

  // const fetchWalletBalance = async (
  //   invocationId: string,
  //   walletBalReq: string
  // ): Promise<any> => {
  //   const reqData = JSON.parse(walletBalReq);
  //   const address = reqData.address;

  //   const metamaskWallet = WalletFactory.getWallet(
  //     selectedChainType!,
  //     selectedWalletName,
  //     selectedNetwork
  //   );
  //   const balRes = await metamaskWallet?.fetchWalletBalance(reqData);
  //   savedUnityInstance.SendMessage(
  //     'Canvas',
  //     'UpdateWalletBalance',
  //     JSON.stringify({
  //       bal: balRes,
  //       invocationId,
  //     })
  //   );

  //   if (selectedWalletName === 'METAMASK') {
  //     // const provider = ethers.getDefaultProvider(selectedNetwork);
  //     // const balance = await provider.getBalance(address);
  //     // const balanceInEth = ethers.utils.formatEther(balance);
  //     // savedUnityInstance.SendMessage(
  //     //   'Canvas',
  //     //   'UpdateWalletBalance',
  //     //   JSON.stringify({
  //     //     bal: balanceInEth,
  //     //     invocationId,
  //     //   })
  //     // );
  //     // implement for Algorand
  //   } else if (selectedWalletName === 'BLOCTO') {
  //     const result = await fcl.account(address);
  //     const bal = result.balance / Math.pow(10, 8);
  //     savedUnityInstance.SendMessage(
  //       'Canvas',
  //       'UpdateWalletBalance',
  //       JSON.stringify({
  //         bal,
  //         invocationId,
  //       })
  //     );
  //   } else if (selectedWalletName === 'PETRA') {
  //     aptosClient.getAccount(address).then(async (acc: any) => {
  //       const bal = await coinClient.checkBalance(address);
  //       savedUnityInstance.SendMessage(
  //         'Canvas',
  //         'UpdateWalletBalance',
  //         JSON.stringify({
  //           bal: Number(bal) * Math.pow(10, -8),
  //           invocationId,
  //         })
  //       );
  //     });
  //   } else if (selectedWalletName === 'SUI_WALLET') {
  //     // TODO - Implementation
  //   } else if (selectedWalletName === WalletType.PERA_WALLET) {
  //   }
  // };

  const sdkInteractionHandler = async (
    invocationId: string,
    methodName: string,
    reqObj: string
  ) => {
    console.log(
      'received JSSDKInteraction from Unity',
      invocationId,
      methodName,
      reqObj
    );
    try {
      if (methodName === 'Connect') {
        web3Connect(invocationId, reqObj);
      }
      if (methodName === 'GetBalance') {
        const walletInstance = WalletFactory.getWallet(
          selectedBlockchain,
          selectedWalletName,
          selectedNetwork
        );
        const balRes = await walletInstance?.fetchWalletBalance(reqObj);
        savedUnityInstance.SendMessage(
          'Canvas/MatrixUnitySDK',
          'MatrixUnityToolkitResponse',
          JSON.stringify({
            isSuccess: true,
            invocationId,
            resData: JSON.stringify({
              bal: balRes,
            }),
          })
        );
      }
      if (methodName === 'SignTransaction') {
        const walletInstance = WalletFactory.getWallet(
          selectedBlockchain,
          selectedWalletName,
          selectedNetwork
        );
        const signTxRes = await walletInstance?.signTransaction(reqObj);
        savedUnityInstance.SendMessage(
          'Canvas/MatrixUnitySDK',
          'MatrixUnityToolkitResponse',
          JSON.stringify({
            resData: JSON.stringify(signTxRes),
            invocationId,
            isSuccess: true,
          })
        );
      }
      if (methodName === 'SendTransaction') {
        const walletInstance = WalletFactory.getWallet(
          selectedBlockchain,
          selectedWalletName,
          selectedNetwork
        );

        const txRes = await walletInstance?.sendTransaction(reqObj);
        savedUnityInstance.SendMessage(
          'Canvas/MatrixUnitySDK',
          'MatrixUnityToolkitResponse',
          JSON.stringify({
            resData: JSON.stringify(txRes),
            invocationId,
            isSuccess: true,
          })
        );
      }
    } catch (e: any) {
      console.log(e);
      savedUnityInstance.SendMessage(
        'Canvas/MatrixUnitySDK',
        'MatrixUnityToolkitResponse',
        JSON.stringify({
          resData: {
            error: e?.message,
          },
          isSuccess: false,
          invocationId,
        })
      );
    }
  };

  useEffect(() => {
    addEventListener('JSSDKInteraction', sdkInteractionHandler);
    return () => {
      removeEventListener('JSSDKInteraction', sdkInteractionHandler);
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
      'Canvas/MatrixUnitySDK',
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
        'Canvas/MatrixUnitySDK',
        'TransactionResponseListener',
        JSON.stringify({
          invocationId,
          txData: JSON.stringify(res),
        })
      );
    } catch (error) {
      savedUnityInstance.SendMessage(
        'Canvas/MatrixUnitySDK',
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
        'Canvas/MatrixUnitySDK',
        'TransactionResponseListener',
        JSON.stringify({
          invocationId,
          txData: JSON.stringify(pendingTransaction),
        })
      );
    } catch (e) {
      savedUnityInstance.SendMessage(
        'Canvas/MatrixUnitySDK',
        'ErrorListener',
        JSON.stringify({
          invocationId,
          e,
        })
      );
    }
  };

  // const signMessage = async (invocationId: string, txReqObj: string) => {
  //   try {
  //     const reqData = JSON.parse(txReqObj);
  //     if (selectedBlockchain === 'APTOS') {
  //       const response = await window.aptos.signMessage({
  //         message: reqData?.message,
  //         nonce: reqData?.nonce,
  //       });
  //       savedUnityInstance.SendMessage(
  //         'Canvas',
  //         'TransactionResponseListener',
  //         JSON.stringify({
  //           invocationId,
  //           txData: JSON.stringify(response),
  //         })
  //       );
  //     }
  //     if (selectedBlockchain === 'ETHEREUM') {
  //       let messageHash = ethers.utils.id(reqData?.message);
  //       // let messageHash = ethers.utils.id('Hello World');
  //       let messageHashBytes = ethers.utils.arrayify(messageHash);
  //       const provider = new ethers.providers.Web3Provider(
  //         window.ethereum,
  //         'any'
  //       );
  //       // get a signer wallet!
  //       const signer = provider.getSigner();
  //       // Sign the binary data
  //       let flatSig = await signer.signMessage(messageHashBytes);
  //       // For Solidity, we need the expanded-format of a signature
  //       let sig = ethers.utils.splitSignature(flatSig);
  //       savedUnityInstance.SendMessage(
  //         'Canvas',
  //         'TransactionResponseListener',
  //         JSON.stringify({
  //           invocationId,
  //           txData: JSON.stringify(sig),
  //         })
  //       );
  //     }
  //     if (selectedBlockchain === 'FLOW') {
  //       const MSG = Buffer.from(reqData?.message).toString('hex');
  //       const signedMessage = await fcl.currentUser.signUserMessage(MSG);
  //       savedUnityInstance.SendMessage(
  //         'Canvas',
  //         'TransactionResponseListener',
  //         JSON.stringify({
  //           invocationId,
  //           txData: JSON.stringify(signedMessage),
  //         })
  //       );
  //     }
  //     if (selectedBlockchain === 'SUI') {
  //       // TODO - Implementation
  //     }
  //   } catch (e) {
  //     savedUnityInstance.SendMessage(
  //       'Canvas',
  //       'ErrorListener',
  //       JSON.stringify({
  //         invocationId,
  //         e,
  //       })
  //     );
  //   }
  // };

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
