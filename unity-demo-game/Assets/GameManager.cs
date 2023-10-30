using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatrixSDK;
using TMPro;

public class GameManager : MonoBehaviour
{

    private MatrixUnityToolkit mw;
    private BaseWallet walletInstance;
    public GameObject selectNetworkMenu;
    [SerializeField] TextMeshProUGUI connectedText;
    // Start is called before the first frame update
    void Start()
    {
        // Init with a config of active blockchains
        mw = MatrixUnityToolkit.Instance;
        //mw.SetConfig(new WalletName[] { WalletName.PeraWallet, WalletName.MyAlgoConnect }, Blockchain.Algorand, "4160"); // chainIDs: 416001, 416002, 416003 , 4160
        //mw.Connect(this.WalletConnected);
        //mw.Connect(WalletName.Metamask, this.WalletConnected);
    }

    public void WalletConnected(BaseWallet walletInstance)
    {
        Debug.Log("Inside WalletConnected");
        string userAddress = walletInstance.GetAddress();
        Debug.Log("userAddress: " + userAddress);
        this.walletInstance = walletInstance;
        if (userAddress.Length >= 6)
        {
            string firstThree = userAddress.Substring(0, 3);
            string lastThree = userAddress.Substring(userAddress.Length - 3, 3);

            connectedText.text = firstThree + "..." + lastThree;
        }
      
    }

    public void InitConnect()
    {
        this.selectNetworkMenu.SetActive(true);
    }

    public void ConnectToEthereum()
    {
        Debug.Log("Trying to connect to ethereum");
        mw.SetConfig(new WalletName[] { WalletName.Metamask}, Blockchain.Ethereum, "1"); // chainIDs: 416001, 416002, 416003 , 4160
        mw.Connect(this.WalletConnected);
        this.selectNetworkMenu.SetActive(false);
    }

    public void ConnectToFlow()
    {
        Debug.Log("Trying to connect to flow");
        mw.SetConfig(new WalletName[] { WalletName.Blocto }, Blockchain.Flow, "mainnet"); // chainIDs: 416001, 416002, 416003 , 4160
        mw.Connect(this.WalletConnected);
        this.selectNetworkMenu.SetActive(false);
    }

    public void ConnectToAlgorand()
    {
        Debug.Log("Trying to connect to algorand");
        mw.SetConfig(new WalletName[] { WalletName.PeraWallet, WalletName.MyAlgoConnect }, Blockchain.Algorand, "416002"); // chainIDs: 416001, 416002, 416003 , 4160
        mw.Connect(this.WalletConnected);
        this.selectNetworkMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetBalance()
    {
        //this.walletInstance.SignTransaction();
        this.walletInstance.GetBalance("balance request", null, null, false);
    }

    public void SignMessage()
    {
        this.walletInstance.SignMessage("test message",null,null,false);
    }

    public void SignTransaction()
    {
        this.walletInstance.SignTransaction("test transaction data", null, null, false);
        //this.walletInstance.SignTransaction();
    }

    public void SendTransaction()
    {
        this.walletInstance.SendTransaction("test transaction data", null, null, false);
        //this.walletInstance.SignTransaction();
    }
}
