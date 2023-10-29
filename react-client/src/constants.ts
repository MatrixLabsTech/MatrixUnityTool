export enum ALGORAND_CHAINIDS {
    "MainNet" = "416001",
    "TestNet" = "416002",
    "BetaNet" = "416003",
    "All_Networks" = "4160",
}

export enum WalletType {
    "Metamask" = "Metamask",
    "PeraWallet" = "PeraWallet",
    "PetraWallet" = "PetraWallet",
    "Blocto" = "Blocto",
    "MyAlgoConnect" = "MyAlgoConnect"
}

export enum NETWORK_MAPPING {
    ETHEREUM_MAINNET = 'mainnet',
    ALGORAND_MAINNET = 416001,
    ALGORAND_TESTNNET = 416002,
    ALGORAND_BETANET = 416003,
    ALGORAND_ALLNETWORKS = 4160,
};

export type NetworkTypeKey = keyof typeof NETWORK_MAPPING;

export const ALGORAND_CLIENT_SERVER_URL = ""
export const ALGORAND_CLIENT_SERVER_PORT = ""

export enum ChainType {
    "Ethereum" = "Ethereum",
    "Flow" = "Flow",
    "Algorand" = "Algorand",
    "Sui" = "Sui",
    "Aptos" = "Aptos"
}