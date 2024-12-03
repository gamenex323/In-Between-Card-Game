using System;
using UnityEngine;

public class Server : ExtendedBehavior
{
    [SerializeField]
    public static Action LoadingStart;

    public static Action LoadingEnd;
    public static Action OnUserDetailAdded;

    public FriendListData friendList;
    //public static Api_Data apiData = new Api_Data(); //this should be saved and loaded with playerprefs
    private int[] balancePurchase;
    private static Server _Instance;
    public static Server Instance
    { get { return _Instance; } }

    protected static bool isLoading = false;
    //--------------------------------------------------------------------------------------------------------
    private void Start()
    {
        balancePurchase = new int[] { 50, 70, 100 };
        Invoke(nameof(UpdateBalanceApp), 3f);
    }

    public void UpdateBalancePurchase(int i)
    {

        print("Balance updation here");
        WalletBalanceManager.instance.SaveWalletBalance(balancePurchase[i]);
        DataManager.Instance.UserDetailsData.userdata.wallet = WalletBalanceManager.instance.LoadWalletBalance();
        WalletManager.instance.SetWalletBalance(DataManager.Instance.UserDetailsData.userdata.wallet);
        UIController.instance.displayMoney.text = WalletBalanceManager.instance.LoadWalletBalance().ToString();

    }
    public void UpdateBalance(int i)
    {

        print("Balance updation here");
        WalletBalanceManager.instance.SaveWalletBalancAtOnce(i);
        DataManager.Instance.UserDetailsData.userdata.wallet = WalletBalanceManager.instance.LoadWalletBalance();
        WalletManager.instance.SetWalletBalance(DataManager.Instance.UserDetailsData.userdata.wallet);
        UIController.instance.displayMoney.text = WalletBalanceManager.instance.LoadWalletBalance().ToString();

    }
    public void UpdateBalanceLocally()
    {
        WalletManager.instance.SetWalletBalance(WalletBalanceManager.instance.LoadWalletBalance());
        DataManager.Instance.UserDetailsData.userdata.wallet = WalletBalanceManager.instance.LoadWalletBalance();

    }

