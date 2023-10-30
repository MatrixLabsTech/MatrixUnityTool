using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatrixSDK
{

    [Serializable]
    public class WalletConnectionRes
    {
        public string address;
    }


    public class BaseWallet
    {
        private MatrixUnityToolkit mw;
        public WalletName Name { get; set; }
        public string UserAddress { get; set; }

        public BaseWallet()
        {
            mw = MatrixUnityToolkit.Instance;
        }

        public virtual void SetAddress(string reqData)
        {
            try
            {
                Debug.Log("reqData: " + reqData);
                WalletConnectionRes res = JsonUtility.FromJson<WalletConnectionRes>(reqData);
                Debug.Log("res.address: " + res.address);
                this.UserAddress = res.address;
            } catch(Exception e) {
                Debug.LogError(e);
            }
        }

        public virtual void Init()
        {
            // Initialization code
        }

        public virtual void Connect()
        {
            // Connection code
        }

        public virtual void Disconnect()
        {
            // Disconnection code
        }

        public virtual string GetAddress()
        {
            // Get address code
            return UserAddress;
        }

        public void SignMessage(string message, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 10)
        {
            mw.CallSignMessage(message, OnSuccess, OnError, enableTimeout, timeoutSeconds);
        }

        public void SignTransaction(string reqData, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 10)
        {
            mw.CallSignTransaction(reqData, OnSuccess, OnError, enableTimeout, timeoutSeconds);
        }

        public void SendTransaction(string reqData, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 10)
        {
            mw.CallSendTransaction(reqData, OnSuccess, OnError, enableTimeout, timeoutSeconds);
        }

        public virtual void GetBalance(string reqData, Action<string> OnSuccess, Action<string> OnError, bool enableTimeout, int timeoutSeconds = 10)
        {
            mw.CallGetBalance(reqData, OnSuccess, OnError, enableTimeout, timeoutSeconds);
        }
    }
}