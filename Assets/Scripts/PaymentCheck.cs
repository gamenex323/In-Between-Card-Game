using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PaymentCheck : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/check_payment_status";

        www = UnityWebRequest.Post(url, data);
        AddHeaderAuthorization();

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
public class PaymentCheckRoot
{
    public bool success;
    public string message;
    public PaymentCheckData paymentData;
    public object error;
}
[System.Serializable]
public class PaymentCheckData
{
    public string encrypted_string;
    public int payment_status = 1;
}


