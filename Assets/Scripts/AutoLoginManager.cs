using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class AutoLoginManager : MonoBehaviour
{
    [SerializeField]
   public bool AllowAutoLogin;
    private static AutoLoginManager _Instance;
    public static AutoLoginManager Instance { get { return _Instance; } }
    public int InitialBalance = 10;
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        //Invoke(nameof(FirstCome), 2f);
        if (AllowAutoLogin)
        {

           
            CheckAndLoadScreen();
        }
        else
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Login);
        }
        
    }
    void FirstCome()
    {
        if (PlayerPrefs.GetInt("firstCome") == 0)
        {
            print("first come");
            Server.Instance.UpdateBalancePurchase(2);
            PlayerPrefs.SetInt("" + Server.Instance.username, InitialBalance);
            PlayerPrefs.SetInt("firstCome", 1);
        }

        Server.Instance.UpdateBalanceLocally();
    }
    //********************************************************************************************************
    public void CheckAndLoadScreen()
    {
        //Debug.Log("IsLogedIn==" + IsLogedIn);
        Debug.Log("Login Token==" + DataManager.Instance.GetAuthKey());
        if (DataManager.Instance.GetAuthKey() !="")
        {
            MyDebug.Log("Call auto login");

            //int roleId = PlayerPrefs.GetInt("role");            
            //Server.Instance.CallLogIn(PlayerPrefs.GetString("email"), PlayerPrefs.GetString("password"));
            //ScreenNavManager.Instance.ShowHome();
            Server.Instance.CallGetUserDetail();
        }
        else
        {
            //ScreenMgr.ScreenManager.instance.Show("LoginScreen");
            // ScreenNavManager.Instance.ShowScreen("LoginScreen");
            GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

        }


    }
    //-----------------------------------------------------------------------------------

    /*public void UpdateLoginStatus(int _value, string _un="", string _up="")
    {
        PlayerPrefs.SetInt("logIn", _value);
        PlayerPrefs.SetString("email", _un);
        PlayerPrefs.SetString("password", _up);
        //PlayerPrefs.SetInt("role", (int)DataManager.Instance.UserType);
    }*/

    /*bool IsLogedIn
    {
        get{ return PlayerPrefs.GetInt("logIn") ==1?true:false; }
    }*/
}
