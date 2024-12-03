using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Withdraw : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/payout_request";

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
public class WithdrawRoot
{
    public bool success;
    public string message;
    public List<object> payoutdata;
    public object error;
}