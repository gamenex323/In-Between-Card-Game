using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TableSetting : Api_Call
{
    public override void Call(WWWForm data)
    {
        url = Server_Constants.serverApi_BaseUrl + "/api/get_table_settings";

        www = UnityWebRequest.Get(url);
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
public class TableSettingRoot
{
    public bool success;
    public string message;
    public List<TableSettingData> tableData;
    public object error;
}
[System.Serializable]
public class TableSettingData
{

    public int id;
    public string table_name;
    public int boot_amount;
    public int max_blind;
    public int chaal_limit;
    public int pot_limit;
}
