using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserFriendList : Api_Call
{
    
    // Start is called before the first frame update
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/friend-list";


        //www = new WWW(url, data);
        www = UnityWebRequest.Get(url);
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
public class FriendListData
{
   
    public bool success;
    public string message;
    public List<Friendlist> friendlist;
    public object error;
}

[System.Serializable]
public class Friendlist
{
    public int id;
    public string name;
    public string email;
    public int is_online;
    public string profile_avatar;
    public int wallet_amount;
    public int user_id;
    public int friend_id;
    public string join_code;
}
