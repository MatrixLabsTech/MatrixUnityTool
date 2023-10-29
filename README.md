# MatrixUnityToolkit (MUT)
<div style="display:flex">
<img src="https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white" />

<img src="https://img.shields.io/badge/Ethereum-3C3C3D?style=for-the-badge&logo=Ethereum&logoColor=white" />

<img src="https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white" />

<img src="https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB" />
</div>
</br>

**_NOTE:_**  MatrixUnityToolkit recently got updated to V2 with additional support of Algorand and suppporting wallets. If you were working with V1, please make sure you have checked latest API updates

![MultiChain WebGL ToolKit](https://raw.githubusercontent.com/MatrixLabsTech/MatrixUnityTool/main/Docs/v2/banner.png)

## Usage

#### 1. Importing

You should import the `MatrixSDK` namespace into your script.

```csharp
using MatrixSDK;
```

#### 2. Initializing the Toolkit

We use a singleton approach, to import the sdk, it is as simple as this:

```csharp
private MatrixUnityToolkit mw;

void Start() {
    mw = MatrixUnityToolkit.Instance;
}
```

#### 3. Setting Configuration

Matrix Unity Toolkit supports multiple chains and wallets including Algorand, Ethereum, Flow, Aptos & Sui. Follow the example to configure the Web3 wallets and chains for your application:
```csharp
mw.SetConfig(new WalletName[] { WalletName.PeraWallet, WalletName.MyAlgoConnect }, Blockchain.Algorand, "4160"); // chainIDs: 416001, 416002, 416003 , 4160
```

#### 4. Connecting to a Wallet

We provide 2 ways to connect to a wallet. Each method requires you to pass in a callback function (or not) which provides an instance of the wallet after connection is made:
1. Using the provided UI (similar to WalletConnect)
```csharp
mw.Connect(this.WalletConnected);
```
2. Manually selecting the wallet
```csharp
mw.Connect(WalletName.PeraWallet, this.WalletConnected);
```

#### 4. Use Web3 Features!

##### 1. Get Balance
Getting balance of user's account can be done using the following:
```csharp
  this.walletInstance.GetBalance("{}", null, null, false);
```
Here, we can pass any optional metadata, onSuccess, onError functions, enable timeout, set timeout duration

##### 2. Sign Message

Sign transaction can be done using the following:
```csharp
  this.walletInstance.SignMessage("sample message", null, null, false);
```
Here, we can pass any message, onSuccess, onError functions, enable timeout, set timeout duration

##### 3. Sign Transaction

Sign transaction can be done using the following:
```csharp
  this.walletInstance.SignTransaction("{}", null, null, false);
```
Here, we can pass any optional metadata, onSuccess, onError functions, enable timeout, set timeout duration

##### 4. Send Transaction

Send transaction can be done using the following:
```csharp
  this.walletInstance.SendTransaction("{}", null, null, false);
```
Here, we can pass any optional metadata, onSuccess, onError functions, enable timeout, set timeout duration


## Demo Video (Outdated)

<div align="center">
      <a href="https://www.youtube.com/watch?v=QHrcnESh7zc">
     <img
      src="https://img.youtube.com/vi/QHrcnESh7zc/0.jpg"
      alt="Demo Video"
      style="width:100%;">
      </a>
    </div>

## Design Doc

**Introduction**

## Overview
 Matrix Unity Tool is a multichain Web3 toolkit which helps integrate Web3 to Unity WebGL based games. It provides a easy-to-use set of APIs inside unity for game developers to design their multichain games.

- Helps integrate WebGL Unity 3D build with Web
- Provides a framework and a standard UI for integrating Web 3 Auth for Algorand, Ethereum, Flow, Aptos & Sui.
- Provides a package for Unity 3D and React
- Customizable UI, API for easy integration
- Provides API for signing messages, transactions

## Product Requirements

1. A game developer should be able to integrate the package provided to add Web support to the non-Web3 game
1. A game developer should be able to make use of the provided sample UI for integrating Web3 authentication in Unity3D games
2. A game developer should not be required to modify React code as It should be plug-and-play
3. A game developer should be able to make use of the provided API for connecting a wallet, carrying out transactions, and fetching wallet balance

## Technical Specifications

Tech Stack -

```
Frontend -  React, Typescript
Client -  Unity 3D (and C#)
Others - https://react-unity-webgl.dev/
```

## Future Goals

1. **Supporting New Chains & Wallets** - Add new Classes (parsers) forintegrating more wallets
2. **Documentation** - create easy to follow documentation and a video tutorial series for game developers to use this package.
3. **Adding more APIs** - Add more APIs for blockchain, such as NFT creation, and coin transfer, making it even easier for the developers
4. **Auto-generate React App** - We want to create a CLI-based application that auto-generates a React app that can be directly used as the client front-end.

## Assumptions

1. The user (game developer) is familiar with Unity and Web3 terminologies and APIs
2. Only WebGL build is supported

**Solutions**

## Integration Flow

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/integration-flow.png?raw=true)

