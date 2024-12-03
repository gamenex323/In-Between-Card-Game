using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Login : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/login";      
      

        www = UnityWebRequest.Post(url, data);
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
public class LoginDataRoot
{
    public bool success;
    public string message;
    public Logindata logindata;
    public object error;

}
[System.Serializable]
public class Logindata
{
    public string token;
    public string token_type;
}

