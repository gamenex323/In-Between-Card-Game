using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonLobbyManager : MonoBehaviourPunCallbacks
{
    //public string gameVersion = "0.1";
    public static PhotonLobbyManager instance;

    public bool IsForceDisconnected;
    public bool IsLastConnected;
    public static Action<Player> OnMasterSwitch;
    public Text amountText;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    //Method*****************************************************************************************
    public void ConnectToPhoton()
    {
        //ChessGameController.Instance.RestartGame();
        print("ConnectToPhoton");
        Server.Instance.SetStatus("Connect To Photon");
        print("PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState===" + PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);

        if (PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
        {
            IsForceDisconnected = true;
            Server._LoadingStart();
            //AdsManager.instance.DestroyBannerAd();
            PhotonNetwork.Disconnect();
            return;
        }

        IsForceDisconnected = false;
        IsLastConnected = false;
        //PhotonRoomManager.instance.IsLastConnectedAndJoined = false;

        Server._LoadingStart();
        print("Connectiong to photon server......");
        Server.Instance.SetStatus("Connecting to photon server......");
        //PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.SendRate = 80;
        PhotonNetwork.SerializationRate = 80;
        PhotonPlayerManager.instance.PlayerSetDefaultValue();
        PhotonNetwork.ConnectUsingSettings();
    }

    public void DisconnetToPhoton()
    {
        print("Force DisconnetToPhoton");
        if (PhotonNetwork.IsConnected)
        {
            IsForceDisconnected = true;
            //PhotonRoomManager.instance.IsLastConnectedAndJoined = false;
            Server._LoadingStart();

            //PhotonSenderReciver.Instance.Send_GameStop_Event();

            PhotonNetwork.Disconnect();
        }
        else
        {
            //Server._LoadingStart();
            ConnectToPhoton();
        }
        //Forced leave game
    }

    public bool IsIAmDealer()
    {
        if (PhotonNetwork.IsMasterClient)
            return true;
        else
            return false;
    }

    public void LogOutGame()
    {
        amountText.text = "0";
        PlayerPrefs.SetInt("firstCome", 0);
        PlayerPrefs.SetInt("User" + DataManager.Instance.GetUserId() + "_balance_", DataManager.Instance.GetUserBalance());
        MessageBox.Instance.Show("Are you sure, you want to log out?", MsgType.YESNOBOTH);
        MessageBox.Instance.OnOkClicked += OnLogoutYes;


    }

    public void ReLoginGame()
    {
        PopupsManager.Instance.Hide();
        PopupsManager2.Instance.Hide();
        MessageBox.Instance.Show("Session Timeout. Please login again!", MsgType.OKONLY);
        MessageBox.Instance.setOkText("Login");
        MessageBox.Instance.OnOkClicked += OnLogoutYes;
        Server._LoadingEnd();



    }
    void OnLogoutYes()
    {
        MessageBox.Instance.OnOkClicked -= OnLogoutYes;
        DataManager.Instance.SetAuthKey("");
        DataManager.Instance.SetProfileImage(null);
        GameEvents.EventScreenshow?.Invoke(ScreenType.Login);
        IsForceDisconnected = false;
        IsLastConnected = false;
        PhotonRoomManager.instance.IsLastConnectedAndJoined = false;
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
    }
    //Photon Callback**************************************************************************
    public override void OnConnectedToMaster()
    {
        print("photon Connected to server");
        Server.Instance.SetStatus("photon Connected to server");
        //print(PhotonNetwork.LocalPlayer.NickName);
        if (!PhotonNetwork.InLobby)
            PhotonNetwork.JoinLobby();
        print("photon joining lobby...");
        Server.Instance.SetStatus("photon joining lobby...");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        print("OnMasterClientSwitched");
        if (OnMasterSwitch != null)
        {
            OnMasterSwitch(newMasterClient);
        }

    }

    public override void OnJoinedLobby()
    {
        print("Lobby joined");
        //if (!IsLastConnected)
        //{
        //    print("Show home...");
        //    ScreenNavManager.Instance.ShowScreen("HomeScreen");
        //    //PopupsManager.Instance.Show("WaitingPopup");
        //}
        Server._LoadingEnd();

        Debug.Log("Is game on " + GameManager.Instance.IsGameOn());
        Debug.Log("PhotonRoomManager.instance.IsLastConnectedAndJoined " + PhotonRoomManager.instance.IsLastConnectedAndJoined);

        if (GameManager.Instance.IsGameOn())
        {
            //Debug.Log("we left the room id is  " + PlayerPrefs.GetString("LeftRoomId"));
            Debug.Log("IsGameOn--  rejoin room=" + PlayerPrefs.GetString("LeftRoomId"));

            PhotonNetwork.JoinRoom(PlayerPrefs.GetString("LeftRoomId"));
        }
        else if (PhotonRoomManager.instance.IsLastConnectedAndJoined)
        {
            //Debug.Log("PhotonRoomManager.instance.IsLastConnectedAndJoinedPhotonRoomManager.instance.IsLastConnectedAndJoined");

            //Debug.Log("Create or join room --  " + PlayerPrefs.GetString("LeftRoomId"));

            //PhotonRoomManager.instance.CreateRoom(PlayerPrefs.GetString("LeftRoomId"));

            //PhotonNetwork.JoinOrCreateRoom(PlayerPrefs.GetString("LeftRoomId"));
        }
        else
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
            //Debug.Log( "game is off  " + ChessGameController.Instance.IsGameOn());
        }
        IsLastConnected = true;
    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        //base.OnDisconnected(cause);
        print("Disconnected from server for resion " + cause.ToString());
        Server.Instance.SetStatus("Disconnected from server for resion" + cause.ToString());
        //Server.Instance.logTxt2.text = cause.ToString();

        if (cause.ToString() == "ClientTimeout")
        {
            if (IsLastConnected == false)
            {
                ConnectToPhoton();
                return;
            }
            else
            {
                Debug.Log("game is on disconnecting...." + GameManager.Instance.IsGameOn());

                if (GameManager.Instance.IsGameOn())
                {
                    Debug.Log("game is on...." + GameManager.Instance.IsGameOn());
                }
                else
                {
                    Debug.Log("game is off on disconnect...." + GameManager.Instance.IsGameOn());

                    MessageBox.Instance.Show("Session timed out, please return home", MsgType.OKONLY);
                    MessageBox.Instance.setOkText("Home");
                    MessageBox.Instance.OnOkClicked += OnHomeBtnClicked;
                    //GameManager.Instance.EndGame(true);
                    IsLastConnected = false;
                }
            }
        }

        if (IsForceDisconnected)
        {
            ConnectToPhoton();
            return;
        }

        //if (cause.ToString() == "DisconnectByClientLogic" || cause.ToString() == "ServerTimeout") return;

        //MessageBox.Instance.Show("Session timed out, please return home", MsgType.OKONLY);
        //MessageBox.Instance.setOkText("Home");
        //MessageBox.Instance.OnOkClicked += OnHomeBtnClicked;
        //IsLastConnected = false;

        //ScreenNavManager.Instance.ShowScreen("NetworkLost");
    }

    private void OnHomeBtnClicked()
    {
        MessageBox.Instance.OnOkClicked -= OnHomeBtnClicked;

        DisconnetToPhoton();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (GallaryImagePicker.Instance.isImageUploading) return;
        Debug.Log("OnApplicationPause " + isPaused);
        if (isPaused)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            //if (PhotonNetwork.IsConnected)
            //{
            //    PhotonNetwork.Disconnect();
            //}

            //PopupsManager.Instance.Hide();
            //GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
            //AutoLoginManager.Instance.CheckAndLoadScreen();
            SceneManager.LoadScene(0);
        }
    }
}