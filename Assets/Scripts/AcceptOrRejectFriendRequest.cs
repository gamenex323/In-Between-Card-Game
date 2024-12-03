using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AcceptOrRejectFriendRequest : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/accept-reject-request";


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
public class AcceptOrRejectFriendRequestData
{
    public bool success;
    public string message;
    public string friendrequestaccept;
}
