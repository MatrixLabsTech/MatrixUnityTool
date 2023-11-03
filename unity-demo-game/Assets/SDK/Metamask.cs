using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MatrixSDK
{
    public class Metamask : BaseWallet
    {
        // Singleton instance of the PeraWallet class
        private static Metamask _instance;
        private MatrixUnityToolkit mw;

        public static Metamask Instance
        {
            get
            {
                // If the instance does not exist, create it
                if (_instance == null)
                {
                    _instance = new Metamask();
                }

                return _instance;
            }
        }

        private Metamask()
        {
            // Private constructor to prevent external instantiation
            Name = WalletName.PeraWallet;
            mw = MatrixUnityToolkit.Instance;
        }

        public Metamask(WalletName name)
        {
            this.Name = name;
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