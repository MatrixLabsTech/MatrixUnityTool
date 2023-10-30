//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.Runtime.InteropServices;
//using UnityEngine.UI;
//using TMPro;
//using System;

//[Serializable]
//public enum MatrixState
//{
//    NOT_CONNECTED, // WALLET NOT CONNECTED
//    WALLET_CONNECTING,
//    IDLE,
//    ERROR,
//    WALLET_BALANCE_FETCHING,
//    TRANSACTION_PENDING,
//    TRANSACTION_COMPLETED,
//}
//[Serializable]
//public enum Blockhain
//{
//    None,
//    ETHEREUM,
//    FLOW,
//    APTOS,
//    SUI
//}
//[Serializable]
//public enum BlockchainNetwork
//{
//    None,
//    ETHEREUM_TESTNET,
//    ETHEREUM_MAINNET,
//    POLYGON_MAINNET,
//    FLOW_DEVNET,
//    FLOW_MAINNET,
//    APTOS_DEVNET,
//    APTOS_MAINNET,
//    BSC_MAINNET,
//    SUI_DEVNET
//}
//[Serializable]
//public enum Wallet
//{
//    None,
//    METAMASK,
//    BLOCTO,
//    PETRA,
//    SUI_WALLET

//}
//[Serializable]
//public enum UISTATE
//{
//    SELECT_WALLET,
//    WALLET_CONNECTING,
//    SELECT_NETWORK
//}

//[Serializable]
//public class Web3WalletConnectRequest
//{
//    public string walletName;
//    public string blockchain;
//    public string network;
//}

//[Serializable]
//public class Web3SignMessageRequest
//{
//    public string message;
//    public string nonce;
//}

//[Serializable]
//public class FlowArg
//{
//    public string val;
//    public string type;
//}

//[Serializable]
//public class Web3FlowQueryRequest
//{
//    public string cadenceScript;
//    public List<FlowArg> args;
//}

//[Serializable]
//public class Web3EthereumTxRequest
//{
//    public string rawTransaction;
//}

//[Serializable]
//public class Web3AptosTxRequest
//{
//    public string rawTransaction;
//}

//[Serializable]
//public class Web3SuiTxRequest
//{
//    public string rawTransaction;
//}

//[Serializable]
//public class Web3WalletBalRequest
//{
//    public string address;
//}

//[Serializable]
//public class Web3WalletConnectResponse
//{
//    public string invocationId;
//    public string address;
//}

//[Serializable]
//public class Web3WalletBalResponse
//{
//    public string invocationId;
//    public float bal;
//}

//[Serializable]
//public class Web3TransactionResponse
//{
//    public string invocationId;
//    public string txData;
//}

//[Serializable]
//public class Web3Error
//{
//    public string invocationId;
//    public string message;
//}


//[Serializable]
//public class JSData
//{
//    public Web3WalletConnectRequest walletConnectionReq;
//    public Web3FlowQueryRequest flowQueryReq;
//    public Web3EthereumTxRequest ethTxReq;
//    public Web3AptosTxRequest aptosTxReq;
//    public Web3SuiTxRequest suiTxReq;
//    public Web3WalletBalRequest walletBalReq;
//    public Web3SignMessageRequest signMessageRequest;
//}


//public class EventHandler : MonoBehaviour
//{

//    Dictionary<Wallet, Blockhain[]> walletBlockchainMapping = new Dictionary<Wallet, Blockhain[]>(){
//        {Wallet.METAMASK, new Blockhain[]{Blockhain.ETHEREUM}},
//        {Wallet.BLOCTO, new Blockhain[] {Blockhain.FLOW}},
//        {Wallet.PETRA, new Blockhain[] {Blockhain.APTOS}},
//        {Wallet.SUI_WALLET, new Blockhain[] {Blockhain.SUI}}
//    };

