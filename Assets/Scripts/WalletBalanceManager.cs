using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WalletBalanceManager : MonoBehaviour
{
    public static int walletBalance; // Variable to store the wallet balance
    public static WalletBalanceManager instance;
    public GameObject checkingPanel;
    public string playerPrefWallet;
    void Start()
    {

        instance = this;
        // Call a function to load the wallet balance from the JSON file
        //LoadWalletBalance();
    }

    public int LoadWalletBalance()
    {
        // Specify the path to the JSON file
        print("User ID: " + DataManager.Instance.GetUserId());
        return PlayerPrefs.GetInt("wallet_balance" + DataManager.Instance.GetUserId());
    }

    public void SaveWalletBalance(int newBalance)
    {
        print("Balance updation here 1");

        // Calculate the updated balance by adding the new balance to the previous balance
        PlayerPrefs.SetInt("wallet_balance" + DataManager.Instance.GetUserId(), newBalance + PlayerPrefs.GetInt("wallet_balance" + DataManager.Instance.GetUserId()));
        print("The Balance set is: again userID" + DataManager.Instance.GetUserId() + "  " + PlayerPrefs.GetInt("wallet_balance" + DataManager.Instance.GetUserId()));

    }
    public void SaveWalletBalancAtOnce(int newBalance)
    {
        print("Balance updation here 1");

        // Calculate the updated balance by adding the new balance to the previous balance
        PlayerPrefs.SetInt("wallet_balance" + DataManager.Instance.GetUserId(), newBalance);
        print("The Balance set is: again userID at once" + DataManager.Instance.GetUserId() + "  " + PlayerPrefs.GetInt("wallet_balance" + DataManager.Instance.GetUserId()));

    }

}

[System.Serializable]
public class WalletBalanceData
{
    public GameObject panel;
    public int balance;
}
