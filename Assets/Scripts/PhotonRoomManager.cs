using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;

//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonRoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text roomText;

    [SerializeField] private SeatData RoomSeatData;
    [SerializeField] private string currentRoomSeatDataStatus = "";

    public bool IsLastConnectedAndJoined;

    public static PhotonRoomManager instance;

    public const string CARD_DECK_IDS = "card_id";
    public const string POT_COINS = "pot_coin";
    public const string CURRENT_PLAYER_TURN_ID = "current_player_turn_id";
    public const string IS_ROUND_RUNNING = "round_running";

    public const string CURRENT_BET_AMOUNT = "current_bet_amount";
    public const string TABLE_MINIMUM_BET = "table_min_bet";

    public const string PLAYER_WAITING_TIMER = "player_waiting_timer";
    public const string PLAYER_TIMER = "player_timer";
    public const string SEAT_AVALIBALITY = "player_seat";
    public const string TABLE_ID = "table_id";

    private bool isRoomCreate;

    private bool isForceLeaveRoom;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }

    private void Update()
    {
        string str = "";
        str += "Photon Conected= " + PhotonNetwork.IsConnected + " \n";
        str += "In Lobby= " + PhotonNetwork.InLobby + " \n";
        str += "In Room= " + PhotonNetwork.InRoom + " \n";
        str += "Is Master client= " + PhotonNetwork.IsMasterClient + " \n";
        str += "my SeatId= " + PhotonPlayerManager.instance.mySeatId + " \n";
        
       
        if (PhotonNetwork.InRoom)
        {
            str += "my occupied= " + (string)PhotonNetwork.CurrentRoom.CustomProperties[SEAT_AVALIBALITY] + " \n";
            str += "my Seat id= " + PhotonNetwork.LocalPlayer.CustomProperties[PhotonPlayerManager.PLAYER_SEAT_ID] + " \n";
            str += "PhotonNetwork.CurrentRoom.Name= " + PhotonNetwork.CurrentRoom.Name + " \n";
            str += "Total Player In room = " + PhotonNetwork.PlayerList.Length + " \n";
            //str += "Total Active Player In room = " + PhotonPlayerManager.instance.GetActivePlayerCount() + " \n";
            str += "Total Active Player In room = " + PhotonRoomManager.instance.GetTurnId() + " \n";
            if (PhotonPlayerManager.instance.currentPlayer != null)
            {
                str += "my Seat Id seatID = " + PhotonPlayerManager.instance.mySeatId + " \n";
                str += "currentPlayer seatID = " + PhotonPlayerManager.instance.currentPlayer.PlayerSeatId + " \n";
            }
            //str += "currentRoomSeatDataStatus= " + currentRoomSeatDataStatus + " \n";
        }
        roomText.text = str;
    }

    //ROOM METHOD******************************************************************************************************************
    //CREATE OR  JOIN PUBLIC ROOM**********************************************************************************
    public void JoinRandomRoom()
    {
        //if ((2 * GameSetting.Instance.MinimumBetAmount) > WalletManager.instance.GetTempWalletAmount())
        if (WalletManager.instance.GetWalletAmount() < GameSetting.Instance.TempWalletAmount)
        {
            MessageBox.Instance.Show("Your wallet balance is low.\nPlease add money.", MsgType.OKONLY);
            return;
        }

        Debug.Log("============JoinRandomRoom==============");
        if (!PhotonNetwork.IsConnected) return;
        //PhotonPlayerManager.instance.SetUpdatedWalletBalance();
        Server._LoadingStart();
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRandomRoom()
    {
        Debug.Log("============CreateRandomRoom==============");
        if (!PhotonNetwork.IsConnected) return;
        isRoomCreate = true;
        PhotonNetwork.CreateRoom(null, GetPublicRoomOptions(), TypedLobby.Default);
    }

    //PRIVATE ROOM**********************************************************************************
    public void CreatePrivateRoom()
    {
        //if ((2 * GameSetting.Instance.MinimumBetAmount) > WalletManager.instance.GetTempWalletAmount())
        if (WalletManager.instance.GetWalletAmount() < GameSetting.Instance.TempWalletAmount)
        {
            MessageBox.Instance.Show("Your wallet balance is low.\nPlease add money.", MsgType.OKONLY);
            return;
        }

        if (!PhotonNetwork.IsConnected) return;

        isRoomCreate = true;
        Server._LoadingStart();
        //PhotonPlayerManager.instance.SetUpdatedWalletBalance();
        string roomCode = Utility.RandomString(6);//Utility.GetUniqueID();
        PhotonNetwork.CreateRoom(roomCode, GetPublicRoomOptions(true), null);
    }

    public void JoinPrivateRoom(string roomCode)
    {

        //if ((2 * GameSetting.Instance.MinimumBetAmount) > WalletManager.instance.GetTempWalletAmount())
        if (WalletManager.instance.GetWalletAmount() < GameSetting.Instance.TempWalletAmount)
        {
            MessageBox.Instance.Show("Your wallet balance is low.\nPlease add money.", MsgType.OKONLY);
            return;
        }
        Debug.Log("Call _ Join Room");
        if (!PhotonNetwork.IsConnected) return;
        PhotonPlayerManager.instance.SetUpdatedWalletBalance();
        isRoomCreate = false;
        Server._LoadingStart();
        Debug.Log("Join Room " + roomCode);
        PhotonNetwork.JoinRoom(roomCode);
    }

    //*****************************************************************************************
    /* public void Call_CreateRoom()
     {
         Debug.Log("Call_CreateRoom");
         if (!PhotonNetwork.IsConnected) return;
         isRoomCreate = true;
         Server._LoadingStart();

         string roomCode = Utility.RandomString(6);

         PhotonNetwork.CreateRoom(roomCode, Room_options_Code(), null);
     }*/

    public void Call_LeaveRoom()
    {
        
        if (GameManager.Instance.currentGameMode == GameMode.PLAY_ONLINE)
        {
            PhotonWaitingTimerManager.instance.StopMyRoundTimer();
        }
        Debug.Log("Call _ LeaveRoom");
        isForceLeaveRoom = true;
        IsLastConnectedAndJoined = false;
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("PhotonNetwork.InRoom");
            PhotonNetwork.LeaveRoom();
            Server._LoadingStart();
        }
        else
        {
            Debug.Log("PhotonNetwork.InRoom not");
            //PopupsManager.Instance.Show("CreateOrJoinPopup");
            PopupsManager.Instance.Hide();
        }
    }

    // PHOTON CALLBACK**********************************************************************
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("Create room failed: " + message + " returnCode" + returnCode);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("============OnJoinedRoom==============");
        base.OnJoinedRoom();
        print("Room created successfully" + PhotonNetwork.CurrentRoom.Name);

        GameManager.Instance.Wait(.5f * PhotonNetwork.PlayerList.Length, () =>
        {
            PhotonPlayerManager.instance.SetSeatId(PhotonNetwork.LocalPlayer, GetNextAvailableSeatId());
            GameEvents.EventScreenshow?.Invoke(ScreenType.Game);
            WalletManager.instance.SetTempWalletBalance(GameSetting.Instance.TempWalletAmount);
            PhotonPlayerManager.instance.SetUpdatedWalletBalance();

            Server._LoadingEnd();
        });

        if (GameManager.Instance.IsGameOn())
        {
            //PhotonSenderReciver.Instance.Send_GameResume_Event();
        }
        else
        {
            print("its fresh game");
            if (IsLastConnectedAndJoined)
            {
                return;
            }

            /*if (!GameSettings.instance.IsOnlinePlayer())
            {
                if (isRoomCreate)
                {
                    PopupsManager.Instance.Show("CreateRoomPopup");
                }
                else
                {
                    PopupsManager.Instance.Show("WaitingPopup");
                }
            }*/

            //GameManager.instance.ResetGame();

            IsLastConnectedAndJoined = true;
            Debug.Log("IsLastConnectedAndJoined=" + IsLastConnectedAndJoined);

            PlayerPrefs.SetString("RoomCode", GetRoomName());

            //GameManager.instance.isGameOn = true;
            //PhotonPlayerManager.instance.CheckPlayer();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("============OnJoinRandomFailed==============");
        Debug.Log("failed message " + message);

        base.OnJoinRoomFailed(returnCode, message);
        //Server._LoadingEnd();
        Debug.Log("returnCode " + returnCode);
        IsLastConnectedAndJoined = false;
        if (GameManager.Instance.IsOnlinePlayer())
        {
            CreateRandomRoom();
            return;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Server._LoadingEnd();

        Debug.Log("room join failed here returnCode " + returnCode);

        if (returnCode == 32758 && !IsLastConnectedAndJoined)
        //if (returnCode == 32758)
        {
            MessageBox.Instance.Show(message, MsgType.OKONLY);
            return;
        }
        IsLastConnectedAndJoined = false;
        if (returnCode == 32748)
        {
            /*string roomCode = PlayerPrefs.GetString("RoomCode","");
            if (roomCode != "")
            {
                JoinPrivateRoom(roomCode);
                return;
            }*/

            MessageBox.Instance.Show(message, MsgType.OKONLY);
            MessageBox.Instance.setOkText("Home");
            MessageBox.Instance.OnOkClicked += OnHomeClicked;

            //GameManager.Instance.EndGame();
            return;
        }

        MessageBox.Instance.Show(message, MsgType.OKONLY);
        MessageBox.Instance.setOkText("Home");
        MessageBox.Instance.OnOkClicked += OnHomeClicked;
        //GameManager.Instance.EndGame();
        Debug.Log("On JoinRoom Failed");
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //base.OnRoomPropertiesUpdate(propertiesThatChanged);
        string str = "";
        if (propertiesThatChanged.ContainsKey(CARD_DECK_IDS))
        {
            str = (string)propertiesThatChanged[CARD_DECK_IDS];
            Debug.Log("OnRoomPropertiesUpdate=" + CARD_DECK_IDS + "=" + str);
        }
        if (propertiesThatChanged.ContainsKey(SEAT_AVALIBALITY))
        {
            string s = (string)propertiesThatChanged[SEAT_AVALIBALITY];
            currentRoomSeatDataStatus = s;
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
        //Server._LoadingEnd();
        IsLastConnectedAndJoined = false;
        PhotonPlayerManager.instance.RemovePlayer(PhotonNetwork.LocalPlayer);
        /*if (ChessGameController.Instance.GetGameState() == GameState.Waiting)
        {
            if (GameSettings.instance.IsOnlinePlayer())
            {
                PopupsManager.Instance.Hide();
            }
            else
            {
                PopupsManager.Instance.Show("CreateOrJoinPopup");
            }
        }*/
        if (isForceLeaveRoom)
        {
            //PopupsManager.Instance.Hide();
            isForceLeaveRoom = false;
        }
    }

    //END Photon callback***********************************************************************************

    //Set/Get room propertis**********************************************************************

    //**************************************************************************************
    //GETTER******************************************************************************
    public double GetWaitingTime()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PLAYER_WAITING_TIMER))
        {
            double waitTime = (double)PhotonNetwork.CurrentRoom.CustomProperties[PLAYER_WAITING_TIMER];
            return waitTime;
        }
        return 0f;
    }

    public double GetRoundTime()
    {
        if (PhotonNetwork.InRoom)
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PLAYER_TIMER))
            {
                double RoundTime = (double)PhotonNetwork.CurrentRoom.CustomProperties[PLAYER_TIMER];
                return RoundTime;
            }
        return 0f;
    }

    public string GetRoomName()
    {
        return PhotonNetwork.CurrentRoom.Name;
    }

    /* public List<int> GetRandomDeck()
     {
         string str = (string)PhotonNetwork.CurrentRoom.CustomProperties[CARD_DECK_IDS];
         List<int> ids = Utility.StringToIntList(str);
         return ids;
     }*/

    public bool IsPlayerInRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InRoom)
            {
                return true;
            }
        }
        return false;
    }

    private RoomOptions GetPublicRoomOptions(bool isPrivate = false)
    {
        RoomOptions roomOps = new RoomOptions();
        roomOps.IsVisible = !isPrivate;

        //roomOps.IsOpen = true;
        //roomOps.EmptyRoomTtl = 60000;
        //roomOps.PlayerTtl = -1;
        roomOps.PublishUserId = true;
        roomOps.MaxPlayers = GameSetting.Instance.maxPlayers;
        roomOps.CustomRoomProperties = GetTable();

        return roomOps;
    }

    private Hashtable GetTable()
    {
        Hashtable tbl = new Hashtable {
            { CARD_DECK_IDS, "" },
            { POT_COINS, "" },
            { CURRENT_PLAYER_TURN_ID, ""},
            { IS_ROUND_RUNNING, false },
            { CURRENT_BET_AMOUNT, "" },
            { TABLE_MINIMUM_BET, GameSetting.Instance.MinimumBetAmount },
            { PLAYER_TIMER, GameSetting.Instance.PlayerTime },
            { PLAYER_WAITING_TIMER, GameSetting.Instance.PlayerWaitingTime },
            { SEAT_AVALIBALITY, GetDefaultSet() },
            { TABLE_ID, "" },
        };
        return tbl;
    }

    public string GetTurnId()
    {
        string sId = (string)PhotonNetwork.CurrentRoom.CustomProperties[CURRENT_PLAYER_TURN_ID];
        return sId;
    }

    public string GetTableId()
    {
        string tId = (string)PhotonNetwork.CurrentRoom.CustomProperties[TABLE_ID];
        return tId;
    }

    public double GetMinimumBetAmount()
    {
        double sId = (double)PhotonNetwork.CurrentRoom.CustomProperties[TABLE_MINIMUM_BET];
        return sId;
    }

    public double GePotAmount()
    {
        double pc = (double)PhotonNetwork.CurrentRoom.CustomProperties[POT_COINS];
        return pc;
    }

    private string GetDefaultSet()
    {
        string playerToJson = JsonUtility.ToJson(RoomSeatData);
        Debug.Log(playerToJson);
        return playerToJson;
        //JsonUtility.FromJson
    }

    public int GetNextAvailableSeatId()
    {
        string str = (string)PhotonNetwork.CurrentRoom.CustomProperties[SEAT_AVALIBALITY];
        SeatData sd = JsonUtility.FromJson<SeatData>(str);
        Seat freeSeat = sd.Seats.Find(i => i.isOccupied == false);
        freeSeat.isOccupied = true;
        string updatedStr = JsonUtility.ToJson(sd);
        //Debug.Log(updatedStr);
        UpdateRoomSeat(updatedStr);

        return freeSeat.SeatId;
    }

    public void RemoveOccupiedSeat()
    {
        string str = (string)PhotonNetwork.CurrentRoom.CustomProperties[SEAT_AVALIBALITY];
        SeatData sd = JsonUtility.FromJson<SeatData>(str);
        for (int i = 0; i < sd.Seats.Count; i++)
        {
            if (i <= PhotonNetwork.PlayerList.Length - 1)
                sd.Seats[i].isOccupied = true;
            else
                sd.Seats[i].isOccupied = false;
        }
        //Seat occupiedSeat = sd.Seats.Find(i => i.SeatId == seatId);
        //occupiedSeat.isOccupied = false;

        string updatedStr = JsonUtility.ToJson(sd);
        
        UpdateRoomSeat(updatedStr);
    }

    //SETTER******************************************************************************

    public void SetRandomDeck(string card_value)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log(PhotonNetwork.IsMasterClient);

        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
            {CARD_DECK_IDS, card_value}
        });
    }

    public void SetTurnId(string tId)
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
            {CURRENT_PLAYER_TURN_ID, tId}
        });
    }

    public void SetTableId(string tId)
    {
        //if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
            {TABLE_ID, tId}
        });
    }

    public void SetGameState(RoomState gs)
    {
    }

    public void SetRoomClose()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        //PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void SetVisibleOff()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        //PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void SetPotAmount(double value)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
            {POT_COINS, value}
        });
    }

    public void SetBetAmount(double currentBetAmount)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
            {CURRENT_BET_AMOUNT, currentBetAmount}
        });
    }

    //UPDATER***********************************************************************************
    public void UpdateRoomSeat(string updatedSeat)
    {
        Debug.Log(updatedSeat);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
            {SEAT_AVALIBALITY, updatedSeat}
        });
    }

    public void RemoveTopThreeCard()
    {
        string str = (string)PhotonNetwork.CurrentRoom.CustomProperties[CARD_DECK_IDS];
        List<int> ids = Utility.StringToIntList(str);

        for (int i = 0; i < 3; i++)
        {
            ids.RemoveAt(ids.Count - 1);
        }

        string new_card_value = Utility.IntListToString(ids);
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable {
            {CARD_DECK_IDS, new_card_value}
        });
    }

    //********************************************************************************************
    private void OnHomeClicked()
    {
        MessageBox.Instance.OnOkClicked -= OnHomeClicked;
        PhotonLobbyManager.instance.DisconnetToPhoton();
    }
}

[Serializable]
public class SeatData
{
    public List<Seat> Seats;
}

[Serializable]
public class Seat
{
    public int SeatId = 0;
    public bool isOccupied = false;
}