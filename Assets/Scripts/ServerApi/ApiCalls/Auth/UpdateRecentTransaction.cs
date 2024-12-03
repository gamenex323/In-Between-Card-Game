using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateRecentTransaction : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/update-recent-transaction";

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
