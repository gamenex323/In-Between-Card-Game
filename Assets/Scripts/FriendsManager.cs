using UnityEngine;

public class FriendsManager : MonoBehaviour
{
    public static FriendsManager Instance;
    public AddFriends _addFriends;
    public FriendsList _friendsList, _inviteList;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update

    public void CallFindFriend(string s)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("email", s);
        Debug.Log("CallGetUserDetail");
        //SetStatus("Call Get User Detail");

        Server.Instance.ApiRequest(typeof(SearchFriends), OnCallFindFriend, formData, false);
    }

    private void OnCallFindFriend(JSONObject data)
    {
        Debug.Log("OnCallFindFriend");
        // SetStatus("User Detail recived");
        FindFriends findfriends = JsonUtility.FromJson<FindFriends>(data.Print());

        DataManager.Instance.FindFriendsdata = findfriends;
        //data from api

        if (findfriends.success)
        {
            //_LoadingStart();
            Debug.Log(findfriends);
            _addFriends.ShowSearchedFriend();
            //PhotonLobbyManager.instance.ConnectToPhoton();
        }
        else
        {
            Server._LoadingEnd();
            MessageBox.Instance.Show(findfriends.message);
            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }

    public void CallUserFriendList()
    {
        Debug.Log("CallUserFriendList");
        Server.Instance.ApiRequest(typeof(UserFriendList), OnCallUserFriendList);
    }

    private void OnCallUserFriendList(JSONObject data)
    {
        Debug.Log("OnCallUserFriendList");
        // SetStatus("User Detail recived");
        //FriendsList.instance.ShowFriendListData();
        _friendsList.clearFriendList();
        _inviteList.clearFriendList();
        Server.Instance.friendList = JsonUtility.FromJson<FriendListData>(data.Print());

        //DataManager.Instance.FriendListdata = friendList;
        ////data from api

        if (Server.Instance.friendList.success)
        {
            //_LoadingStart();
            Debug.Log(Server.Instance.friendList);
            _inviteList.msgText.gameObject.SetActive(false);
            _friendsList.msgText.gameObject.SetActive(false);
            _friendsList.ShowFriendListData();
            _inviteList.ShowFriendListData();
            //PhotonLobbyManager.instance.ConnectToPhoton();
        }
        else
        {
            Server._LoadingEnd();
            _friendsList.msgText.gameObject.SetActive(true);
            _friendsList.msgText.text = Server.Instance.friendList.message;
            _inviteList.msgText.gameObject.SetActive(true);
            _inviteList.msgText.text = Server.Instance.friendList.message;
            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }

    //*********************************************************************************************************************
    //User Friend Req API
    public void CallUserFriendReq(int i)
    {
        Debug.Log("CallUserFriendReq");
        WWWForm formData = new WWWForm();
        formData.AddField(" friend_user_id", i);
        Server.Instance.ApiRequest(typeof(UserFriendReq), OnCallUserFriendReq, formData, false);
    }

    private void OnCallUserFriendReq(JSONObject data)
    {
        Debug.Log("OnCallUserFriendList");
        // SetStatus("User Detail recived");
        //FriendsList.instance.ShowFriendListData();

        FriendReqData friendReqData = JsonUtility.FromJson<FriendReqData>(data.Print());

        ////DataManager.Instance.FriendListdata = friendList;
        //////data from api

        if (friendReqData.success)
        {
            //_LoadingStart();
            Debug.Log(friendReqData);
            Debug.Log("Succcccccccccccccccccccccccccccccccccces");
            //FindObjectOfType<FriendsList>().ShowFriendListData();
            //PhotonLobbyManager.instance.ConnectToPhoton();
        }
        else
        {
            Server._LoadingEnd();
            MessageBox.Instance.Show(friendReqData.message);
            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }

    //*********************************************************************************************************************
    //Pending User Friend Req
    public void PendingFriendReq()
    {
        Debug.Log("PendingFriendReq");
        Server.Instance.ApiRequest(typeof(PendingFriendReq), OnCallPendingFriendReq);
    }

    private void OnCallPendingFriendReq(JSONObject data)
    {
        Debug.Log("OnCallUserFriendList");
        // SetStatus("User Detail recived");
        //FriendsList.instance.ShowFriendListData();

        PendingfriendrequestData pendingFriendReqData = JsonUtility.FromJson<PendingfriendrequestData>(data.Print());

        //friendList = JsonUtility.FromJson<FriendListData>(data.Print());

        //DataManager.Instance.FriendListdata = friendList;
        ////data from api

        if (pendingFriendReqData.success)
        {
            //_LoadingStart();
            Debug.Log(pendingFriendReqData);
            _addFriends.msgText.gameObject.SetActive(false);
            _addFriends.PendingFriendReqwUpdate(pendingFriendReqData);
            //PhotonLobbyManager.instance.ConnectToPhoton();
        }
        else
        {
            Server._LoadingEnd();
            _addFriends.msgText.gameObject.SetActive(true);
            _addFriends.msgText.text = pendingFriendReqData.message;
            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }

    //*****************************************************************************************
    //Accept or Reject friend Req
    public void AcceptOrRejectFriendReq(int id, int reqType)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("id", id);
        formData.AddField("request_type", reqType);
        Debug.Log("AcceptOrRejectFriendReq");
        Server.Instance.ApiRequest(typeof(AcceptOrRejectFriendRequest), OnCallAcceptOrRejectFriendReq, formData, false);
    }

    private void OnCallAcceptOrRejectFriendReq(JSONObject data)
    {
        Debug.Log("OnCallAcceptOrRejectFriendReq");
        // SetStatus("User Detail recived");
        //FriendsList.instance.ShowFriendListData();

        //friendList = JsonUtility.FromJson<FriendListData>(data.Print());

        //DataManager.Instance.FriendListdata = friendList;
        ////data from api
        ///
        AcceptOrRejectFriendRequestData acceptOrRejectFriendRequestData = JsonUtility.FromJson<AcceptOrRejectFriendRequestData>(data.Print());

        if (acceptOrRejectFriendRequestData.success)
        {
            //_LoadingStart();
            Debug.Log(acceptOrRejectFriendRequestData);
            MessageBox.Instance.Show(acceptOrRejectFriendRequestData.message);
            //PhotonLobbyManager.instance.ConnectToPhoton();
        }
        else
        {
            Server._LoadingEnd();
            MessageBox.Instance.Show(acceptOrRejectFriendRequestData.message);
            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////
    //Remove Friend
    public void Unfriend(int id)
    {
        WWWForm formData = new WWWForm();
        formData.AddField("unfriend_id", id);
        Debug.Log("Unfriend");
        Server.Instance.ApiRequest(typeof(Unfriend), OnCallUnfriend, formData, false);
    }

    private void OnCallUnfriend(JSONObject data)
    {
        Debug.Log("OnCallUnfriend");
        // SetStatus("User Detail recived");
        //FriendsList.instance.ShowFriendListData();

        //friendList = JsonUtility.FromJson<FriendListData>(data.Print());

        //DataManager.Instance.FriendListdata = friendList;
        ////data from api
        ///
        UnfriendData unfriendData = JsonUtility.FromJson<UnfriendData>(data.Print());

        if (unfriendData.success)
        {
            //_LoadingStart();
            Debug.Log(unfriendData);
            MessageBox.Instance.Show(unfriendData.message);
            //PhotonLobbyManager.instance.ConnectToPhoton();
        }
        else
        {
            Server._LoadingEnd();
            MessageBox.Instance.Show(unfriendData.message);
            //ScreenNavManager.Instance.ShowScreen("NetworkLost");
        }
    }
}