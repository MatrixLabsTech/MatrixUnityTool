import { ChainType, NetworkTypeKey, WalletType } from "../constants";
import { PeraWallet } from "./PeraWallet";
import { Wallet } from "./Wallet";

export class WalletFactory {
    private static walletInstances: any = {
        [WalletType.PeraWallet]: null,
        [WalletType.Metamask]: null
    };
    static getWallet(chainType: ChainType, walletType: WalletType, networkType: NetworkTypeKey) {
        if (!this.walletInstances[walletType]) {
            switch (walletType) {
                case WalletType.PeraWallet:
                    return new PeraWallet(chainType, networkType);
                // Add cases for other wallet types if needed
                default:
                    throw new Error(`Unsupported wallet type: ${walletType}`);
            }
        }
        return this.walletInstances[walletType];
    }
}