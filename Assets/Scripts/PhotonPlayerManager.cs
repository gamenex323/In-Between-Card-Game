using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonPlayerManager : MonoBehaviourPunCallbacks
{
    public static Action OnAllPlayersJoined;
    public static Action OnNewPlayerJoined;
    public static Action OnAllPlayersReady;
    public static Action OnAllPlayersCoinRecived;
    public static Action OnCenterCardFliped;
    public static Action OnPlayerWinLossEventCompleted;

    [SerializeField] private ActionButtons buttonActionPanel;
    [SerializeField] private List<PlayerObj> playersSittingList;

    [SerializeField] private Transform betPosObj;
    public State myState;
    public int mySeatId;
    public string myPlayerName;
    public PlayerObj myPlayer;
    public PlayerObj currentPlayer;
    public double currentBetAmount;

    public const string PLAYER_STATE = "p_state";
    public const string PLAYER_SEAT_ID = "p_seat_id";
    public const string PLAYER_COIN = "p_coin";
    public const string IS_COIN_COLECTED = "is_coin_collected";
    public const string PLAYER_IMAGE_URL = "p_image_url";

    private int numberOfPlayers;
    public static PhotonPlayerManager instance;

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

    private void Start()
    {
        ResetPlayers();
        ActionButtons.OnClicked += OnActionClicked;
        ActionButtons.OnRaiseAmountChanged += OnRaiseAmountChanged;

        WalletManager.OnAmoutDeducted += SetAmoutDeductedStatus;
        WalletManager.OnWalletUpated += SetUpdatedWalletBalance;
        PhotonRoundTimerManager.OnRoundTimeFinished += OnRoundTimeFinished;
    }

    private void OnDestroy()
    {
        ActionButtons.OnClicked -= OnActionClicked;
        ActionButtons.OnRaiseAmountChanged -= OnRaiseAmountChanged;
        WalletManager.OnAmoutDeducted -= SetAmoutDeductedStatus;
        WalletManager.OnWalletUpated -= SetUpdatedWalletBalance;
        PhotonRoundTimerManager.OnRoundTimeFinished -= OnRoundTimeFinished;
    }

    //*************************************************************************************************************
    //PLAYER EVENT ***********************************************************************************************
    //*************************************************************************************************************
    public void PlayerSetDefaultValue()
    {
        PhotonNetwork.NickName = DataManager.Instance.GetUserName();

        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.None;

        //this will set from api
        // string str = (UnityEngine.Random.Range(0, 999)).ToString();
        // PhotonNetwork.AuthValues.UserId = "User_Id" + str;
        PhotonNetwork.AuthValues.UserId = DataManager.Instance.UserDetailsData.userdata.id.ToString();
        WalletManager.instance.SetTempWalletBalance(GameSetting.Instance.TempWalletAmount);
        //Player default value set
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {
                { PLAYER_COIN,  WalletManager.instance.GetTempWalletAmount() },
                { IS_COIN_COLECTED,  false },
                {PLAYER_IMAGE_URL,DataManager.Instance.UserDetailsData.userdata.profile_avatar}
        });
    }

    public void InitPlayer()
    {
        currentPlayer = null;
        betPosObj.gameObject.SetActive(false);
        buttonActionPanel.DisableBtns();
        for (int i = 0; i < playersSittingList.Count; i++)
        {
            playersSittingList[i].gameObject.SetActive(false);
            //playersSittingList[i].SetEnableDisable(false);
        }
        //SetSeatId(PhotonNetwork.LocalPlayer, PhotonRoomManager.instance.GetNextAvailableSeatId());
    }

    public void OnActionClicked(ButtonActionType type)
    {
        switch (type)
        {
            case ButtonActionType.FOLD:

                betPosObj.gameObject.SetActive(false);
                PhotonSenderReciver.instance.ButtonSelected((int)type, 0);
                PhotonRoundTimerManager.instance.StopRoundTimer();

                break;

            case ButtonActionType.DRAW:
                OnButtonSelected(type, currentBetAmount);

                break;

            case ButtonActionType.RAISE:

                break;

            case ButtonActionType.MAXBET:
                    double amt = (WalletManager.instance.GetTempWalletAmount() >= PotManager.Instance.totalPotAmount) ? PotManager.Instance.totalPotAmount : WalletManager.instance.GetTempWalletAmount();
                    OnButtonSelected(type, amt);
                break;
        }
    }
    ButtonActionType type;
    double amt;
    private void OnButtonSelected(ButtonActionType type, double amt)
    {
        WalletManager.instance.DeductAmount(amt);
        this.type = type;
        this.amt = amt;
        //PhotonSenderReciver.instance.ButtonSelected((int)type, amt);
        PhotonRoomManager.instance.SetBetAmount(amt);
        PhotonRoundTimerManager.instance.StopRoundTimer();

        //play bet api .. no loading
        Debug.Log(DataManager.Instance.UserDetailsData.userdata.id.ToString());
        OnPlayBet(PhotonRoomManager.instance.GetTableId(), DataManager.Instance.UserDetailsData.userdata.id.ToString(), amt, "bet");
    }

    /******************************************************************************************************/
    /*Play Bet ************************************************************************************/
    /******************************************************************************************************/

    public void OnPlayBet(string table_id, string user_id, double amount, string deal_status)
    {
        Debug.Log("table_id=" + table_id);
        Debug.Log("user_id=" + user_id);
        Debug.Log("amount=" + amount);
        Debug.Log("deal_status=" + deal_status);

        WWWForm formData = new WWWForm();
        formData.AddField("table_id", "1");
        formData.AddField("user_id", user_id);
        formData.AddField("amount", amount.ToString());
        formData.AddField("deal_status", deal_status);
        //Debug.Log("play bet api" + formData.data);
        Server.Instance.ApiRequest(typeof(PlayBet), OnPlayBetUpdated, formData, false);
    }

    private void OnPlayBetUpdated(JSONObject data)
    {
        PlayBetRoot playBetRoot = JsonUtility.FromJson<PlayBetRoot>(data.Print());
        playBetRoot.success = true;
        if (playBetRoot.success)
        {
            Debug.Log("play bet api success");
            DataManager.Instance.PlayBetData = playBetRoot;
            PhotonSenderReciver.instance.ButtonSelected((int)type, amt);
        }
        else
        {
            MessageBox.Instance.Show(playBetRoot.message, MsgType.OKONLY);
        }
    }

    private void OnRaiseAmountChanged(double amt)
    {
        //Debug.LogError("OnRaiseAmountChanged=" + amt);
        currentBetAmount = amt;
        betPosObj.GetComponentInChildren<Text>().text = currentBetAmount.ToString();
    }

    private void OnRoundTimeFinished()
    {
        if (GameManager.Instance.currentGameState != State.WAITING)
        {
            //if (GameManager.instance.currentGameState == GameState.GAME_COMPLETED) return;

            if (PhotonNetwork.PlayerList.Length == 1)
            {
                MessageBox.Instance.Show("All players left the game.", MsgType.OKONLY);
                MessageBox.Instance.setOkText("Home");
                MessageBox.Instance.OnOkClicked += OnHomeClicked;
                GameManager.Instance.RoundOver();
                //PhotonRoomManager.instance.SetRoomClose();
            }
        }
        if (GameManager.Instance.isMyturn)
        {
            betPosObj.gameObject.SetActive(false);
            buttonActionPanel.DisableBtns();
            PhotonSenderReciver.instance.ButtonSelected((int)ButtonActionType.FOLD, 0);
        }
    }

    //COLLECT**********************************************************************************
    public void PlayPotCollectionAnimation(Action _callback)
    {
        PotManager.Instance.CollectPotAmount();
        for (int i = 0; i < playersSittingList.Count; i++)
        {
            playersSittingList[i].PlayCoinPotCollectionAnimation(_callback);
        }
    }

    //BET**********************************************************************************
    public void PlayBetAnimation(double amt)
    {
        //Debug.LogError("PlayBetAnimation=" + amt);
        currentBetAmount = amt;
        currentPlayer.PlayCoinBetAnimation(currentBetAmount, OnBetAnimationCompleted);
    }

    private void OnBetAnimationCompleted()
    {
        betPosObj.gameObject.SetActive(false);
        CardDeckManager.Instance.FlipCenterCard(OnCenterCardFliped);
    }

    //WIN/LOSS**********************************************************************************
    public void ShowWinLoseAnimation(bool isWin)
    {
        //Debug.LogError("ShowWinLoseAnimation=" + currentBetAmount);

        if (isWin)
        {
            currentPlayer.PlayWinLoseAnimation();
            PotManager.Instance.PlayWinAmoutPotToBetPosition(OnWinCoinMovedPotToBetPos);
        }
        else
        {
            currentPlayer.PlayCoinLossAnimation(currentBetAmount, OnWinLossAmountRecivedAnimationCompleted);
            //Debug.LogError(currentBetAmount);
            PotManager.Instance.AddPotAmount(currentBetAmount);
        }

        if (GameManager.Instance.isMyturn)
        {
            string won_status = isWin ? "won" : "lose";
            string table_id = DataManager.Instance.PlayBetData.dealPlayData.deal_play_id;
            OnDealResult(table_id, won_status);
        }
    }

    /******************************************************************************************************/
    /*Deal result ************************************************************************************/
    /******************************************************************************************************/

    public void OnDealResult(string table_id, string won_status)
    {
        Debug.Log("OnDealResult deal_play_id=" + table_id);
        WWWForm formData = new WWWForm();
        formData.AddField("deal_play_id", table_id);
        formData.AddField("won_status", won_status);
        //Debug.Log("deal bet api" + formData.data);
        Server.Instance.ApiRequest(typeof(DealResult), OnDealResultRecieved, formData, false);
    }

    private void OnDealResultRecieved(JSONObject data)
    {
        DealResultRoot dealResult = JsonUtility.FromJson<DealResultRoot>(data.Print());

        if (dealResult.success)
        {
            Debug.Log("deal result api success");
            DataManager.Instance.DealResult = dealResult;
        }
        else
        {
            MessageBox.Instance.Show(dealResult.message, MsgType.OKONLY);
        }
    }

    //*******************************************************************************************************
    public void OnWinCoinMovedPotToBetPos()
    {
        GameManager.Instance.Wait(2f, () =>
        {
            Debug.Log("OnWinCoinMovedPotToBetPos");
            currentPlayer.PlayCoinWinCollectAnimation(2 * currentBetAmount, OnWinLossAmountRecivedAnimationCompleted);
            if (currentPlayer == myPlayer)
            {
                WalletManager.instance.AddWinngAmount(2 * currentBetAmount);
            }
        }
        );
    }

    private void OnWinLossAmountRecivedAnimationCompleted()
    {
        Debug.Log("OnWinLossAmountRecivedAnimationCompleted");
        GameManager.Instance.Wait(1.5f, () =>
        {
            OnPlayerWinLossEventCompleted?.Invoke();
        });
    }

    private void OnAnimationCompleted()
    {
        Debug.Log("OnAnimationCompleted");
    }

    //************************************************************************************************************

    public void ActionButtonEnble()
    {
        betPosObj.gameObject.SetActive(true);
        betPosObj.GetComponentInChildren<Text>().text = "BET";
        buttonActionPanel.EnableBtns();
    }

    public void CheckSetPlayersEnableByAvailableCoins()
    {
        for (int i = 0; i < playersSittingList.Count; i++)
        {
            if (playersSittingList[i].isActive && playersSittingList[i].gameObject.activeSelf)
            {
                playersSittingList[i].CheckCoinsToPlay();
            }
        }
    }

    private void ResetPlayers()
    {
        for (int i = 0; i < playersSittingList.Count; i++)
        {
            playersSittingList[i].GetComponent<PlayerObj>().ResetPlayer();
            playersSittingList[i].gameObject.SetActive(false);
        }
        betPosObj.gameObject.SetActive(false);
    }

    //*************************************************************************************************************

    private void CheckAllPlayerLeft()
    {
        /*if (GameManager.Instance.currentGameState == State.INPLAY)
        {
            //if (GameManager.instance.currentGameState == GameState.GAME_COMPLETED) return;

            if (PhotonNetwork.PlayerList.Length == 1)
            {
                MessageBox.Instance.Show("All players left the game.", MsgType.OKONLY);
                MessageBox.Instance.setOkText("Home");
                MessageBox.Instance.OnOkClicked += OnHomeClicked;
                GameManager.Instance.RoundOver();
                //PhotonRoomManager.instance.SetRoomClose();
            }
        }*/

        if (GameManager.Instance.currentGameState != State.WAITING && !GameManager.Instance.isMyturn)
        {
            //if (GameManager.instance.currentGameState == GameState.GAME_COMPLETED) return;

            if (PhotonNetwork.PlayerList.Length == 1)
            {
                MessageBox.Instance.Show("All players left the game.", MsgType.OKONLY);
                MessageBox.Instance.setOkText("Home");
                MessageBox.Instance.OnOkClicked += OnHomeClicked;
                GameManager.Instance.RoundOver();
                //PhotonRoomManager.instance.SetRoomClose();
            }
        }
    }

    private void OnHomeClicked()
    {
        MessageBox.Instance.OnOkClicked -= OnHomeClicked;
        PhotonRoomManager.instance.Call_LeaveRoom();
    }

    //*************************************************************************************************************
    //PHOTON EVENT ***********************************************************************************************
    //*************************************************************************************************************
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnNewPlayerJoined?.Invoke();
        if (PhotonNetwork.PlayerList.Length == GameSetting.Instance.maxPlayers)
        {
            OnAllPlayersJoined?.Invoke();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom");
        RemovePlayer(otherPlayer);
        if (GameManager.Instance.currentGameState == State.INPLAY)
        {
            if (PhotonLobbyManager.instance.IsIAmDealer())
            {
                string otherPlayerId = otherPlayer.UserId;
                string currTurnplayerID = PhotonRoomManager.instance.GetTurnId();
                Debug.Log("otherPlayerId=" + otherPlayerId);
                Debug.Log("currTurnplayerID=" + currTurnplayerID);

                if (otherPlayerId == currTurnplayerID)
                {
                    Debug.Log("true= fold");
                    OnActionClicked(ButtonActionType.FOLD);
                }
            }
        }
        CheckAllPlayerLeft();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        //Debug.Log("OnPlayerPropertiesUpdate==="+ changedProps.ContainsKey(PLAYER_SEAT_ID));


        //Recived only to me*****************************
        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if (changedProps.ContainsKey(PLAYER_STATE))
            {
                myState = (State)(int)changedProps[PLAYER_STATE];
            }
        }
        //Debug.Log("OnPlayerPropertiesUpdate");
        if (changedProps.ContainsKey(PLAYER_COIN))
        {
            double coins = (double)changedProps[PLAYER_COIN];
            //Debug.Log("coins targetPlayer name" + targetPlayer.NickName +"  == "+ coins);
        }

        //Recived TO All Players****************************
        if (changedProps.ContainsKey(PLAYER_SEAT_ID))
        {
            //Debug.Log(targetPlayer.NickName);
            int SeatId = (int)changedProps[PLAYER_SEAT_ID];
            Debug.Log(targetPlayer.NickName + " CHANGE SEAT ID=" + SeatId);
            AddAndUpdatePlayer();
            //AddPlayer(targetPlayer, SeatId);
        }

        //Recived TO All Players****************************
        if (changedProps.ContainsKey(PLAYER_STATE))
        {
            CheckAllPlayersAreReady();
        }
        //Recived TO All Players Pot COIN****************************
        if (changedProps.ContainsKey(IS_COIN_COLECTED))
        {
            CheckAllCoinPotCoinCollected();
        }

        //Update player data
        PlayerObj player = playersSittingList.Find(i => i.pPlayer == targetPlayer);
        player?.PlayerPropertiesUpdate(changedProps);
    }

    //*************************************************************************************************************

    private void CheckAllPlayersAreReady()
    {
        int count = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            State stat = (State)(int)PhotonNetwork.PlayerList[i].CustomProperties[PLAYER_STATE];
            if (stat == State.READY)
            {
                count++;
            }
        }
        if (count == PhotonNetwork.PlayerList.Length)
        {
            OnAllPlayersReady?.Invoke();
        }
        //Debug.Log("CheckAllPlayersAreReady=" + count);
    }

    private void CheckAllCoinPotCoinCollected()
    {
        Debug.Log("CheckAllCoinPotCoinCollected");
        int count = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            bool collected = (bool)PhotonNetwork.PlayerList[i].CustomProperties[IS_COIN_COLECTED];
            Debug.Log(PhotonNetwork.PlayerList[i].NickName + " = " + collected);
            //Debug.Log("i=" + collected);
            if (collected == true)
            {
                Debug.Log(PhotonNetwork.PlayerList[i].NickName + " = " + collected);
                count++;
            }
        }
        if (count == PhotonNetwork.PlayerList.Length)
        {
            OnAllPlayersCoinRecived?.Invoke();
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                PhotonNetwork.PlayerList[i].SetCustomProperties(new Hashtable() { { IS_COIN_COLECTED, false } });
            }
        }
        Debug.Log("CheckAllCoinPotCoinCollected=" + count);
    }

    private void AddAndUpdatePlayer()
    {
        for (int i = 0; i < playersSittingList.Count; i++)
        {
            playersSittingList[i].gameObject.SetActive(false);
        }

        int seatId = GetMySeatId();
        mySeatId = seatId;

        SetAllPreviousPlayerUI(seatId);
        SetAllNextPlayerUI(seatId);

        //my player
        PlayerObj p = playersSittingList[0];
        mySeatId = seatId;
        myPlayerName = GetMyName();
        myPlayer = p;
        p.pPlayer = GetMyPhotonPlayer();
        p.PlayerSeatId = seatId;
        p.gameObject.SetActive(true);
        //Debug.Log("sssss id " +p.PlayerSeatId);
        p.InitPlayer();

       

        


    }

    /*private void AddPlayer(Player pPlayer, int seatId)
    {       
        
        PlayerObj p = null;

        Debug.Log("AddPlayer seatId = " + seatId + "pPlayer name+" + pPlayer.NickName);
        if (PhotonNetwork.LocalPlayer == pPlayer)
        {
            mySeatId = seatId;
            myPlayerName = pPlayer.NickName;
            SetAllPreviousPlayerUI(mySeatId);
            p = playersSittingList[0];
            myPlayer = p;
        }
        else
        {
            p = playersSittingList.Find(i => i.gameObject.activeSelf == false);
        }
        //Debug.Log("seatId==" + seatId);
        p.pPlayer = pPlayer;
        p.PlayerSeatId = seatId;
        p.gameObject.SetActive(true);
        //Debug.Log("sssss id " +p.PlayerSeatId);
        p.InitPlayer();
    }*/

    public void RemovePlayer(Player pPlayer = null)
    {
        Debug.Log("RemovePlayer==" + pPlayer.NickName);
        if (pPlayer != null)
        {
            PlayerObj p = playersSittingList.Find(i => i.pPlayer == pPlayer);
            Debug.Log(p);
            p.gameObject?.SetActive(false);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                //PhotonNetwork.PlayerList[i].CustomProperties[PLAYER_SEAT_ID] = i;
                PhotonNetwork.PlayerList[i].SetCustomProperties(new Hashtable() { { PLAYER_SEAT_ID, i } });

                //Debug.Log(PhotonNetwork.PlayerList[i].NickName + " ReAssign id=" + i);
            }

            PhotonRoomManager.instance.RemoveOccupiedSeat();
        }
        //ReAssignSeat();
    }
    /*public void ReAssignSeat()
    {
        Debug.Log("ReAssignSeat()");
        
    }*/

    private void SetAllPreviousPlayerUI(int myPlayerId)
    {
        Debug.Log("SetAllPreviousPlayerUI = " + myPlayerId);
        for (int i = myPlayerId; i >= 1; i--)
        {
            
            Player pPlayer = PhotonNetwork.PlayerList[i - 1];
            PlayerObj p = playersSittingList[playersSittingList.Count - i];
            p.pPlayer = pPlayer;
            p.PlayerSeatId = (int)pPlayer.CustomProperties[PLAYER_SEAT_ID];
            p.gameObject.SetActive(true);
            p.InitPlayer();
        }
    }

    private void SetAllNextPlayerUI(int myPlayerId)
    {
        Debug.Log("SetAllNextPlayerUI = " + myPlayerId);
        for (int i = myPlayerId+1; i <PhotonNetwork.PlayerList.Length; i++)
        {
            Debug.Log(i);

            Player pPlayer = PhotonNetwork.PlayerList[i];
            PlayerObj p = playersSittingList[i];
            p.pPlayer = pPlayer;
            p.PlayerSeatId = (int)pPlayer.CustomProperties[PLAYER_SEAT_ID];
            p.gameObject.SetActive(true);
            p.InitPlayer();
        }
    }

    private Player GetPlayerBySeatId(int sId)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            //Debug.Log(PhotonNetwork.PlayerList[i].UserId);
            int pSid = (int)PhotonNetwork.PlayerList[i].CustomProperties[PLAYER_SEAT_ID];
            if (pSid == sId)
            {
                return PhotonNetwork.PlayerList[i];
            }
        }
        return null;
    }

    //GEETER******************************************************************************************
    public Player[] GetPlayers()
    {
        return PhotonNetwork.PlayerList;
    }

    public string GetMyId()
    {
        return PhotonNetwork.AuthValues.UserId;
    }

    public PlayerObj GetMyPlayer()
    {
        return myPlayer;
    }

    public Player GetMyPhotonPlayer()
    {
        return PhotonNetwork.LocalPlayer;
    }

    public PlayerObj GetCurrentPlayer()
    {
        return playersSittingList.Find(i => i.isMyTurn == true);
    }

    public int GetMyTurnId()
    {
        return myPlayer.PlayerSeatId;
    }

    public string GetMyUserId()
    {
        return myPlayer.GetMyId();
    }

    public string GetMyName()
    {
        return PhotonNetwork.NickName.ToUpper();
    }

    public PlayerObj GetPlayerById(int id)
    {
        return playersSittingList.Find(i => i.PlayerSeatId == id);
    }
    public int GetMySeatId()
    {
        return (int)PhotonNetwork.LocalPlayer.CustomProperties[PLAYER_SEAT_ID];
    }
    public double GetCurrentBetAmount()
    {
        return currentBetAmount;
    }

    public int GetActivePlayerCount()
    {
        List<PlayerObj> plObjs = playersSittingList.FindAll(i => i.isActive == true);
        return plObjs.Count;
    }
    public int GetTotalPlayerCount()
    {
        return PhotonNetwork.PlayerList.Length;
    }
    public int GetNextPlayerSeatId()
    {
        int totalIndex = playersSittingList.Count;

        //int startIndex = (nxtIndex > (playersSittingList.Count - 1)) ? 0 : nxtIndex;
        int index = currentPlayer==null?-1:currentPlayer.PlayerSeatId;
        //Debug.Log("before index==" + index);
        do
        {
            index++;
            //Debug.Log("Next ==" + index);
            if (index >= totalIndex)
            {
                index = 0;
                //Debug.Log("index > totalIndex ==" + index);
            }

            if (GetPlayerActiveBySeatId(index))
            {                
                //Debug.Log("GetPlayerActiveBySeatId Index ==" + index);
                return index;
            }
            if (index == currentPlayer.PlayerSeatId)
            {
                //Debug.Log("break while");
                break;
            }
        }
        while (index < totalIndex);

        //Debug.Log("return==0");
        return index;
    }

    private bool GetPlayerActiveBySeatId(int seatID)
    {
        PlayerObj plObjs = playersSittingList.Find(i => i.PlayerSeatId == seatID);
        //Debug.Log("PlayerObj==" + plObjs);
        if (plObjs != null)
        {
            //Debug.Log("seatID activeSelf== " + seatID +" = "+ plObjs.gameObject.activeSelf);
            //Debug.Log("seatID isActive== " + seatID +" = "+ plObjs.isActive);
            if (!plObjs.gameObject.activeSelf || !plObjs.isActive)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    /*int GetFromStartActivePlayerId()
    {
        for (int i = 0; i < playersSittingList.Count; i++)
        {
            if (playersSittingList[i].gameObject.activeSelf)
            {
                if (playersSittingList[i].isActive)
                {
                    return playersSittingList[i].PlayerSeatId;
                }
            }
        }
        return 0;
    }*/

    public string GetUserList()
    {
        string str = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (i + 1 >= PhotonNetwork.PlayerList.Length)
            {
                str += PhotonNetwork.PlayerList[i].UserId;
            }
            else
                str += PhotonNetwork.PlayerList[i].UserId + ",";
        }
        return str;
    }

    //SETTER***************************************************************************************
    public void SetSittingPlayerList(List<PlayerObj> pListObj)
    {
        playersSittingList = new List<PlayerObj>();
        for (int i = 0; i < pListObj.Count; i++)
        {
            playersSittingList.Add(pListObj[i]);
        }
    }

    public void SetState(State currentGameState)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { PLAYER_STATE, (int)currentGameState } });
    }

    public void SetSeatId(Player p, int seatId)
    {
        p.SetCustomProperties(new Hashtable() { { PLAYER_SEAT_ID, seatId } });
    }

    public void SetTurn(int tId)
    {
        currentPlayer?.UpdateRoundTimer(0);
        currentBetAmount = PhotonRoomManager.instance.GetMinimumBetAmount();
        for (int i = 0; i < playersSittingList.Count; i++)
        {
            playersSittingList[i].SetTurn(false);
        }
        currentPlayer = GetPlayerById(tId);
        currentPlayer.SetTurn(true);
        Debug.Log("====Current player id===" + currentPlayer.GetMyId());
        if (GameManager.Instance.isMyturn)
        {
            PhotonRoomManager.instance.SetTurnId(currentPlayer.GetMyId());
        }
    }

    private void SetAmoutDeductedStatus()
    {
        Debug.Log("SetAmoutDeductedStatus");
        if (GameManager.Instance.currentGameState == State.POT_COLLECTION)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { IS_COIN_COLECTED, true } });
        }
    }

    public void SetUpdatedWalletBalance()
    {
        //Debug.Log("OnWalletUpated");
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { PLAYER_COIN, WalletManager.instance.GetTempWalletAmount() } });
    }

    //setter

    /*public void SetMyTurnActive(bool _isActive)
    {
        Hashtable hashtable = new Hashtable();
        hashtable["IsMyTurnActive"] = _isActive;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }*/

    //****************************************************************
}