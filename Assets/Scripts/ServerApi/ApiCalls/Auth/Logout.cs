using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Logout : Api_Call
{
    public override void Call(WWWForm data = null)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/logout";

        www = UnityWebRequest.Get(url);
        AddHeaderAuthorization();
        //www = new WWW(url, form);
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