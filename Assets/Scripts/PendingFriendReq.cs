using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PendingFriendReq : Api_Call
{
    // Start is called before the first frame update
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/pending-friend-request";


        //www = new WWW(url, data);
        www =  UnityWebRequest.Get(url);
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
public class Pendingfriendrequest
{
    public int id;
    public int user_id;
    public string status;
    public int friend_id;
    public string name;
}

[System.Serializable]
public class PendingfriendrequestData
{
    public bool success;
    public string message;
    public List<Pendingfriendrequest> pendingfriendrequest;
    public object error;
}


//[System.Serializable]
//public class FriendListData
//{

//    public bool success;
//    public string message;
//    public List<Friendlist> friendlist;
//    public object error;
//}

//[System.Serializable]
//public class Friendlist
//{
//    public int id;
//    public string name;
//    public string email;
//    public string avatar;
//}
