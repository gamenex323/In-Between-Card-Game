using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Register : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/signup";             
        
        //Debug.Log(data);
        www = UnityWebRequest.Post(url, data);
        //www = UnityWebRequest.Post(url, form);
        

    }

    public override void ResponseCallback()
    {
        if (!status_text.HasField("success"))
        {
            //credentials already used (account details not unique)
        }
    }

    public override void EndCallback()
    {

    }
}

public class RegisterData
{
    public bool success;
    public string message;
    public string error;
}