//    Dictionary<Blockhain, BlockchainNetwork[]> blockchainNetworkMapping = new Dictionary<Blockhain, BlockchainNetwork[]>(){
//        {Blockhain.ETHEREUM, new BlockchainNetwork[]{BlockchainNetwork.ETHEREUM_MAINNET,BlockchainNetwork.ETHEREUM_TESTNET,BlockchainNetwork.POLYGON_MAINNET}},
//        {Blockhain.FLOW, new BlockchainNetwork[] {BlockchainNetwork.FLOW_DEVNET,BlockchainNetwork.FLOW_MAINNET}},
//        {Blockhain.APTOS, new BlockchainNetwork[] {BlockchainNetwork.APTOS_DEVNET}},
//        {Blockhain.SUI, new BlockchainNetwork[] {BlockchainNetwork.SUI_DEVNET}}
//    };
//    [DllImport("__Internal")] private static extern void SendEthereumTransaction(string invocationId, string reqJSON);
//    [DllImport("__Internal")] private static extern void SendAptosTransaction(string invocationId, string reqJSON);
//    [DllImport("__Internal")] private static extern void SendSuiTransaction(string invocationId, string reqJSON);
//    [DllImport("__Internal")] private static extern void SendFlowQuery(string invocationId, string reqJSON);
//    [DllImport("__Internal")] private static extern void Web3Connect(string invocationId, string reqJSON);
//    [DllImport("__Internal")] private static extern void FetchWalletBalance(string invocationId, string accAddress);
//    [DllImport("__Internal")] private static extern void RequestChangeNetwork(string networkName);
//    [DllImport("__Internal")] private static extern void Web3SignMessage(string invocationId, string reqJSON);

//    public Button testButton;
//    public Text accountListText;
//    public static Wallet walletName;
//    public static UISTATE currentScreen = UISTATE.SELECT_WALLET;
//    public static string accountAddress;
//    public static bool menuHidden = true;

//    public GameObject selectWalletMenu;
//    public GameObject connectingWalletMessage;
//    public GameObject selectNetworkMenu;
//    public GameObject web3Menu;

//    public GameObject walletBalanceText;

//    public Sprite metamaskSprite;
//    public Sprite bloctoSprite;
//    public Sprite petraSprite;
//    public Sprite suiSprite;

//    public GameObject selectNetworkMetamask;
//    public GameObject selectNetworkBlocto;
//    public GameObject selectNetworkPetra;
//    public GameObject selectNetworkSui;
//    public GameObject connectButtonText;

//    public GameObject errorMessageGO;
//    public TextMeshProUGUI errorMessageGOText;

//    public GameObject successMessageGO;
//    public TextMeshProUGUI successMessageGOText;

//    public static MatrixState currentState = MatrixState.NOT_CONNECTED;
//    public static Blockhain currentBlockchain = Blockhain.None;
//    public static BlockchainNetwork currentNetwork = BlockchainNetwork.None;


//    List<string> pendingInvocations = new List<string>();

//    IEnumerator InvokeJSMethod(string methodName, JSData args, int timeout)
//    {
//        string invocationId = System.Guid.NewGuid().ToString();
//        pendingInvocations.Add(invocationId);
//#if UNITY_WEBGL == true && UNITY_EDITOR == false
//        if (methodName == "Web3Connect")
//        {
//            Web3Connect(invocationId, JsonUtility.ToJson(args.walletConnectionReq));
//        }
//        if (methodName == "SendFlowQuery")
//        {
//            SendFlowQuery(invocationId, JsonUtility.ToJson(args.flowQueryReq));
//        }
//        if (methodName == "SendEthereumTransaction")
//        {
//            SendEthereumTransaction(invocationId, JsonUtility.ToJson(args.ethTxReq));
//        }
//        if (methodName == "FetchWalletBalance")
//        {
//            FetchWalletBalance(invocationId, JsonUtility.ToJson(args.walletBalReq));
//        }
//        if(methodName == "Web3SignMessage")
//        {
//            Web3SignMessage(invocationId, JsonUtility.ToJson(args.signMessageRequest));
//        }
//        if(methodName == "SendAptosTransaction"){
//            SendAptosTransaction(invocationId, JsonUtility.ToJson(args.aptosTxReq));
//        }
//        if(methodName == "SendSuiTransaction"){
//            SendSuiTransaction(invocationId, JsonUtility.ToJson(args.signMessageRequest));
//        }
//#endif
//        Debug.Log("Started::: invocationId: " + invocationId);
//        yield return new WaitForSeconds(timeout);
//        // check if we got response response (failure case)
//        // if not - then do something with an error
//        if (pendingInvocations.Contains(invocationId))
//        {
//            Debug.Log("timeout for " + methodName + ", invocationId: " + invocationId);
//            pendingInvocations.Remove(invocationId);
//            // TO do - Also Remove pending invocation for success
//            // Future- maybe create a dynamic function with dynamic args which can be invoked via JS and it will contain invocationId which will contain the type of method, arg types etc
//            // This will make code cleaner and abstract
//            ErrorListener(JsonUtility.ToJson(new Web3Error() { invocationId = invocationId, message = "Timeout (" + timeout + "s)" }));
//        }

