using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MatrixSDK
{
    public class WalletManager
    {
        public Dictionary<WalletName, BaseWallet> wallets;

        public WalletManager()
        {
            wallets = new Dictionary<WalletName, BaseWallet>();
        }

        public void InitializeWallets(WalletName[] walletNames)
        {
            // Initialize and add all wallet instances to the dictionary
            foreach (WalletName item in walletNames)
            {
                switch (item)
                {
                    case WalletName.PeraWallet:
                        {
                            // your code 
                            // for MULTIPLY operator
                            wallets.Add(item, new PeraWallet(item));
                            break;
                        }
                    case WalletName.MyAlgoConnect:
                        {
                            // your code 
                            // for MULTIPLY operator
                            wallets.Add(item, new MyAlgoConnect(item));
                            break;
                        }
                    case WalletName.Metamask:
                        {
                            // your code 
                            // for MULTIPLY operator
                            wallets.Add(item, new Metamask(item));
                            break;
                        }
                    default: break;

                }

            }
            Debug.LogError("Initialized Wallets");

            // Add other wallet instances as needed
        }

        public BaseWallet GetWalletByWalletName(WalletName walletName)
        {
            if (wallets.ContainsKey(walletName))
            {
                return wallets[walletName];
            }
            else
            {
                throw new Exception("Wallet not found");
            }
        }
    }
}