    public void UpdateBalanceApp()
    {

        DataManager.Instance.UserDetailsData.userdata.wallet = WalletBalanceManager.instance.LoadWalletBalance();
        WalletManager.instance.SetWalletBalance(DataManager.Instance.UserDetailsData.userdata.wallet);

        print("Server Wallet Balance: " + WalletBalanceManager.instance.LoadWalletBalance());
    }
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    private static bool IsNetworkAvailable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // ScreenNavManager.Instance.ShowScreen("NetworkLost");
            return false;
        }

        return true;
    }

    public void SetStatus(string _str)
    {
        //logTxt.text = _str;
    }

    //--------------------------------------------------------------------------------------------------------
    private System.Type _type;

    private Action<JSONObject> _callback;
    private WWWForm _data;
    private bool _isShowLoading;

    public void ApiRequest(System.Type type, System.Action<JSONObject> callback = null, WWWForm data = null, bool isShowLoading = true)
    {
        _type = type;
        _callback = callback;
        _data = data;
        _isShowLoading = isShowLoading;

        if (isShowLoading)
        {
            _LoadingStart();
        }

        GameObject go = new GameObject();
        if (type == null)
        {
            return;
        }
        //add the apiCall component to the game object and remember the info we need to initialize the call class
        Api_Call call = null;

        //call =
        go.AddComponent(type);
        if (go.GetComponent<Api_Call>())
        {
            call = go.GetComponent<Api_Call>();
        }

        Api_Call_Params callParams = new Api_Call_Params(call, callback, data, type);
        //calls.Add(callParams);
        call.callParams = callParams;
        InitiateCall(call, data);
        MyDebug.Log("ApiRequest " + type);
    }

    //Api_Call _currentCall;
    private void InitiateCall(Api_Call call, WWWForm data)
    {
        Api_Call _currentCall = call;
        StartCoroutine(_currentCall.Initialize(data, ApiCallback));
    }

    private void ApiCallback(Api_Call_Params callParams, JSONObject data)
    {
        //MyDebug.Log("API CALL:" + _currentCall.callParams.type);
        MyDebug.Log("API RESPONSE: " + data);
        MyDebug.Log("___________________");
        //we check for errors first
        if (data.HasField("message"))
        {
            MyDebug.Log("Refreshing");
            //we check if we need to refresh the token
            if (data.GetField("message").str.Contains("not authrized"))
            {
                // OnUserDetailUpdateCall();
                PhotonLobbyManager.instance.ReLoginGame();
                //we don't propagate the callback anymore because the api call did not finish, it need to refesh the token first
                MyDebug.Log("Refreshing");

                return;
            }
        }
        else
        {
            // 
        }

        //we call the current call's callback
        callParams.callback(data);
        if (_isShowLoading)
        {
            _LoadingEnd();
        }
    }

    public static void Retry()
    {
        //ApiRequest(_type, _callback, _data, _isShowLoading);
    }

    //EVENT LISTENERS******************************************************************************
    //LOGIN----------------------------------------------------------------------------------------
    internal string username;

    private string password;

    public void CallLogIn(string un, string pass)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", un);
        formData.AddField("password", pass);
        //if (!IsNetworkAvailable()) return;
        Debug.Log("CallLogIn.....");
        username = un;
        password = pass;

        ApiRequest(typeof(Login), OnLoginCompleted, formData, false);
        SetStatus("Call login");
        _LoadingStart();
    }


    void FirstCome()
    {
        if (PlayerPrefs.GetInt("firstCome") == 0)
        {
            print("first come");
            //PlayerPrefs.SetInt("" + email_input.text, 10);
            PlayerPrefs.SetInt("firstCome", 1);
            PlayerPrefs.SetInt("totalUsers", PlayerPrefs.GetInt("totalUsers") + 1);
            Server.Instance.UpdateBalance(PlayerPrefs.GetInt("User" + DataManager.Instance.GetUserId() + "_balance_"));
            if ((PlayerPrefs.GetInt("User" + DataManager.Instance.GetUserId() + "_balance_")) == 0)
            {
                Server.Instance.UpdateBalance(100);
            }
            //////if (DataManager.Instance.GetUserIdInt() <= PlayerPrefs.GetInt("totalUsers"))
            //////{
            //////    Server.Instance.UpdateBalance(PlayerPrefs.GetInt("User" + DataManager.Instance.GetUserId() + "_balance_"));
            //////}
            //////else
            //////{
            //////    Server.Instance.UpdateBalancePurchase(2);

            //////}
        }

        UpdateBalanceApp();
        //Server.Instance.UpdateBalanceLocally();

    }

    private void OnLoginCompleted(JSONObject data)
    {
        //loginData = JsonUtility.FromJson<Api_Login>(status.Print());
        //data from api

        Invoke(nameof(FirstCome), 10f);
        LoginDataRoot loginData = JsonUtility.FromJson<LoginDataRoot>(data.Print());

        Debug.Log("OnLoginCompleted");
        //DataManager.Instance.LoginData = loginData;
        //data from api
        SetStatus("Login Completed");
        if (loginData.success)
        {
            Debug.Log("success");
            DataManager.Instance.SetAuthKey(loginData.logindata.token);
            CallGetUserDetail();
            //CallFindFriend("moogli@gmail.com");
        }
        else
        {
            MessageBox.Instance.Show(loginData.message, MsgType.OKONLY);
            PlayerPrefs.DeleteAll();
            GameEvents.EventScreenshow?.Invoke(ScreenType.Login);
            _LoadingEnd();
            //
        }
    }

    //Get User Details***************************************************************************

    public void CallGetUserDetail()
    {
        _LoadingStart();
        Debug.Log("CallGetUserDetail");
        SetStatus("Call Get User Detail");
        ApiRequest(typeof(UserDetails), OnUserDetailCompleted, null, false);
    }





    private void OnUserDetailCompleted(JSONObject data)
    {
        Debug.Log("OnUserDetailCompleted");
        SetStatus("User Detail recived");
        UserDetailsDataRoot userDetailData = JsonUtility.FromJson<UserDetailsDataRoot>(data.Print());

        //data from api

        if (userDetailData.success)
        {
            DataManager.Instance.UserDetailsData = userDetailData;
            //_LoadingStart();
            Debug.Log(userDetailData);
            TemplateManager.Instance.SetTemplate();

            TableSetting();
            //DataManager.Instance.UserDetailsData.userdata.wallet = 10;
            //WalletManager.instance.SetWalletBalance(DataManager.Instance.UserDetailsData.userdata.wallet);
            //by zohaib

            //WalletBalanceManager.SaveWalletBalance(100);
            //WalletManager.instance.AddWinngAmount(100);
        }
        else
        {
            _LoadingEnd();
            PhotonLobbyManager.instance.ReLoginGame();
            //MessageBox.Instance.Show(userDetailData.message);
            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }

    //Get User Details without actions***************************************************************************

    public void OnUserDetailUpdateCall()
    {
        Debug.Log("CallGetUserDetail");
        SetStatus("Call Get User Detail");
        ApiRequest(typeof(UserDetails), OnUserDetailUpdateCalled, null, false);
    }

    private void OnUserDetailUpdateCalled(JSONObject data)
    {
        Debug.Log("OnUserDetailCompleted");
        SetStatus("User Detail recived");
        UserDetailsDataRoot userDetailData = JsonUtility.FromJson<UserDetailsDataRoot>(data.Print());

        //data from api

        if (userDetailData.success)
        {
            DataManager.Instance.UserDetailsData = userDetailData;
            //_LoadingStart();
            Debug.Log(userDetailData);
            print("The balance is: " + DataManager.Instance.UserDetailsData.userdata.wallet);

            WalletManager.instance.SetWalletBalance(DataManager.Instance.UserDetailsData.userdata.wallet);

        }
        else
        {
            _LoadingEnd();
            //PhotonLobbyManager.instance.LogOutGame();
            // MessageBox.Instance.Show(userDetailData.message);
            PhotonLobbyManager.instance.ReLoginGame();

            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }

    //-------------------------table setting API---------------------------------------------------------
    public void TableSetting()
    {
        Debug.Log("table settings api");

        ApiRequest(typeof(TableSetting), TableSettingUpdated, null, false);
    }

    private void TableSettingUpdated(JSONObject data)
    {
        TableSettingRoot tableSetting = JsonUtility.FromJson<TableSettingRoot>(data.Print());
        DataManager.Instance.TableSettingData = tableSetting;

        if (tableSetting.success)
        {
            Debug.Log("table settings set");
            GameSetting.Instance.MinimumBetAmount = tableSetting.tableData[0].boot_amount;
            PhotonLobbyManager.instance.ConnectToPhoton();
        }
        else
        {
            MessageBox.Instance.Show(tableSetting.message);
        }
    }

    //****************************************************************************************************
    //Find Friend
    //public void CallFindFriend(string s)
    //{
    //    WWWForm formData = new WWWForm();
    //    formData.AddField("username", s);
    //    Debug.Log("CallGetUserDetail");
    //    //SetStatus("Call Get User Detail");

    //    ApiRequest(typeof(SearchFriends), OnCallFindFriend, formData, false);

    //}

    //void OnCallFindFriend(JSONObject data)
    //{
    //    Debug.Log("OnCallFindFriend");
    //    // SetStatus("User Detail recived");
    //    FindFriends findfriends = JsonUtility.FromJson<FindFriends>(data.Print());

    //    DataManager.Instance.FindFriendsdata = findfriends;
    //    //data from api

    //    if (findfriends.success)
    //    {
    //        //_LoadingStart();
    //        Debug.Log(findfriends);
    //        FindObjectOfType<AddFriends>().ShowSearchedFriend();
    //        //PhotonLobbyManager.instance.ConnectToPhoton();

    //    }
    //    else
    //    {
    //        _LoadingEnd();
    //        MessageBox.Instance.Show(findfriends.message);
    //        //ScreenNavManager.Instance.ShowScreen("NetworkLost");
    //    }

    //}

    ////*********************************************************************************************************************
    ////FriendList

    //public void CallUserFriendList()
    //{
    //    Debug.Log("CallUserFriendList");
    //    ApiRequest(typeof(UserFriendList), OnCallUserFriendList);
    //}

    //void OnCallUserFriendList(JSONObject data)
    //{
    //    Debug.Log("OnCallUserFriendList");
    //    // SetStatus("User Detail recived");
    //    //FriendsList.instance.ShowFriendListData();

    //    friendList = JsonUtility.FromJson<FriendListData>(data.Print());

    //    //DataManager.Instance.FriendListdata = friendList;
    //    ////data from api

    //    if (friendList.success)
    //    {
    //        //_LoadingStart();
    //        Debug.Log(friendList);
    //        FindObjectOfType<FriendsList>().ShowFriendListData();
    //        //PhotonLobbyManager.instance.ConnectToPhoton();

    //    }
    //    else
    //    {
    //        _LoadingEnd();
    //        MessageBox.Instance.Show(friendList.message);
    //        //ScreenNavManager.Instance.ShowScreen("NetworkLost");
    //    }
    //}
    ////*********************************************************************************************************************
    ////User Friend Req API
    //public void CallUserFriendReq(int i)
    //{
    //    Debug.Log("CallUserFriendReq");
    //    WWWForm formData = new WWWForm();
    //    formData.AddField(" friend_user_id", i);
    //    ApiRequest(typeof(UserFriendReq), OnCallUserFriendReq,formData, false);
    //}

    //void OnCallUserFriendReq(JSONObject data)
    //{
    //    Debug.Log("OnCallUserFriendList");
    //    // SetStatus("User Detail recived");
    //    //FriendsList.instance.ShowFriendListData();

    //   FriendReqData  friendReqData= JsonUtility.FromJson<FriendReqData>(data.Print());

    //    ////DataManager.Instance.FriendListdata = friendList;
    //    //////data from api

    //    if (friendReqData.success)
    //    {
    //        //_LoadingStart();
    //        Debug.Log(friendReqData);
    //        Debug.Log("Succcccccccccccccccccccccccccccccccccces");
    //        //FindObjectOfType<FriendsList>().ShowFriendListData();
    //        //PhotonLobbyManager.instance.ConnectToPhoton();

    //    }
    //    else
    //    {
    //        _LoadingEnd();
    //        MessageBox.Instance.Show(friendReqData.message);
    //        //ScreenNavManager.Instance.ShowScreen("NetworkLost");
    //    }
    //}

    ////*********************************************************************************************************************
    ////Pending User Friend Req
    //public void PendingFriendReq()
    //{
    //    Debug.Log("PendingFriendReq");
    //    ApiRequest(typeof(PendingFriendReq), OnCallPendingFriendReq);
    //}

    //void OnCallPendingFriendReq(JSONObject data)
    //{
    //    Debug.Log("OnCallUserFriendList");
    //    // SetStatus("User Detail recived");
    //    //FriendsList.instance.ShowFriendListData();

    //    //friendList = JsonUtility.FromJson<FriendListData>(data.Print());

    //    //DataManager.Instance.FriendListdata = friendList;
    //    ////data from api

    //    //if (friendList.success)
    //    //{
    //    //    //_LoadingStart();
    //    //    Debug.Log(friendList);
    //    //    FindObjectOfType<FriendsList>().ShowFriendListData();
    //    //    //PhotonLobbyManager.instance.ConnectToPhoton();

    //    //}
    //    //else
    //    //{
    //    //    _LoadingEnd();
    //    //    MessageBox.Instance.Show(friendList.message);
    //    //    //ScreenNavManager.Instance.ShowScreen("NetworkLost");
    //    //}
    //}

    //********************************************************************************************************************
    //Loding.................................
    public static void _LoadingStart()
    {
        if (LoadingStart != null)
        {
            LoadingStart();
        }
    }

    public static void _LoadingEnd()
    {
        Debug.Log("_LoadingEnd");
        if (LoadingEnd != null)
        {
            LoadingEnd();
        }
    }

    public static bool IsInternetAvailable()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return true;
        }
        return false;
    }
}