//    }

//    /*
//    SELECT_WALLET
//    WALLET_CONNECTING
//    WALLET_CONNECTION_ERROR

//    SELECT_NETWORK
//    WALLET_CONNECTING
//    WALLET_CONNECTION_ERROR
//    */

//    void Start()
//    {

//        Debug.Log("START....");
//        selectWalletMenu.SetActive(true);
//        connectingWalletMessage.SetActive(!true);
//        selectNetworkMenu.SetActive(!true);

//        web3Menu.SetActive(false);
//        errorMessageGO.SetActive(false);
//        successMessageGO.SetActive(false);
//        //PerformFlowQuery();

//    }

//    void UpdateUIState(bool hideMenu)
//    {
//        EventHandler.menuHidden = hideMenu;
//        web3Menu.SetActive(!EventHandler.menuHidden);
//        EventHandler.currentScreen = UISTATE.WALLET_CONNECTING;
//        UpdateActiveWalletLogo();
//        if (
//            EventHandler.currentState == MatrixState.IDLE ||
//            EventHandler.currentState == MatrixState.ERROR ||
//            EventHandler.currentState == MatrixState.TRANSACTION_COMPLETED
//            )
//        {
//            // Hide all menu in Idle
//            selectWalletMenu.SetActive(true);
//            connectingWalletMessage.SetActive(false);
//            selectNetworkMenu.SetActive(false);

//            EventHandler.currentScreen = UISTATE.SELECT_NETWORK;
//        }

//        if (
//            EventHandler.currentState == MatrixState.NOT_CONNECTED
//            )
//        {
//            // Show Select Wallet Menu
//            selectWalletMenu.SetActive(true);
//            connectingWalletMessage.SetActive(false);
//            selectNetworkMenu.SetActive(false);
//            EventHandler.currentScreen = UISTATE.SELECT_NETWORK;
//        }

//        if (
//            EventHandler.currentState == MatrixState.WALLET_BALANCE_FETCHING
//            )
//        {
//            // Show Connecting Menu
//            selectWalletMenu.SetActive(false);
//            connectingWalletMessage.SetActive(true);
//            selectNetworkMenu.SetActive(false);

//            EventHandler.currentScreen = UISTATE.WALLET_CONNECTING;
//        }

//        if (
//            EventHandler.currentState == MatrixState.WALLET_CONNECTING
//            )
//        {
//            // Show Connecting Menu
//            selectWalletMenu.SetActive(false);
//            connectingWalletMessage.SetActive(true);
//            selectNetworkMenu.SetActive(false);

//            EventHandler.currentScreen = UISTATE.WALLET_CONNECTING;
//        }


//        if (EventHandler.walletName != Wallet.None)
//        {

//            if (walletName == Wallet.METAMASK)
//            {
//                selectNetworkMetamask.SetActive(true);
//                selectNetworkBlocto.SetActive(!true);
//                selectNetworkPetra.SetActive(!true);
//                selectNetworkSui.SetActive(!true);
//            }
//            if (walletName == Wallet.BLOCTO)
//            {
//                selectNetworkMetamask.SetActive(!true);
//                selectNetworkBlocto.SetActive(true);
//                selectNetworkPetra.SetActive(!true);
//                selectNetworkSui.SetActive(!true);
//            }
//            if (walletName == Wallet.PETRA)
//            {
//                selectNetworkMetamask.SetActive(!true);
//                selectNetworkBlocto.SetActive(!true);
//                selectNetworkPetra.SetActive(true);
//                selectNetworkSui.SetActive(!true);
//            }
//            if (walletName == Wallet.SUI_WALLET)
//            {
//                selectNetworkMetamask.SetActive(!true);
//                selectNetworkBlocto.SetActive(!true);
//                selectNetworkPetra.SetActive(!true);
//                selectNetworkSui.SetActive(true);
//            }
//        }
//    }

