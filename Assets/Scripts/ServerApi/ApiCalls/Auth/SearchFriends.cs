using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SearchFriends : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/search-friend";


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
public class FindFriends
{
    public bool success;
    public string message;
    public Search search;
    
    public object error;
}

[System.Serializable]
public class Search
{
    public int id;
    public string username;
    public string email;
}



