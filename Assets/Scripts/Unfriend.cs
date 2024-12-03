using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Unfriend : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/unfriend";


        //www = new WWW(url, data);
        www = UnityWebRequest.Post(url, data);
        AddHeaderAuthorization();
    }

    public override void EndCallback()
    {
    }

    public override void ResponseCallback()
    {
    }
}

[System.Serializable]
public class UnfriendData
{
    public bool success;
    public string message;
    //public string friendrequestaccept;
}