//    public void ToggleDisplayMenu()
//    {
//        Debug.Log("Toggling Display Menu...");
//        //EventHandler.menuHidden = !EventHandler.menuHidden;
//        //web3Menu.SetActive(!EventHandler.menuHidden);
//        UpdateUIState(!EventHandler.menuHidden);
//    }

//    public void GoToWalletSelect()
//    {
//        Debug.Log("GoToWalletSelect..");
//        selectWalletMenu.SetActive(true);
//        connectingWalletMessage.SetActive(!true);
//        selectNetworkMenu.SetActive(!true);
//        EventHandler.currentScreen = UISTATE.SELECT_WALLET;

//    }

//    public void SelectWallet(string walletName)
//    {
//        Debug.Log("walletName: " + walletName);
//        EventHandler.walletName = (Wallet)Enum.Parse(typeof(Wallet), walletName, true);
//        EventHandler.currentBlockchain = walletBlockchainMapping[EventHandler.walletName][0];

//        if (EventHandler.currentNetwork == BlockchainNetwork.None)
//        {
//            EventHandler.currentNetwork = blockchainNetworkMapping[EventHandler.currentBlockchain][0];
//        }


//        //UpdateActiveWalletLogo();
//        UpdateUIState(false);

//        selectWalletMenu.SetActive(false);
//        connectingWalletMessage.SetActive(true);
//        selectNetworkMenu.SetActive(false);

//        TextMeshProUGUI textmeshPro = connectButtonText.transform.GetComponent<TextMeshProUGUI>();
//        textmeshPro.SetText("Reconnect");
//        //#if UNITY_WEBGL == true && UNITY_EDITOR == false
//        //                Web3Connect(EventHandler.walletName.ToString(), EventHandler.currentBlockchain.ToString(), EventHandler.currentNetwork.ToString());
//        //#endif
//        StartCoroutine(InvokeJSMethod("Web3Connect", new JSData()
//        {
//            walletConnectionReq = new Web3WalletConnectRequest()
//            {
//                blockchain = EventHandler.currentBlockchain.ToString(),
//                walletName = EventHandler.walletName.ToString(),
//                network = EventHandler.currentNetwork.ToString()

//            }
//        }, 10));

//    }

//    public void UpdateActiveWalletLogo()
//    {
//        Debug.Log("Updating active wallet logo: " + EventHandler.walletName);
//        Wallet wName = EventHandler.walletName;
//        Image img = connectingWalletMessage.transform.GetChild(0).GetComponent<Image>();

//        switch (wName)
//        {
//            case Wallet.METAMASK:
//                {
//                    img.sprite = metamaskSprite;
//                    break;
//                }
//            case Wallet.BLOCTO:
//                {
//                    img.sprite = bloctoSprite;
//                    break;
//                }
//            case Wallet.PETRA:
//                {
//                    img.sprite = petraSprite;
//                    break;
//                }
//            case Wallet.SUI_WALLET:
//                {
//                    img.sprite = suiSprite;
//                    break;
//                }
//            default: break;
//        }
//    }

//    public void ShowConnectionSuccess(string jsonString)
//    {
//        Web3WalletConnectResponse res = JsonUtility.FromJson<Web3WalletConnectResponse>(jsonString);
//        Debug.Log($"Account Address: {res.address}");
//        //EventHandler.currentScreen = "WALLET_CONNECTED";
//        pendingInvocations.Remove(res.invocationId);
//        EventHandler.accountAddress = res.address;
//        EventHandler.currentState = MatrixState.IDLE;
//        UpdateUIState(true);
//        //selectWalletMenu.SetActive(true);
//        //connectingWalletMessage.SetActive(false);
//        //selectNetworkMenu.SetActive(false);

//        //EventHandler.menuHidden = true;
//        //web3Menu.SetActive(false);

//    }

//    public void SignMessage()
//    {
//        Debug.Log("Sign Message Button Clicked");
//        StartCoroutine(InvokeJSMethod("Web3SignMessage", new JSData()
//        {
//            signMessageRequest = new Web3SignMessageRequest()
//            {
//               message = "MATRIX LABS",
//               nonce = "MATRIX NONCE"
//            }
//        }, 10));

