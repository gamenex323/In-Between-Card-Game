using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreateTable : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/create_deal_table";

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
public class CreateTableRoot
{
    public bool success;
    public string message;
    public TableCreated tableCreated;
    public object error;
}
[System.Serializable]
public class TableCreated
{
    public string table_id;
}

