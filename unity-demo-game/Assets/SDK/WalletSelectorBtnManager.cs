using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MatrixSDK
{
    public class WalletSelectorBtnManager : MonoBehaviour
    {
        public WalletName walletName;
        private MatrixUnityToolkit mw;
        // Start is called before the first frame update
        void Start()
        {
            mw = MatrixUnityToolkit.Instance;
        }


        public void Connect()
        {
            Debug.Log("Connecting Wallet: " + walletName.ToString());
            mw.ConnectWallet(this.walletName);
        }
    }
}