import { ChainType, NETWORK_MAPPING, NetworkTypeKey, WalletType } from "../constants";
import { Wallet } from "./Wallet";
import { ethers } from "ethers";

let provider: ethers.providers.Web3Provider;
export class MetamaskWallet extends Wallet {
    walletType = WalletType.Metamask;

    constructor(chainType: ChainType, networkType: NetworkTypeKey) {
        super(chainType, networkType);
        console.log("chainID", NETWORK_MAPPING[networkType]);
        provider = new ethers.providers.Web3Provider(window.ethereum);
    }

    async connect(): Promise<void> {
        try {
            await window.ethereum.request({ method: 'eth_requestAccounts' });
            const accounts = await provider.listAccounts();
            this.userAddress = accounts[0]
        } catch (e) {
            console.log("Error connecting to", this.walletType, e)
        }
    }

    getAddress(): String | null | undefined {
        return this.userAddress
    }

    async sendTransaction(txReqObj: string): Promise<any> {
        let txObj = JSON.parse(txReqObj);
        txObj = JSON.parse(txObj.rawTransaction);
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
            const waitedRes = await res.wait()
            return waitedRes;
        } catch (error) {
            console.log("Error sending transaction", error)
            throw error;
        }
    }

    async signMessage(reqObj: string): Promise<any> {
        const reqData = JSON.parse(reqObj);
        let messageHash = ethers.utils.id(reqData?.message);
        let messageHashBytes = ethers.utils.arrayify(messageHash);
        // get a signer wallet!
        const signer = provider.getSigner();
        // Sign the binary data
        let flatSig = await signer.signMessage(messageHashBytes);
        // For Solidity, we need the expanded-format of a signature
        let sig = ethers.utils.splitSignature(flatSig);
        return JSON.stringify(sig)
    }

    async fetchWalletBalance(reqObj: string): Promise<any> {
        const balance = await provider.getBalance(this.userAddress as string);
        const balanceInEth = ethers.utils.formatEther(balance);
        return balanceInEth
    }
}