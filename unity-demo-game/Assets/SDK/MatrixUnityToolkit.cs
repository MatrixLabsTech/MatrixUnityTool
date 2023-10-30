using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace MatrixSDK
{

    public enum MatrixUnityEvent
    {
        Timeout = 0,
        Connection = 1,
        SmartContractInteraction = 2,
        SignMessage = 3,
        SendTransaction = 4,
        SignTransaction = 5
    }

    public enum Blockchain
    {
        Ethereum = 0,
        Flow = 1,
        Aptos = 2,
        Sui = 3,
        Algorand = 4
    }

    public enum WalletName
    {
        Metamask = 1,
        Blocto = 2,
        PetraWallet = 3,
        SuiWallet = 4,
        PeraWallet = 5,
        MyAlgoConnect = 6,
    }


    public class InvocationData
    {
        public MethodNames MethodName { get; set; }
        public string ReqData { get; set; }
        public bool EnableTimeout { get; set; }
        public int TimeoutSeconds { get; set; }

        public Action<string> OnSuccess;
        public Action<string> OnError;

        public InvocationData(MethodNames methodName, string reqData, bool enableTimeout, Action<string> onSuccess, Action<string> onError, int timeoutSeconds)
        {
            this.MethodName = methodName;
            this.ReqData = reqData;
            this.EnableTimeout = enableTimeout;
            this.TimeoutSeconds = timeoutSeconds;

            this.OnSuccess = onSuccess;
            this.OnError = onError;
        }
    }


    public class Event_JSSDKInteraction_Timeout
    {
        public string invocationId;
        public InvocationData invocationData;
    }

    public class Event_JSSDKInteraction_OnGetERC20TokenData
    {
        public string invocationId;
        public GetERC20TokenDataResponse infoJson;
    }

    public class Event_JSSDKInteraction_OnInvokeEthereumSmartContractMethod
    {
        public string invocationId;
        public string methodName;
        public string infoJson; // JSON String
    }

    public class Event_JSSDKInteraction_OnSignMessage
    {
        public string invocationId;
        public string blockchainType;
        public string signature; // JSON String
    }

    public class Event_JSSDKInteraction_GenericResponse
    {
        public string invocationId;
        public string methodName;
        public string infoJson; // JSON String
    }

    public class Event_JSSDKInteraction_QueryTransaction
    {
        public string invocationId;
        public string infoJson; // JSON String
    }

    public class Event_JSSDKInteraction_SendTransaction
    {
        public string invocationId;
        public string txHash; // JSON String
    }

    [Serializable]
    public class GetERC20TokenDataRequest
    {
        public string abiCode;
        public string contractAddress;
    }

    [Serializable]
    public class ConnectWalletRequest
    {
        public string walletName;
        public string blockchain;
        public string network;
    }

    [Serializable]
    public class InvokeEthereumSmartContractMethodRequest
    {
        public string abiCode;
        public string contractAddress;
        public string methodName;
        public string args; // JSON array
    }

    [Serializable]
    public class GenericRequest
    {
        public string metaData;
    }

    [Serializable]
    public class SignMessageRequest
    {
        public string message;
    }

    [Serializable]
    public class QueryTransactionRequest
    {
        public string txHash;
        public string blockchainType;
    }

    [Serializable]
    public class SendTransactionRequest
    {
        public string reqData;
        public string blockchainType;
    }

    [Serializable]
    public class SignTransactionRequest
    {
        public string reqData;
    }

    [Serializable]
    public class JSData
    {
        public GetERC20TokenDataRequest getERC20TokenDataReq;
        public ConnectWalletRequest connectWalletRequest;
        //public InvokeEthereumSmartContractMethodRequest invokeEthereumSmartContractMethodRequest;
        public SignMessageRequest signMessageRequest;
        public GenericRequest genericRequest;
        public QueryTransactionRequest queryTransactionRequest;
        public SendTransactionRequest sendTransactionRequest;
        public SignTransactionRequest signTransactionRequest;
    }

    [Serializable]
    public class GetERC20TokenDataResponse
    {
        public string name;
        public string symbol;
        public string totalSupply;
    }

    [Serializable]
    public class SignMessageResponse
    {
        public string blockchainType;
        public string signature;
    }

    [Serializable]
    public class SignTransactionResponse
    {
        public object blob;
    }

    [Serializable]
    public class SendTransactionResponse
    {
        public string txHash;
    }

    [Serializable]
    public class MatrixUnityToolkitResponse
    {
        public string invocationId;
        public string methodName;
        public string resData;
        public bool isSuccess;
    }

    [Serializable]
    public enum MethodNames
    {
        Connect,
        GetBalance,
        SignMessage,
        SignTransaction,
        QueryTransaction,
        SendTransaction
    }

    public class MatrixUnityToolkit : MonoBehaviour
    {
#if UNITY_EDITOR || ENV_TESTCLIENT
        public static void JSSDKInteraction(string invocationId, string methodName, string reqJSON)
        {
            Debug.Log("JSSDKInteraction:: InvocationId: " + invocationId + ", methodName: " + methodName + ", reqJSON: " + reqJSON);
        }
#else
         [DllImport("__Internal")] private static extern void JSSDKInteraction(string invocationId, string methodName, string reqJSON);
#endif

        public GameObject parentObj;
        public GameObject blockchainSelectorUI;
        public GameObject networkSelectorUI;

        public GameObject connectingWalletPopup;
        public GameObject shadow;

        public GameObject walletSelectorContent;
        public GameObject successMessagePrefab;
        public GameObject errorMessagePrefab;

        public GameObject walletSelectorBtnPrefab;

        public string activeBlockchain;
        public string selectedNetwork;

        private WalletManager walletManager;


        private Action<BaseWallet> connectionCallback;


        [SerializeField] private MatrixUnityEventDispatcher eventDispatcher;

        public static MatrixUnityToolkit Instance { get; private set; }
        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            this.parentObj = this.gameObject;
            this.walletManager = new WalletManager();
        }

        public void ToggleUI(bool show)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(show);
            }
        }

        Dictionary<string, InvocationData> pendingInvocations = new Dictionary<string, InvocationData>();
        private WalletName activeWallet;
        private BaseWallet walletInstance;


        private async Task<string> InvokeJSMethodAsync(MethodNames methodName, JSData args, Action<string> onSuccess, Action<string> onError, bool enableTimeout, int timeoutSeconds)
        {
            string invocationId = System.Guid.NewGuid().ToString();
            string jsonArgs = JsonUtility.ToJson(args);
            pendingInvocations.Add(invocationId, new InvocationData(methodName, jsonArgs, enableTimeout, onSuccess, onError, timeoutSeconds));
            switch (methodName)
            {
                case MethodNames.Connect:
                    JSSDKInteraction(invocationId, methodName.ToString(), JsonUtility.ToJson(args.connectWalletRequest));
                    break;
                case MethodNames.GetBalance:
                    JSSDKInteraction(invocationId, methodName.ToString(), JsonUtility.ToJson(args.genericRequest));
                    break;
                case MethodNames.SignMessage:
                    JSSDKInteraction(invocationId, methodName.ToString(), JsonUtility.ToJson(args.signMessageRequest));
                    break;
                case MethodNames.SignTransaction:
                    JSSDKInteraction(invocationId, methodName.ToString(), JsonUtility.ToJson(args.signTransactionRequest));
                    break;
                case MethodNames.SendTransaction:
                    JSSDKInteraction(invocationId, methodName.ToString(), JsonUtility.ToJson(args.sendTransactionRequest));
                    break;
            }

            Debug.Log("Started::: invocationId: " + invocationId);

            if (enableTimeout)
            {
                StartCoroutine(InitTimeoutHandler(invocationId, timeoutSeconds));
            }
            return invocationId; // Return the invocationId as a result of the function
        }

        IEnumerator InitTimeoutHandler(string invocationId, int timeoutSeconds)
        {
            yield return new WaitForSeconds(timeoutSeconds);
            if (pendingInvocations.ContainsKey(invocationId))
            {
                Debug.Log("Timeout for: " + invocationId);
                InvocationData invocationData = pendingInvocations[invocationId];
                this.connectingWalletPopup.SetActive(false);
                this.shadow.SetActive(false);
                //this.walletInstance = this.walletManager.GetWalletByWalletName(this.activeWallet);
                //this.connectionCallback.Invoke(this.walletInstance); // For testing
                ShowErrorMessage("Timeout error.");
                if(invocationData.OnError != null)
                {
                    invocationData.OnError("Timeout error for: " + invocationId);
                }

                var data = new
                {
                    invocationId = invocationId,
                    invocationData = invocationData
                };

                eventDispatcher.DispatchEvent(MatrixUnityEvent.Timeout, data);
                pendingInvocations.Remove(invocationId);
            }
        }

        public void MatrixUnityToolkitResponse(string jsonString)
        {
            this.connectingWalletPopup.SetActive(false);
            MatrixUnityToolkitResponse res = JsonConvert.DeserializeObject<MatrixUnityToolkitResponse>(jsonString);
            //MatrixUnityToolkitResponse res = JsonUtility.FromJson<MatrixUnityToolkitResponse>(jsonString);
            Debug.Log("Response from MatrixUnityToolkit: isSuccess? " + res.isSuccess + "invocationId: " + res.invocationId + " jsonString: " + jsonString);

            if (!pendingInvocations.ContainsKey(res.invocationId)) return;
            Debug.Log("Invocation pending");
            InvocationData iv = pendingInvocations[res.invocationId];
            if (res.isSuccess)
            {
                ShowSuccessMessage("Success! data: " + res.resData);
                if (iv.OnSuccess != null)
                {
                    iv.OnSuccess(res.resData);
                }
            }
            else
            {
                ShowErrorMessage("Error: " + res.invocationId);
                if (iv.OnError != null)
                {
                    iv.OnError(res.resData);
                }
            }
            Debug.Log("infoJSON" + res.resData);
            if (iv.MethodName == MethodNames.Connect)
            {
                Debug.Log("Inside MethodNames: Connect");
                var data = new
                {
                    methodName = res.methodName,
                    invocationId = res.invocationId,
                    infoJson = res.resData
                };
                this.walletInstance = this.walletManager.GetWalletByWalletName(this.activeWallet);
                this.walletInstance.SetAddress(res.resData);
                Debug.Log("wallet instance" + this.walletInstance.Name);
                this.connectionCallback.Invoke(this.walletInstance);
                Debug.Log("Invoked");
                eventDispatcher.DispatchEvent(MatrixUnityEvent.Connection, data);
            }
            else if (iv.MethodName == MethodNames.SignMessage)
            {
                SignMessageResponse signData = JsonUtility.FromJson<SignMessageResponse>(res.resData);

                var data = new
                {
                    invocationId = res.invocationId,
                    blockchainType = signData.blockchainType,
                    signature = signData.signature
                };

                eventDispatcher.DispatchEvent(MatrixUnityEvent.SignMessage, data);
            }
            else if (iv.MethodName == MethodNames.SignTransaction)
            {
                SignTransactionResponse signTxRes = JsonUtility.FromJson<SignTransactionResponse>(res.resData);

                var data = new
                {
                    invocationId = res.invocationId,
                    blob = signTxRes.blob
                };

                eventDispatcher.DispatchEvent(MatrixUnityEvent.SignTransaction, data);

            }
            else if (iv.MethodName == MethodNames.SendTransaction)
            {
                SendTransactionResponse sendTxRes = JsonUtility.FromJson<SendTransactionResponse>(res.resData);

                var data = new
                {
                    invocationId = res.invocationId,
                    txHash = sendTxRes.txHash
                };

                eventDispatcher.DispatchEvent(MatrixUnityEvent.SendTransaction, data);

            }
            pendingInvocations.Remove(res.invocationId);
        }

        public void ConnectWallet(WalletName walletName)
        {
            Debug.Log("ConnectWallet: "+ walletName);
            this.connectingWalletPopup.SetActive(true);
            this.HideMultichainConnectUI();
            
            _ = InvokeJSMethodAsync(MethodNames.Connect, new JSData()
            {
                connectWalletRequest = new ConnectWalletRequest()
                {
                    blockchain = this.activeBlockchain,
                    network = this.selectedNetwork,
                    walletName = walletName.ToString()
                }
            }, null, null, true, 30);
            this.activeWallet = walletName;
        }

        //public void CallInvokeEthereumSmartContractMethod(string abiCode, string contractAddress, string methodName, string args, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 10)
        //{
        //    Debug.Log("CallInvokeEthereumSmartContractMethod");
        //    _ = InvokeJSMethodAsync(MethodNames.InvokeEthereumSmartContractMethod, new JSData()
        //    {
        //        invokeEthereumSmartContractMethodRequest = new InvokeEthereumSmartContractMethodRequest()
        //        {
        //            abiCode = abiCode,
        //            contractAddress = contractAddress,
        //            methodName = methodName,
        //            args = args
        //        }
        //    }, OnSuccess,
        //    OnError, enableTimeout, timeoutSeconds);
        //}

        public void CallGetBalance(string metaData, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 30)
        {
            Debug.Log("CallGetBalance");
            _ = InvokeJSMethodAsync(MethodNames.GetBalance, new JSData()
            {
                genericRequest = new GenericRequest()
                {
                    metaData = metaData
                }
            },
            OnSuccess,
            OnError,
            enableTimeout, timeoutSeconds);
        }


        public void CallSignMessage(string message, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 30)
        {
            Debug.Log("CallSignMessage");
            _ = InvokeJSMethodAsync(MethodNames.SignMessage, new JSData()
            {
                signMessageRequest = new SignMessageRequest()
                {
                    message = message
                }
            },
            OnSuccess,
            OnError,
            enableTimeout, timeoutSeconds);
        }

        public void CallSignTransaction(string reqData, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 30)
        {
            Debug.Log("CallSignMessage");
            _ = InvokeJSMethodAsync(MethodNames.SignTransaction, new JSData()
            {
                signTransactionRequest = new SignTransactionRequest()
                {
                    reqData = reqData,                }
            },
            OnSuccess,
            OnError,
            enableTimeout, timeoutSeconds);
        }

        public void CallSendTransaction(string reqData, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 30)
        {
            Debug.Log("CallSendTransaction");
            _ = InvokeJSMethodAsync(MethodNames.SendTransaction, new JSData()
            {
                sendTransactionRequest = new SendTransactionRequest()
                {
                    reqData = reqData,
                    blockchainType = this.activeBlockchain
                }
            }, OnSuccess,
            OnError, enableTimeout, timeoutSeconds);
        }

        public void CallQueryTransaction(string txHash, string blockchainType, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 30)
        {
            Debug.Log("CallQueryTransaction");
            _ = InvokeJSMethodAsync(MethodNames.QueryTransaction, new JSData()
            {
                queryTransactionRequest = new QueryTransactionRequest()
                {
                    txHash = txHash,
                    blockchainType = blockchainType
                }
            }, OnSuccess,
            OnError, enableTimeout, timeoutSeconds);
        }

        public void HideMultichainConnectUI()
        {
            //this.parentObj.SetActive(false);
            this.blockchainSelectorUI.SetActive(false);
        }

        public void Connect(Action<BaseWallet> connectionCallback)
        {
            Vector3 buttonPosition = walletSelectorBtnPrefab.transform.position;
            short count = 0;

            foreach (KeyValuePair<WalletName, BaseWallet> entry in this.walletManager.wallets)
            {
                GameObject newButton = Instantiate(this.walletSelectorBtnPrefab, this.walletSelectorContent.transform);
                newButton.name = "WalletBtn:" + entry.Key;
                newButton.transform.position = buttonPosition;
                RectTransform rt = newButton.GetComponent<RectTransform>();
                // Increment the vertical position for the next button
                rt.anchoredPosition = new Vector2(5, 110 - newButton.GetComponent<RectTransform>().rect.height * count);
                Text textComponent = newButton.GetComponentInChildren<Text>();
                newButton.GetComponent<WalletSelectorBtnManager>().walletName = entry.Key;
                if (textComponent != null)
                {
                    textComponent.text = entry.Key.ToString();
                }
                count++;
            }

            this.parentObj.SetActive(true);
            this.blockchainSelectorUI.SetActive(true);
            this.connectionCallback = connectionCallback;
        }

        public void Connect(WalletName walletName, Action<BaseWallet> connectionCallback)
        {
            this.connectionCallback = connectionCallback;
            this.ConnectWallet(walletName);
        }

        public void SetConfig(WalletName[] enabledWallets, Blockchain selectedBlockchain, string selectedNetwork)
        {
            this.activeBlockchain = selectedBlockchain.ToString();
            this.selectedNetwork = selectedNetwork;
            this.walletManager.InitializeWallets(enabledWallets);
        }

        public void ShowSuccessMessage(string msg)
        {
            // Spawn the success message prefab
            GameObject successMessage = Instantiate(successMessagePrefab, transform);

            // Adjust the RectTransform of the spawned message
            RectTransform rectTransform = successMessage.GetComponent<RectTransform>();

            // Set the left, top, right, and bottom positions
            rectTransform.offsetMin = new Vector2(196, 168); // Adjust 'left' and 'bottom' values
            rectTransform.offsetMax = new Vector2(-325, -123); // Adjust 'right' and 'top' values

            successMessage.GetComponent<MessageManager>().msg = msg;
        }

        public void ShowErrorMessage(string err)
        {
            // Spawn the success message prefab
            GameObject errMsg = Instantiate(errorMessagePrefab, transform);

            // Adjust the RectTransform of the spawned message
            RectTransform rectTransform = errMsg.GetComponent<RectTransform>();

            // Set the left, top, right, and bottom positions
            rectTransform.offsetMin = new Vector2(196, 168); // Adjust 'left' and 'bottom' values
            rectTransform.offsetMax = new Vector2(-325, -123); // Adjust 'right' and 'top' values

            errMsg.GetComponent<MessageManager>().msg = err;
        }

    }

}

