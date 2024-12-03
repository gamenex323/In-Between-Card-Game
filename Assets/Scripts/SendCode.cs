using UnityEngine;
using UnityEngine.Networking;

public class SendCode : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/share-join-code";

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
public class SendCodeRoot
{
    public bool success;
    public string message;
    public string requestcode;
    public object error;
}