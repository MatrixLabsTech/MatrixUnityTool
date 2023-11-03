import { ChainType, NETWORK_MAPPING, NetworkTypeKey, WalletType } from "../constants";
import { Wallet } from "./Wallet";
const fcl = require('@blocto/fcl');


export class BloctoWallet extends Wallet {
    walletType = WalletType.Blocto;

    constructor(chainType: ChainType, networkType: NetworkTypeKey) {
        super(chainType, networkType);
        console.log("chainID", NETWORK_MAPPING[networkType]);
        // flow code
        fcl
            .config()
            .put('accessNode.api', 'https://rest-testnet.onflow.org')
            .put('discovery.wallet.method', 'POP/RPC')
            .put(
                'challenge.handshake',
                'https://flow-wallet-testnet.blocto.app/authn'
            );
    }

    setUser(val: any) {
        this.userAddress = val.addr
    }

    async connect(): Promise<void> {
        try {
            fcl.authenticate();
            fcl.currentUser().subscribe(this.setUser);
        } catch (e) {
            console.log("Error connecting to", this.walletType, e)
        }
    }
    async disconnect(): Promise<void> {
        fcl.unauthenticate();
    }

    getAddress(): String | null | undefined {
        return this.userAddress
    }

    async sendTransaction(txReqObj: string): Promise<any> {
        const reqData = JSON.parse(txReqObj);
        const queryRes = await fcl.query({
            cadence: reqData.cadenceScript,
            args: (arg: any, t: any) => [
                arg(reqData.args[0].val, t[reqData.args[0].type]),
            ],
        });
        return JSON.stringify(queryRes)
    }

    async signMessage(reqObj: string): Promise<any> {
        const reqData = JSON.parse(reqObj);
        const MSG = Buffer.from(reqData?.message).toString('hex');
        const signedMessage = await fcl.currentUser.signUserMessage(MSG);
        return JSON.stringify(signedMessage)
    }

    async fetchWalletBalance(reqObj: string): Promise<any> {
        const result = await fcl.account(this.userAddress);
        const bal = result.balance / Math.pow(10, 8);
        return bal;
    }
}