import { PeraWalletConnect } from "@perawallet/connect"
import { ChainType, NETWORK_MAPPING, NetworkTypeKey, WalletType } from "../constants";
import { AlgorandChainIDs } from "@perawallet/connect/dist/util/peraWalletTypes";
import { Wallet } from "./Wallet";
import algosdk from "algosdk";

const ALGORAND_CLIENT_SERVER_URL = 'https://testnet-algorand.api.purestake.io/ps2';
const ALGORAND_TOKEN = {
    'X-API-Key': ''
}

let instance: PeraWallet | null = null;
let peraWallet: PeraWalletConnect;
let algod: algosdk.Algodv2;

let tempSignedTx: any;
export class PeraWallet extends Wallet {
    walletType = WalletType.PeraWallet;

    constructor(chainType: ChainType, networkType: NetworkTypeKey) {
        super(chainType, networkType);

        if (!instance) {
            console.log("chainID", NETWORK_MAPPING[networkType] as AlgorandChainIDs);
            peraWallet = new PeraWalletConnect({
                chainId: 416002, //NETWORK_MAPPING[networkType] as AlgorandChainIDs
            });
            algod = new algosdk.Algodv2(ALGORAND_TOKEN, ALGORAND_CLIENT_SERVER_URL, '');
            instance = this;
        }
        return instance;
    }

    async connect(): Promise<void> {
        try {
            const newAccounts = await peraWallet.connect()
            this.userAddress = newAccounts[0]
        } catch (e) {
            console.log("Error connecting to", this.walletType, e)
        }
    }

    async disconnect(): Promise<void> {
        peraWallet.disconnect();
        this.userAddress = null;
    }

    getAddress(): String | null | undefined {
        return this.userAddress
    }



    async signTransaction(txReqObject: string): Promise<any> {
        const parsed = JSON.parse(txReqObject);
        const txGroups = await generatePaymentTxns({
            to: "GD64YIY3TWGDMCNPP553DZPPR6LDUSFQOIJVFDPPXWEG3FVOJCCDBBHU5A",
            initiatorAddr: this.userAddress as string
        });
        const signedTxn = await peraWallet.signTransaction([txGroups]);
        console.log(signedTxn);
        tempSignedTx = signedTxn;
        return { blob: signedTxn };
    }

    async sendTransaction(txReqObject: string): Promise<void> {
        const parsed = JSON.parse(txReqObject);
        try {
            const data = await algod.sendRawTransaction(tempSignedTx).do();
            console.log(data)
        } catch (error) {
            console.log("Couldn't send txns", error);
        }
    }

    async fetchWalletBalance(reqObj: string): Promise<any> {
        const accountInfo = await algod.accountInformation(this.userAddress as string).do();
        console.log(accountInfo);
        return accountInfo;
    }
}

async function generatePaymentTxns({
    to,
    initiatorAddr
}: {
    to: string;
    initiatorAddr: string;
}) {
    const suggestedParams = await algod.getTransactionParams().do();

    const txn = algosdk.makePaymentTxnWithSuggestedParamsFromObject({
        from: initiatorAddr,
        to,
        amount: 1,
        suggestedParams
    });

    return [{ txn, signers: [initiatorAddr] }];
}