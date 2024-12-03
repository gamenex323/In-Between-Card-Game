using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PaymentRequest : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/ccpay";       

        www = UnityWebRequest.Post(url, data);
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
public class PaymentRequestData
{
    public bool success;
    public string msg;
    public string approval_url;
    public object error;
}
