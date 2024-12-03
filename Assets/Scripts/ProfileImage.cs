using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ProfileImage : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/change-avatar";

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


//profile api
[System.Serializable]
public class ProfileRoot
{
    public bool success;
    public string message;
    public ProfileData profiledata;
    public object error;

}
[System.Serializable]
public class ProfileData
{
    public string token;
    public string token_type;
    public byte[] avatar;
}

