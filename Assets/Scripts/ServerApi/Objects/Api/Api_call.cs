using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Api_Call : MonoBehaviour
{

    //api call details
    public string url;
    public JSONObject status_text = new JSONObject();

    protected System.Action<Api_Call_Params, JSONObject> _callback;

    public Api_Call_Params callParams;

    //
    public UnityWebRequest www;
    // WWWForm _form;
    /*public WWWForm form
    {
        get
        {
            if (_form == null)
            {
                _form = new WWWForm();                
            }

            return _form;
        }
    }*/



   /* Hashtable _headers;
    public Hashtable headers
    {
        get
        {
            if (_headers == null)
            {
                _headers = new Hashtable();
                //_headers.Add("Content-Type", "application/json");
                _headers.Add("Authorization", "Bearer " + DataManager.Instance.LoginData.logindata.token);
            }

            return _headers;
        }
    }*/
    //
    public void AddHeaderAuthorization()
    {
        www.SetRequestHeader("Authorization", "Bearer " + DataManager.Instance.GetAuthKey());
        www.SetRequestHeader("Accept", "application/json");

    }

    public IEnumerator Initialize(WWWForm data, System.Action<Api_Call_Params, JSONObject> callback)
    {
        status_text.Clear();
        //_headers = null;

        _callback = callback;

        //we call the inheriting class's Call method. this is set in other classes not this one
        Call(data);
        yield return www.SendWebRequest();

        status_text = new JSONObject(www.downloadHandler.text);
        status_text.AddField("error", new JSONObject(www.error));
        ResponseCallback();

        //after the call has returned a message from the server we destroy the object. some classes will redefine this "Die" method
        Die();
    }

    public virtual void Die()
    {

        //if this call should call another function when it's finished it should do so now
        if (_callback != null)
            _callback(callParams, status_text);

        DestroySelf();

    }

    public void DestroySelf()
    {
        //the end callback function can be used in classes that inherit this one to set custom event logic when this call is done loading. you will set whatever you want to happen
        //at the end of each callback in the api call class that will inherit this one. you will use "public override void EndCallback() { code }".
        EndCallback();

        //we remove the call from the stack
        //Server.apiRequest.DestroyCall(this);

        //we remove the object for good
        GameObject.Destroy(gameObject);
    }

    public abstract void ResponseCallback();
    public abstract void EndCallback();

    public abstract void Call(WWWForm data);
}