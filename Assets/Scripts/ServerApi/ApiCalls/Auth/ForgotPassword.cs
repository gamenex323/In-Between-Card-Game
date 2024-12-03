using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ForgotPassword : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/forget-password";       

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
public class ForgotPasswordDataRoot
{
    public bool success;
    public string message;
    public string forgetdata;
    public object error;
}