//    }

//    public void UpdateWalletBalance(string jsonString)
//    {
//        Web3WalletBalResponse res = JsonUtility.FromJson<Web3WalletBalResponse>(jsonString);
//        Debug.Log("bal->>" + res.bal.ToString());
//        pendingInvocations.Remove(res.invocationId);
//        TextMeshProUGUI textmeshPro = walletBalanceText.transform.GetComponent<TextMeshProUGUI>();
//        textmeshPro.SetText(res.bal.ToString());
//        StartCoroutine(HandleMessageDisplay("success", res.invocationId, "Wallet balance updated"));
//    }

//    public void FetchLatestBalance()
//    {
//        //#if UNITY_WEBGL == true && UNITY_EDITOR == false
//        //        FetchWalletBalance(EventHandler.accountAddress);
//        //#endif

//        StartCoroutine(InvokeJSMethod("FetchWalletBalance", new JSData()
//        {
//            walletBalReq = new Web3WalletBalRequest()
//            {
//                address = EventHandler.accountAddress

//            }
//        }, 10));
//    }

//    public void ErrorListener(string jsonString)
//    {
//        Web3Error res = JsonUtility.FromJson<Web3Error>(jsonString);
//        Debug.Log($"Connection Error: {res.message}");
//        EventHandler.currentState = MatrixState.ERROR;
//        UpdateUIState(true);
//        // TODO - Emit Errors from script so they can be subscribed by other scripts.
//        pendingInvocations.Remove(res.invocationId);
//        StartCoroutine(HandleMessageDisplay("error", res.invocationId, res.message));
//    }

//    IEnumerator HandleMessageDisplay(string messageType, string invocationId, string msg)
//    {
//        if (messageType == "error")
//        {
//            errorMessageGO.SetActive(true);
//            errorMessageGOText.SetText(msg + " for invocation " + invocationId);
//            yield return new WaitForSeconds(5);
//            errorMessageGO.SetActive(false);
//            errorMessageGOText.SetText("");
//        }
//        if (messageType == "success")
//        {
//            successMessageGO.SetActive(true);
//            successMessageGOText.SetText("Success: " + msg);
//            yield return new WaitForSeconds(5);
//            successMessageGO.SetActive(false);
//            successMessageGOText.SetText("");
//        }
//    }



//    public void ToggleSelectNetwork()
//    {
//        Debug.Log("ToggleSelectNetwork::" + EventHandler.walletName);
//        if (EventHandler.walletName == Wallet.None) return;
//        EventHandler.currentScreen = UISTATE.SELECT_NETWORK;
//        //UpdateUIState(false);
//        web3Menu.SetActive(true);
//        selectWalletMenu.SetActive(false);
//        connectingWalletMessage.SetActive(false);
//        selectNetworkMenu.SetActive(true);
//        if (walletName == Wallet.METAMASK)
//        {
//            selectNetworkMetamask.SetActive(true);
//            selectNetworkBlocto.SetActive(!true);
//            selectNetworkPetra.SetActive(!true);
//            selectNetworkSui.SetActive(!true);
//        }
//        else if (walletName == Wallet.BLOCTO)
//        {
//            selectNetworkMetamask.SetActive(!true);
//            selectNetworkBlocto.SetActive(true);
//            selectNetworkPetra.SetActive(!true);
//            selectNetworkSui.SetActive(!true);
//        }
//        else if (walletName == Wallet.PETRA)
//        {
//            selectNetworkMetamask.SetActive(!true);
//            selectNetworkBlocto.SetActive(!true);
//            selectNetworkPetra.SetActive(true);
//            selectNetworkSui.SetActive(!true);
//        }
//        else if (walletName == Wallet.SUI_WALLET)
//        {
//            selectNetworkMetamask.SetActive(!true);
//            selectNetworkBlocto.SetActive(!true);
//            selectNetworkPetra.SetActive(!true);
//            selectNetworkSui.SetActive(true);
//        }


//    }

//    public void UserUpdatedListener()
//    {

//    }

//    public void PerformFlowQuery()
//    {
//        string cadenceQuery = @"
//          import Profile from 0xba1132bc08f82fe2

