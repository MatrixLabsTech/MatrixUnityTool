using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatrixSDK
{

    public class MyAlgoConnect : BaseWallet
    {
        // Singleton instance of the PeraWallet class
        private static MyAlgoConnect _instance;
        private MatrixUnityToolkit mw;

        public static MyAlgoConnect Instance
        {
            get
            {
                // If the instance does not exist, create it
                if (_instance == null)
                {
                    _instance = new MyAlgoConnect();
                }

                return _instance;
            }
        }

        private MyAlgoConnect()
        {
            // Private constructor to prevent external instantiation
            Name = WalletName.PeraWallet;
            mw = MatrixUnityToolkit.Instance;
        }

        public MyAlgoConnect(WalletName peraWallet)
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

    }
}