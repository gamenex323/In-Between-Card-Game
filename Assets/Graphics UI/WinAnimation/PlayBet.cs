using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayBet : Api_Call
{

    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/play_deal";

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
public class PlayBetRoot
{
    public bool success;
    public string message;
    public DealPlayData dealPlayData;
    public object error;
}
[System.Serializable]
public class DealPlayData
{
    public int pot_amount;
    public int user_balance;
    public string deal_play_id;
    public string user_id;
}


