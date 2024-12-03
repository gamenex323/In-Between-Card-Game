using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Wallet : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/payment_init";

        www = UnityWebRequest.Post(url, data);
        AddHeaderAuthorization();

        //www = UnityWebRequest.Post(url, form);
        //www.SetRequestHeader("Authorization", "barear " + DataManager.Instance.LoginData.token);

    }

    public override void ResponseCallback()
    {

    }

    public override void EndCallback()
    {

    }
}

[System.Serializable]
public class Paymentdata
{
    public string payment_url;
}
[System.Serializable]
public class WalletRoot
{
    public bool success;
    public string message;
    public WalletData paymentdata;
    public object error;
}
[System.Serializable]
public class WalletData
{
    public string token;
    public string token_type;
    public string payment_url;
    public string encrypt_string;
}



