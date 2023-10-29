import { ALGORAND_CLIENT_SERVER_PORT, ALGORAND_CLIENT_SERVER_URL, ChainType, NETWORK_MAPPING, NetworkTypeKey, WalletType } from "../constants";
import { Wallet } from "./Wallet";
import algosdk from "algosdk";
import * as mac from '@randlabs/myalgo-connect';

let instance: MyAlgoConnect | null = null;
const myAlgoConnect = new mac.default();
let algod: algosdk.Algodv2
export class MyAlgoConnect extends Wallet {
    walletType = WalletType.MyAlgoConnect;

    constructor(chainType: ChainType, networkType: NetworkTypeKey) {
        super(chainType, networkType);

        if (!instance) {
            console.log("chainID", NETWORK_MAPPING[networkType]);
            algod = new algosdk.Algodv2("", ALGORAND_CLIENT_SERVER_URL, ALGORAND_CLIENT_SERVER_PORT);
            instance = this;
        } else {
            return instance;
        }
    }

    async connect(): Promise<void> {
        try {
            const accountsSharedByUser = await myAlgoConnect.connect();
            this.userAddress = accountsSharedByUser[0].address
        } catch (e) {
            console.log("Error connecting to", this.walletType, e)
        }
    }

    getAddress(): String | null | undefined {
        return this.userAddress
    }

    async signTransaction(txReqObject: string): Promise<object> {
        const parsedData = JSON.parse(txReqObject);
        /*
        {
            method: "makePaymentTxnWithSuggestedParamsFromObject",
            data: {
                params: {},
                from: "",
                to: "receiver",
                amount: 1000
            }
        }
        */
        if (parsedData?.method === "makePaymentTxnWithSuggestedParamsFromObject") {
            const txn = algosdk.makePaymentTxnWithSuggestedParamsFromObject({
                suggestedParams: parsedData?.params,
                from: parsedData?.sender,
                to: parsedData?.receiver,
                amount: parsedData?.amount, // 1000 // 0.001 Algo
            });
            const signedTxn = await myAlgoConnect.signTransaction(txn.toByte());
            return signedTxn;
        }

        throw new Error("Invalid data provided.")
    }

    async sendTransaction(txReqObject: string): Promise<any> {
        const parsedData = JSON.parse(txReqObject);
        const response = await algod.sendRawTransaction(parsedData.blob).do();
        return response;
    }
}