//          pub fun main(address: Address): Profile.ReadOnly? {
//            return Profile.read(address)
//          }";

//        List<FlowArg> args = new List<FlowArg>
//        {
//            new FlowArg()
//            {
//                type = "Address",
//                val = "0xba1132bc08f82fe2"

//            }
//        };

//        StartCoroutine(InvokeJSMethod("SendFlowQuery", new JSData()
//        {
//            flowQueryReq = new Web3FlowQueryRequest()
//            {
//                cadenceScript = cadenceQuery,
//                args = args

//            }
//        }, 30));


//    }

//    public void PerformEthereumTx()
//    {
//        string rawTx = "{\"from\":\"0x585B95073e8B51FcCC8FFa3415cD1a3B5618aB14\",\"to\":\"0x611E72c39419168FfF07F068E76a077588225798\",\"value\":\"0.05\",\"gasLimit\":\"10\",\"gasPrice\":\"0000000000000001\"}";
//        Web3EthereumTxRequest req = new Web3EthereumTxRequest()
//        {
//            rawTransaction = rawTx
//        };

//        StartCoroutine(InvokeJSMethod("SendEthereumTransaction", new JSData()
//        {
//            ethTxReq = req
//        }, 60));

//    }

//    public void PerformAptosTx()
//    {
//        string rawTx = "{\"arguments\":[\"0x29b2e0c47e550fe0c67d55357fd95d09c534def986a6920be54d0cc9d9dbcf6a\",\"717\"],\"function\":\"0x1::coin::transfer\",\"type\":\"entry_function_payload\",\"type_arguments\":[\"0x1::aptos_coin::AptosCoin\"]}";
//        Web3AptosTxRequest req = new Web3AptosTxRequest()
//        {
//            rawTransaction = rawTx
//        };

//        StartCoroutine(InvokeJSMethod("SendAptosTransaction", new JSData()
//        {
//            aptosTxReq = req
//        }, 60));

//    }

//    public void PerformSuiTx()
//    {
//        string rawTx = "{\"kind\":\"moveCall\",\"data\":{\"packageObjectId\":\"0x2\",\"module\":\"devnet_nft\",\"function\":\"mint\",\"typeArguments\":[],\"arguments\":[\"name\",\"capy\",\"https://cdn.britannica.com/94/194294-138-B2CF7780/overview-capybara.jpg?w=800&h=450&c=crop\"],\"gasBudget\":10000}}";
//        Web3SuiTxRequest req = new Web3SuiTxRequest()
//        {
//            rawTransaction = rawTx
//        };

//        StartCoroutine(InvokeJSMethod("SendSuiTransaction", new JSData()
//        {
//            suiTxReq = req
//        }, 60));

//    }

//    public void TransactionResponseListener(string jsonString)
//    {
//        Web3TransactionResponse res = JsonUtility.FromJson<Web3TransactionResponse>(jsonString);
//        Debug.Log("success for" + res.invocationId);
//        Debug.Log(res);
//        pendingInvocations.Remove(res.invocationId);
//        // TODO - Emit Errors from script so they can be subscribed by other scripts.
//        StartCoroutine(HandleMessageDisplay("success", res.invocationId, res.txData.Substring(0,40)));
//    }



//    public void HandleClickNetworkName(string networkName)
//    {
//        //#if UNITY_WEBGL == true && UNITY_EDITOR == false
//        //            RequestChangeNetwork(networkName);
//        //#endif

//        EventHandler.currentNetwork = (BlockchainNetwork)Enum.Parse(typeof(BlockchainNetwork), networkName); ;
//        StartCoroutine(InvokeJSMethod("Web3Connect", new JSData()
//        {
//            walletConnectionReq = new Web3WalletConnectRequest()
//            {
//                blockchain = EventHandler.currentBlockchain.ToString(),
//                walletName = EventHandler.walletName.ToString(),
//                network = EventHandler.currentNetwork.ToString()

//            }
//        }, 10));

//        EventHandler.currentState = MatrixState.WALLET_CONNECTING;
//        UpdateUIState(false);

//        //selectWalletMenu.SetActive(false);
//        //connectingWalletMessage.SetActive(true);
//        //selectNetworkMenu.SetActive(false);

//    }

//}
