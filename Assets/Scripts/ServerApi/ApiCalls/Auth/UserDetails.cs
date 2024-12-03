using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UserDetails : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/userdetails";

        www = UnityWebRequest.Get(url);
        AddHeaderAuthorization();
        //www = UnityWebRequest.Post(url, form);
        //www.SetRequestHeader("Authorizations", "barear " + DataManager.Instance.LoginData.token);
        
    }

    public override void ResponseCallback()
    {

    }

    public override void EndCallback()
    {

    }
}







[System.Serializable]
public class UserDetailsDataRoot
{
        public bool success ;
        public string message ;
        public Userdata userdata ;
        public object error ;
    }
[System.Serializable]
public class Userdata
{
    public int id;
    public string name;
    public string email;
    public string username;
    public string auth_key;
    public int role;
    public string status;
    public string avatar;
    public string profile_avatar;
    public DateTime created_at;
    public int is_online;
    public int wallet;
    public List<string> paypal_ids;
}
