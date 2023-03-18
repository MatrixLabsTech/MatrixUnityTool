# MultiChain WebGL ToolKit

<img src="https://img.shields.io/badge/Unity-100000?style=for-the-badge&logo=unity&logoColor=white" />

<img src="https://img.shields.io/badge/Ethereum-3C3C3D?style=for-the-badge&logo=Ethereum&logoColor=white" />

<img src="https://img.shields.io/badge/TypeScript-007ACC?style=for-the-badge&logo=typescript&logoColor=white" />

<img src="https://img.shields.io/badge/React-20232A?style=for-the-badge&logo=react&logoColor=61DAFB" />

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/multichaintoolkit.png?raw=true)

## User Guide (Work in Progress)

https://docs.google.com/document/d/1Wo0OT6tPh25yekr_AqHw6vRr5a6Jg57RWoautkzapMw/edit?usp=sharing

## Demo Video

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

MultiChain Web3 ToolKit for Unity is a set of packages (scripts and UI) created to help game developers integrate Web3 APIs with Unity 3D WebGL Builds.

- Helps integrate WebGL Unity 3D build with Web
- Provides a framework and a standard UI for integrating Web 3 Auth for wallets such
  as Ethereum, Flow, Aptos, and Sui
- Provides a package for Unity 3D and React
- Customizable UI, API for easy integration
- Provides API for signing messages, transactions

## Product Requirements

1. A game developer should be able to integrate the package provided to add Web
   support to the non-Web3 game
2. A game developer should be able to make use of the provided sample UI for
   integrating Web3 authentication in Unity3D games
3. A game developer should not be required to modify React code as It should be
   plug-and-play
4. A game developer should be able to make use of the provided API for connecting a
   wallet, carrying out transactions, and fetching wallet balance

## Technical Specifications

Tech Stack -

```
Frontend -  React, Typescript
Client -  Unity 3D (and C#)
Others - https://react-unity-webgl.dev/
```

## Future Goals

1. **Adding New Wallets** - Add new Classes (parsers) forintegrating more wallets
2. **Readme** - create easy to follow documentation and a video tutorial series for game developers to use this package.
3. **Adding more APIs** - Add more APIs for blockchain, such as NFT creation, and coin
   transfer, making it even easier for the developers
4. **Auto-generate React App** - We want to create a CLI-based application that
   auto-generates a React app that can be directly used as the client front-end.

## Assumptions

1. The user (game developer) is familiar with Unity and Web3 terminologies and APIs
2. Only WebGL build is supported

**Solutions**

## Integration Flow

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/integration-flow.png?raw=true)

## UI Flow

1. **Select a Wallet** screen
   ![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_1.png?raw=true)

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_2.png?raw=true)

2. **Get Wallet Balance** screen
   ![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_3.png?raw=true)

3. **Message Signing** screen

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_4.png?raw=true)

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_5.png?raw=true)

4. **Transactions / Smart-Contract Interaction** screen

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_6.png?raw=true)

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_7.png?raw=true)

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/UI_8.png?raw=true)

## APIs

```
Function Name | Parameters | Description
SelectWallet - walletName: string - Invokes the JS function which connects the Web wallet
FetchLatestBalance - void - Invokes the JS function which fetches the balances and returns it back in an async manner
ErrorListener - jsonStr: string - Takes the object sent via the JS code containing the invocation ID and error message. It displays the error message and clears the invocation.
PerformFlowQuery - cadenceQuery: string, Args: List<FlowArg> - Invokes the JS function which performs the Flow Query (read) in Flow blockchain
TransactionResponseListener - jsonStr: string - Shows a success message with transaction data and clears the invocation
```

## High-Level Communication Flow

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/MatrixLabs-Working-High-Level.png?raw=true)

## State Management

![MultiChain WebGL ToolKit](https://github.com/MatrixLabsTech/MatrixUnityTool/blob/main/Docs/state-management.png?raw=true)

```
State | Description
NOT_CONNECTED - Game is not connected to the Web3 wallet
WALLET_CONNECTING - User initiated connection and it's being carried on. Once connected it will go to the IDLE state or if error, then the ERROR state.
IDLE - Wallet is connected, and the game is aware of the user
account address, blockchain name, and network name ERROR For any error such as a wallet connection error, we move to ERROR state.
```

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
6. https://ethereum.stackexchange.com/questions/92095/web3-current-best-practice-to-
   connect-metamask-to-chrome
7. https://stackoverflow.com/questions/56663785/invalid-hook-call-hooks-can-only-be-c
   alled-inside-of-the-body-of-a-function-com
