using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Api_Call_Params
{

    public Api_Call call = null;
    public System.Action<JSONObject> callback = null;
    public WWWForm data = null;
    public System.Type type;

    public Api_Call_Params(Api_Call _call, System.Action<JSONObject> _callback, WWWForm _data, System.Type _type)
    {
        call = _call;
        callback = _callback;
        data = _data;
        type = _type;
    }
}
