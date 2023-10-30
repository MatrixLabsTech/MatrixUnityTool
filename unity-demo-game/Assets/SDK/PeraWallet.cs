using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatrixSDK
{

    public class PeraWallet : BaseWallet
    {
        // Singleton instance of the PeraWallet class
        private static PeraWallet _instance;
        private MatrixUnityToolkit mw;

        public static PeraWallet Instance
        {
            get
            {
                // If the instance does not exist, create it
                if (_instance == null)
                {
                    _instance = new PeraWallet();
                }

                return _instance;
            }
        }

        private PeraWallet()
        {
            // Private constructor to prevent external instantiation
            Name = WalletName.PeraWallet;
            mw = MatrixUnityToolkit.Instance;
        }

        public PeraWallet(WalletName peraWallet)
        {
            this.Name = peraWallet;
        }

        public override void Init()
        {
            // PeraWallet-specific initialization code
        }

        public override void Connect()
        {
            // PeraWallet-specific connection code
        }

        public void SignTransaction(string reqData, Action<string> OnSuccess, Action<string> OnError)
        {
            mw.CallSendTransaction(reqData, OnSuccess, OnError, false);
        }

    }
}