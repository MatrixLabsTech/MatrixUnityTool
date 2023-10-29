import { AptosClient, CoinClient } from "aptos";
import { ChainType, NETWORK_MAPPING, NetworkTypeKey, WalletType } from "../constants";
import { Wallet } from "./Wallet";
const fcl = require('@blocto/fcl');

let aptosClient: AptosClient;
let coinClient: CoinClient;

export class PetraWallet extends Wallet {
    walletType = WalletType.PetraWallet

    constructor(chainType: ChainType, networkType: NetworkTypeKey) {
        super(chainType, networkType);
        console.log("chainID", NETWORK_MAPPING[networkType]);
        aptosClient = new AptosClient(
            'https://fullnode.devnet.aptoslabs.com/v1/'
        );
        coinClient = new CoinClient(aptosClient);
    }

    setUser(val: any) {
        this.userAddress = val.addr
    }

    async connect(): Promise<void> {
        try {
            const user = await window.aptos.connect()
            this.userAddress = user?.address;
        } catch (e) {
            console.log("Error connecting to", this.walletType, e)
        }
    }

    getAddress(): String | null | undefined {
        return this.userAddress
    }

    async sendTransaction(txReqObj: string): Promise<any> {
        let reqData = JSON.parse(txReqObj);
        reqData = JSON.parse(reqData.rawTransaction);

        const pendingTransaction = await (
            window as any
        ).aptos.signAndSubmitTransaction(reqData);

        return JSON.stringify(pendingTransaction);
    }

    async signMessage(reqObj: string): Promise<any> {
        const reqData = JSON.parse(reqObj);
        const signedMessage = await window.aptos.signMessage({
            message: reqData?.message,
            nonce: reqData?.nonce,
        });
        return JSON.stringify(signedMessage)
    }

    async fetchWalletBalance(reqObj: string): Promise<any> {
        const result = await fcl.account(this.userAddress);
        const bal = result.balance / Math.pow(10, 8);
        return bal;
    }
}