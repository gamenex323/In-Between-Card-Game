using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserFriendReq : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/add-friend";


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
public class FriendReqData
{
    public bool success;
    public string message;
    public Requestfriend requestfriend;
}

[System.Serializable]
public class Requestfriend
{
    public int user_id;
    public int friend_id;
    public string status;
    public int id; 
}
