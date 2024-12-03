using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DealResult : Api_Call
{

    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/deal_result";

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
public class DealResultRoot
{
    public bool success;
    public string message;
    public DealResultData tableData;
    public object error;
}
[System.Serializable]
public class DealResultData
{
    public string deal_status;
    public int user_balance;
    public string won_status;
    public string user_id;
    public string pot_amount;
    public string deal_play_id;
}