## UI Flow

1. **Game Demo**
   ![MultiChain WebGL ToolKit](https://raw.githubusercontent.com/MatrixLabsTech/MatrixUnityTool/main/Docs/v2/MUT_1.png)


2. **Select Network (part of Game Demo)**
   ![MultiChain WebGL ToolKit](https://raw.githubusercontent.com/MatrixLabsTech/MatrixUnityTool/main/Docs/v2/MUT_2.png)

3. **Select a Wallet**

  ![MultiChain WebGL ToolKit](https://raw.githubusercontent.com/MatrixLabsTech/MatrixUnityTool/main/Docs/v2/MUT_3.png)

4. **Connection to a web wallet** (eg; Pera Wallet)

  ![MultiChain WebGL ToolKit](https://raw.githubusercontent.com/MatrixLabsTech/MatrixUnityTool/main/Docs/v2/MUT_4.png)

   ![MultiChain WebGL ToolKit](https://raw.githubusercontent.com/MatrixLabsTech/MatrixUnityTool/main/Docs/v2/MUT_5.png)



## High-Level Communication Flow

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/MatrixLabs-Working-High-Level.png?raw=true)

## Definitions

## Blockchain

```
Name | Status | Features
ETHEREUM - Active - Connect Metamask wallet, Connect to
different networks, Fetch balance
FLOW - Active - Connect Blocto wallet, Connect to different
networks, Fetch balance, flow queries
APTOS - Active - Connect Petra wallet, Connect to different
networks, Fetch balance
SUI - In-Development -
```

## BlockchainNetwork

```
Name | Description
ETHEREUM_TESTNET - Ethereum testnet
ETHEREUM_MAINNET - Ethereum mainnet
POLYGON_MAINNET - Polygon mainnet
FLOW_DEVNET - Flow devnet
FLOW_MAINNET - Flow mainnet
APTOS_DEVNET - Aptos devnet
APTOS_MAINNET - Aptos mainnet
BSC_MAINNET - Binance Smart Chain Mainnet
SUI_DEVNET - Sui Devnet
```

## Wallet

```
Name | Status
METAMASK - Active
BLOCTO - Active
PETRA - Active
SUI_WALLET - Not-Active
```

## Web3WalletConnectRequest

```
Key | Type | Description
walletName - string - Wallet name
blockchain - string - Refer: #Blockchain
network - string - Refer: #BlockchainNetwork
```

## FlowArg

```
Key | Type | Description
val - string - Value as defined by https://developers.flow.com/tools/fcl-js/reference/api#argumentfunction
type - string - Refer: https://developers.flow.com/tools/fcl-js/reference/api#argumentfunction
```

## Web3FlowQueryRequest

```
Key | Type | Description
cadenceScript - string Cadence script string args List<FlowArg> Argos (optional) as required by the cadence script
```

## Web3EthereumTxRequest

```
Key Type Description
rawTransaction - string - JSON of the raw transaction, note: convert any function to absolute values
```

## Web3WalletBalRequest

```
Key | Type | Description
address - string - Requests wallet balance for any of the #Blockchain for the provided address
```

## Web3WalletConnectResponse

```
Key | Type | Description
invocationId - string - UUID of the request
bal - float - Wallet balance as float type
```

## Web3TransactionResponse

```
Key | Type | Description
invocationId - string - UUID of the request
txData - string - JSON response which can be parsed as required
```

## Web3Error

```
Key | Type | Description
invocationId - string - UUID of the request
message - string - Error message string
```

## References

1. https://flow-view-source.com/
2. https://developers.flow.com/tools/fcl-js/reference/api
3. https://aptos.dev/guides/aptos-guides
4. https://github.com/jeffreylanters/react-unity-webgl/issues/
5. https://github.com/jeffreylanters/react-unity-webgl/discussions/
6. https://ethereum.stackexchange.com/questions/92095/web3-current-best-practice-to-connect-metamask-to-chrome
1. https://stackoverflow.com/questions/56663785/invalid-hook-call-hooks-can-only-be-called-inside-of-the-body-of-a-function-com
