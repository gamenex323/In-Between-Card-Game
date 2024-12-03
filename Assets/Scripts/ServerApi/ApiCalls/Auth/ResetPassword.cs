using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResetPassword : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/resetpassword";

       
        //Hashtable _headers = new Hashtable();
        //_headers.Add("Content-Type", "application/json");
        //Debug.Log("DataManager.Instance.AuthKey==" + DataManager.Instance.AuthKey);
       //_headers.Add("Authorization", "barear " + DataManager.Instance.AuthKey);
        
        www = UnityWebRequest.Post (url, data);
        AddHeaderAuthorization();
    }

    public override void ResponseCallback()
    {
        
    }

    public override void EndCallback()
    {

    }
}

[System.Serializable]
public class ResetPasswordData
{
    public bool success;
    public string msg;   
}