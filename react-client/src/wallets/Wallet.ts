import { error } from "console";
import { ChainType, NetworkTypeKey } from "../constants";

export class Wallet {
    userAddress: String | null | undefined;
    chainType: ChainType | null | undefined;
    networkType: string | undefined;

    constructor(chainType: ChainType, networkType: NetworkTypeKey) {
        if (chainType) this.chainType = chainType
        if (networkType) this.networkType = networkType
        console.log("Inititalisation function for wallet");
    }

    getAddress(): String | null | undefined {
        throw new Error("Get Address not supported");
    }

    async connect(): Promise<void> {
        throw new Error("Connect not supported");
    }

    async disconnect(): Promise<void> {
        throw new Error("Disconnect not supported");
    }

    async signMessage(reqObj: string): Promise<any> {
        throw new Error("Sign Message function not supported");
    }

    async signTransaction(txReqObject: string): Promise<any> {
        throw new Error("Sign Transaction function not supported");
    }

    async sendTransaction(txReqObject: string): Promise<any> {
        throw new Error("Send Transaction function not supported");
    }

    async fetchWalletBalance(reqObj: string): Promise<any> {
        throw new Error("Fetch wallet balance not supported.")
    }

}