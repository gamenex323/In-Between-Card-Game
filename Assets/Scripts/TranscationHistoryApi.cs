using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TranscationHistoryApi : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/user_transaction_history";

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
public class TranscationHistoryApiRoot
{
    public bool success;
    public string message;
    public List<UserTranscationData> userData;
}
[System.Serializable]
public class UserTranscationData
{
    public int id;
    public int user_id;
    public string type;
    public string txn_status;
    public int amount;
    public int deal_table_id;
    public int transaction_id;
    public int withdraw_request_id;
    public int prev_balance;
    public int current_balance;
    public string created_at;
    public System.DateTime updated_at;
}
/*[System.Serializable]
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
}*/

