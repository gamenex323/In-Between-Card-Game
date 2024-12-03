using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : ExtendedBehavior
{
    [SerializeField] GameScreen gameScreen;



    private double potAmount;

    //bool isGamePause;
    private int currentTurn;

    public bool isMyturn;

    //public PlayerObj currentPlayer;
    public State currentGameState;

    public GameMode currentGameMode;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
                Destroy(gameObject);
        }
    }
    bool isRecollection = false;
    bool isLastRound = false;
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        HomeScreen.SetGameMode += SetGameMode;
        GameScreen.OnStartWaitingTimer += OnStartWaitingTimer;
        PhotonWaitingTimerManager.OnWaitingTimeFinished += StartGame;

        PhotonPlayerManager.OnAllPlayersJoined += OnAllPlayerJoined;
        PhotonPlayerManager.OnAllPlayersReady += OnAllPlayersReady;
        PhotonPlayerManager.OnAllPlayersCoinRecived += OnAllPlayersCoinRecived;
        PhotonPlayerManager.OnCenterCardFliped += CheckResult;
        PhotonPlayerManager.OnPlayerWinLossEventCompleted += OnPlayerWinLossEventCompleted;

        PhotonSenderReciver.OnRecivedNewCardDeck += OnRecivedNewCardDeck;
        PhotonSenderReciver.OnRecivedTurnId += OnRecivedTurnId;
        PhotonSenderReciver.OnRecivedButtonAction += OnRecivedButtonAction;
        PhotonSenderReciver.OnRecivedCurrentPlayerWinLose += OnRecivedCurrentPlayerWinLose;

        CardDeckManager.OnTwoCardFliped += OnTwoCardFliped;

        NextGameWaiting.OnCountdownFinished += OnCountdownFinished;
    }

    private void OnDestroy()
    {
        HomeScreen.SetGameMode -= SetGameMode;
        GameScreen.OnStartWaitingTimer -= OnStartWaitingTimer;
        PhotonWaitingTimerManager.OnWaitingTimeFinished -= StartGame;

        PhotonPlayerManager.OnAllPlayersJoined -= OnAllPlayerJoined;
        PhotonPlayerManager.OnAllPlayersReady -= OnAllPlayersReady;
        PhotonPlayerManager.OnAllPlayersCoinRecived -= OnAllPlayersCoinRecived;
        PhotonPlayerManager.OnCenterCardFliped -= CheckResult;
        PhotonPlayerManager.OnPlayerWinLossEventCompleted -= OnPlayerWinLossEventCompleted;

        PhotonSenderReciver.OnRecivedNewCardDeck -= OnRecivedNewCardDeck;
        PhotonSenderReciver.OnRecivedTurnId -= OnRecivedTurnId;
        PhotonSenderReciver.OnRecivedButtonAction -= OnRecivedButtonAction;
        PhotonSenderReciver.OnRecivedCurrentPlayerWinLose -= OnRecivedCurrentPlayerWinLose;

        CardDeckManager.OnTwoCardFliped -= OnTwoCardFliped;
        NextGameWaiting.OnCountdownFinished -= OnCountdownFinished;
    }

    /******************************************************************************************************/
    /*GAME EVENTS *********************************************************************************************/
    /******************************************************************************************************/

    private void OnStartWaitingTimer()
    {
        ResetGame();
        SetCurrentState(State.WAITING);
        PotManager.Instance.ResetPot();
        PhotonPlayerManager.instance.InitPlayer();

        if (currentGameMode == GameMode.PLAY_ONLINE)
        {
            PhotonWaitingTimerManager.instance.StartRoundTimer();
            PopupsManager.Instance.Show("PlayerWaiting");
        }
        else
        {
            PopupsManager.Instance.Show("PlayerWaitingForFriends");
        }
    }
    
    private void OnAllPlayerJoined()
    {
        PhotonWaitingTimerManager.instance.StopRoundTimer();
        //Set Deck==================================
        InitGame();
    }

    public void StartGame()
    {
        isLastRound = false;
        SetCurrentState(State.WAITFORSTART);
        //PhotonPlayerManager.instance.RemovePlayer();
        PhotonRoomManager.instance.SetRoomClose();
        //PhotonPlayerManager.instance.ReAssignSeat();
        Wait(2, () => { 
        
            //GetComponent<GameScreen>().ShowBackBtn();

            Debug.Log("StartGame");
            PopupsManager.Instance.Hide();
            if (PhotonPlayerManager.instance.GetPlayers().Length >= 2)
            {
                InitGame();
            }
            else
            {
                PopupsManager.Instance.Hide();
                MessageBox.Instance.Show("Players not found for match making..", MsgType.OKONLY);
                MessageBox.Instance.setOkText("Home");
                MessageBox.Instance.OnOkClicked += () => { MessageBox.Instance.OnOkClicked = null; PhotonRoomManager.instance.Call_LeaveRoom(); };
                PhotonRoomManager.instance.SetRoomClose();
            }
        });
    }

    private void InitGame(bool recollect = false)
    {
        if (recollect)
        {
            if (PhotonPlayerManager.instance.GetActivePlayerCount() == 1)
            {
                ShowPopup();
                return;
            }
        }

        gameScreen.ShowBackBtn();
        isRecollection = recollect;
        Debug.Log("InitGame");
        PhotonRoomManager.instance.SetRoomClose();
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            string userList = PhotonPlayerManager.instance.GetUserList();

            string modeName = currentGameMode.ToString();

            Debug.Log(modeName + "---------" + userList);
            CreateTable(modeName, userList);

            ////should call on success
            //string generateNewCardDeckIds = GetNewShuffleDeck();
            //PhotonRoomManager.instance.SetRandomDeck(generateNewCardDeckIds);
            //PhotonSenderReciver.instance.CreateNewCardDeck(generateNewCardDeckIds);
        }

        //Set Deck==================================
        if(!isRecollection)
        SetCurrentState(State.INIT);
    }

    private void OnRecivedNewCardDeck(string newDeck)
    {
        gameScreen.ShowBackBtn();
        PopupsManager.Instance.Hide();
        CardDeckManager.Instance.InitCard(newDeck);
    }

    public void IamReady()
    {
        SetCurrentState(State.READY);
    }

    private void OnAllPlayersReady()
    {
        Debug.Log("OnAllPlayersReady");
        SetCurrentState(State.POT_COLLECTION);
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            CollectPotAmount();
        }
    }

    private void CollectPotAmount()
    {
        Debug.Log("CollectPotAmount");
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            double amt = PhotonRoomManager.instance.GetMinimumBetAmount();
            PhotonSenderReciver.instance.CollectPotAmountRequest(amt);
        }
    }

    private void OnAllPlayersCoinRecived()
    {
        SetCurrentState(State.INPLAY);
        Debug.Log("OnAllPlayersCoinRecived");
        /*if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            PhotonSenderReciver.instance.PlayPotCollectionAnimation
        }*/

        PhotonPlayerManager.instance.PlayPotCollectionAnimation(OnPotcollectionAnimationCompleted);
    }

    private void OnPotcollectionAnimationCompleted()
    {
        Debug.Log("OnPotcollectionAnimationCompleted");
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            //need to work
            //int tId = PhotonRoomManager.instance.GetTurnId();
            //Start from 0
            int nextPlayerTurnId = PhotonPlayerManager.instance.GetNextPlayerSeatId();
            //Debug.Log("nextPlayerTurnId================" + nextPlayerTurnId);
            PhotonSenderReciver.instance.SetTurn(nextPlayerTurnId);
            //PhotonSenderReciver.instance.SetTurn(0);
        }
    }

    private void OnTwoCardFliped()
    {
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            PhotonRoundTimerManager.instance.StartRoundTimer();
        }
        if (isMyturn)
        {
            if (IsSequenceCard()) {
                Wait(1, () =>
                {
                    PhotonPlayerManager.instance.OnActionClicked(ButtonActionType.DRAW);

                });
                
            }
            else if (IsSameCard())
            {
                Wait(1, () =>
                {
                    PhotonPlayerManager.instance.OnActionClicked(ButtonActionType.DRAW);
                });
            }
            else
            {
                PhotonPlayerManager.instance.ActionButtonEnble();
            }
            
        }
    }

    //Recived from Photon********************************************************

    private void OnRecivedAllPotAmount()
    {
    }

    private void OnRecivedTurnId(int tId)
    {
        Debug.Log("OnRecivedTurnId = " + tId);
        currentTurn = tId;
        isMyturn = (currentTurn == PhotonPlayerManager.instance.GetMyTurnId()) ? true : false;

        PhotonPlayerManager.instance.SetTurn(tId);

        //currentPlayer.SetTimer(true);

        CardDeckManager.Instance.MoveThreeCard();
    }

    private void OnRecivedButtonAction(ButtonActionType type, double amt)
    {
        switch (type)
        {
            case ButtonActionType.FOLD:
                CardDeckManager.Instance.RemoveCard(OnCardRemoved);
                break;

            case ButtonActionType.DRAW:
            case ButtonActionType.MAXBET:
                PhotonPlayerManager.instance.PlayBetAnimation(amt);

                break;
        }
    }

    private void OnRecivedCurrentPlayerWinLose(bool isWin)
    {
        PhotonPlayerManager.instance.ShowWinLoseAnimation(isWin);
    }

    private void OnCardRemoved()
    {
        if (currentGameState != State.INPLAY) return;
        StartCoroutine(CheckForNetPlayerTurn());
    }

    /******************************************************************************************************/
    /*GAME LOGIC *********************************************************************************************/
    /******************************************************************************************************/
    bool IsSequenceCard()
    {
        bool isSequenceCard = false;

        List<CardsData> cardsData = CardDeckManager.Instance.GetTopThreeCardData();
        int leftCardRank = (int)cardsData[0].Value;
        //int centerCardRank = (int)cardsData[1].Value;
        int rightCardRank = (int)cardsData[2].Value;              
       
         if (Mathf.Abs(leftCardRank - rightCardRank) == 1)
        {
            isSequenceCard = true;
        }

        return isSequenceCard;
    }
    bool IsSameCard()
    {
        bool isSameCard = false;

        List<CardsData> cardsData = CardDeckManager.Instance.GetTopThreeCardData();
        int leftCardRank = (int)cardsData[0].Value;
        //int centerCardRank = (int)cardsData[1].Value;
        int rightCardRank = (int)cardsData[2].Value;

        if (leftCardRank == rightCardRank)
        {
            isSameCard = true;
        }
        return isSameCard;
    }
    private void CheckResult()
    {
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            List<CardsData> cardsData = CardDeckManager.Instance.GetTopThreeCardData();
            int leftCardRank = (int)cardsData[0].Value;
            int centerCardRank = (int)cardsData[1].Value;
            int rightCardRank = (int)cardsData[2].Value;

            bool iswin = false;
            bool isLeftGraterThenRight = leftCardRank > rightCardRank;

            if (leftCardRank == rightCardRank)
            {
                //same card true
                iswin = true;
            }
            else if (Mathf.Abs(leftCardRank - rightCardRank) == 1)
            {
                iswin = false;
            }
            else if (isLeftGraterThenRight)
            {
                if ((leftCardRank > centerCardRank) && (centerCardRank > rightCardRank))
                {
                    iswin = true;
                }
            }
            else if (!isLeftGraterThenRight)
            {
                if ((leftCardRank < centerCardRank) && centerCardRank < rightCardRank)
                {
                    iswin = true;
                }
            }

            /*
                winning condition

            */

            PhotonSenderReciver.instance.SendCurrentPlayerWinLose(iswin);

            //deal result API
            //if ismyturn {}
        }
    }

    private void OnPlayerWinLossEventCompleted()
    {
        Debug.Log("OnPlayerWinLossEventCompleted");
        CardDeckManager.Instance.RemoveCard(() => { StartCoroutine(CheckForNetPlayerTurn()); });
    }
    
    private IEnumerator CheckForNetPlayerTurn()
    {
        if (currentGameState == State.INPLAY)
        {
            //if (GameManager.instance.currentGameState == GameState.GAME_COMPLETED) return;

            if (PhotonNetwork.PlayerList.Length == 1)
            {
                MessageBox.Instance.Show("All players left the game.", MsgType.OKONLY);
                MessageBox.Instance.setOkText("Home");
                MessageBox.Instance.OnOkClicked += OnHomeClicked;
                RoundOver();
                yield return null;
                //PhotonRoomManager.instance.SetRoomClose();
            }
        }

        //app player wallet balance check and out player
        PhotonPlayerManager.instance.CheckSetPlayersEnableByAvailableCoins();
        yield return new WaitForSeconds(0.02f);
        //Debug.Log("PhotonPlayerManager.instance.GetActivePlayerCount()===" + PhotonPlayerManager.instance.GetActivePlayerCount());

        /*if (PhotonPlayerManager.instance.GetActivePlayerCount() == 1)
        {
            //Game over message
            MessageBox.Instance.Show("Not enough players left to play the game.", MsgType.OKONLY);
            MessageBox.Instance.setOkText("Home");
            MessageBox.Instance.OnOkClicked += OnHomeClicked;
            RoundOver();
            yield return null;
        }*/

        yield return new WaitForSeconds(0.05f);
        if (CardDeckManager.Instance.GetRemainCardCount() < 3)
        {
            Debug.Log("Not enough Card to play the game.");
            //MessageBox.Instance.Show("GAME OVER\n Not enough Card to play the game.", MsgType.OKONLY);
            //MessageBox.Instance.setOkText("Home");
            //MessageBox.Instance.OnOkClicked += OnHomeClicked;

            if (PhotonLobbyManager.instance.IsIAmDealer())
            {
                string generateNewCardDeckIds = GetNewShuffleDeck();
                PhotonRoomManager.instance.SetRandomDeck(generateNewCardDeckIds);
                PhotonSenderReciver.instance.CreateNewCardDeck(generateNewCardDeckIds);
            }
        }
        else if (PotManager.Instance.GetPotAmount() < PhotonRoomManager.instance.GetMinimumBetAmount())
        {
            PopupsManager.Instance.Show("NextGameWaiting");
            //MessageBox.Instance.Show("GAME OVER\n Not enough Pot amount to play the game.", MsgType.OKONLY);
            //MessageBox.Instance.setOkText("Home");
            //MessageBox.Instance.OnOkClicked += OnHomeClicked;

            RoundOver();
        }
        else
        {
            if(isLastRound)
            {
                //Game over message

                ShowPopup();
                RoundOver();
                yield return null;
            }

            if (PhotonPlayerManager.instance.GetActivePlayerCount() == 1)
            {
                isLastRound = true;
            }
            else
            {
                isLastRound = false;
            }

            if (PhotonLobbyManager.instance.IsIAmDealer())
            {
                
                //Next Player turn
                int nextPlayerTurnId = PhotonPlayerManager.instance.GetNextPlayerSeatId();
                Debug.Log("===Generated Next Player Id==" + nextPlayerTurnId);
                PhotonSenderReciver.instance.SetTurn(nextPlayerTurnId);
            }
        }
    }
    void ShowPopup()
    {
        if (PhotonPlayerManager.instance.GetTotalPlayerCount() == 2)
        {
            MessageBox.Instance.Show("Not enough Active players left to play the game.", MsgType.OKONLY);
            MessageBox.Instance.setOkText("Home");
            MessageBox.Instance.OnOkClicked += OnHomeClicked;
        }
        else
        {
            MessageBox.Instance.Show("Not enough players left to play the game.", MsgType.OKONLY);
            MessageBox.Instance.setOkText("Home");
            MessageBox.Instance.OnOkClicked += OnHomeClicked;
        }
    }
    public void ReShuffleCompleted()
    {
        Wait(0.5f, () =>
        {
            if (PhotonLobbyManager.instance.IsIAmDealer())
            {
                //Next Player turn
                int nextPlayerTurnId = PhotonPlayerManager.instance.GetNextPlayerSeatId();
                PhotonSenderReciver.instance.SetTurn(nextPlayerTurnId);
            }
        });
    }

    void OnCountdownFinished()
    {
        PopupsManager.Instance.Hide();
        SetCurrentState(State.POT_COLLECTION);
         
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            Debug.Log("OnCountdownFinished");
            InitGame(true);
        }
    }

    //public void EndGame(bool isForced = false)
    //{
    //Debug.Log("End game callded-----------------------");
    //InRoomRoundTimer.instance.StopRoundTimer();
    // SetGameState(State.FINISHED);

    // UIManager.OnGameFinished(activePlayer.team.ToString());
    /*if (!isForced)
    {
        PopupsManager.Instance.Show("WinLosePopup");
    }*/
    //Notification.Instance.Hide();
    //StopAllCoroutines();

    //PlayerPrefs.SetString("LeftRoomId", "");
    //PhotonRoomManager.instance.IsLastConnectedAndJoined = false;
    //ResetGame();

    //SetGameState(State.WAITING);

    //isGamePause = false;

    //PhotonRoomManager.instance.SetRoomClose();
    // }

    public void RoundOver()
    {
        SetGameState(State.FINISHED);
        PhotonRoundTimerManager.instance.StopRoundTimer();
    }

    private void ResetGame()
    {
        currentGameState = State.WAITING;
        CardDeckManager.Instance.ResetCards();
    }

    private void OnHomeClicked()
    {
        MessageBox.Instance.OnOkClicked -= OnHomeClicked;
        PhotonRoomManager.instance.Call_LeaveRoom();
    }

    /******************************************************************************************************/
    /*SETTER *********************************************************************************************/
    /******************************************************************************************************/

    public void SetGameMode(GameMode gm)
    {
        currentGameMode = gm;
    }

    public void SetGameState(State gs)
    {
        currentGameState = gs;
    }

    private void SetCurrentState(State state)
    {
        currentGameState = state;
        PhotonPlayerManager.instance.SetState(currentGameState);
    }

    /******************************************************************************************************/
    /*GETTER *********************************************************************************************/
    /******************************************************************************************************/

    public string GetNewShuffleDeck()
    {
        //(int count, int minValue, int maxValue)
        List<int> randomList = Utility.GenerateRandomNumbers(CardDeckManager.Instance.GetCardData().Count, 0, CardDeckManager.Instance.GetCardData().Count);
        return Utility.IntListToString(randomList);
    }

    public bool IsGameOn()
    {
        return false;
    }

    public bool IsOnlinePlayer()
    {
        return (currentGameMode == GameMode.PLAY_ONLINE);
    }

    /******************************************************************************************************/
    /*Create Table API *********************************************************************************************/
    /******************************************************************************************************/

    public void CreateTable(string table_type, string user_list)
    {
        Debug.Log("table_type=" + table_type);
        Debug.Log("user_list=" + user_list);
        WWWForm formData = new WWWForm();
        formData.AddField("table_type", table_type);
        formData.AddField("user_list", user_list);
        //Debug.Log("create table api" + formData.data);
        Server.Instance.ApiRequest(typeof(CreateTable), OnTableCreated, formData, false);
    }

    private void OnTableCreated(JSONObject data)
    {
        CreateTableRoot createTableRoot = JsonUtility.FromJson<CreateTableRoot>(data.Print());
        createTableRoot.success = true;
        if (createTableRoot.success)
        {
            //DataManager.Instance.CreateTableData = createTableRoot;
            Debug.Log(createTableRoot.tableCreated.table_id);
            PhotonRoomManager.instance.SetTableId(createTableRoot.tableCreated.table_id);
            Debug.Log("table created");


            if (isRecollection)
            {
                CollectPotAmount();
            }
            else
            {
                string generateNewCardDeckIds = GetNewShuffleDeck();
                PhotonRoomManager.instance.SetRandomDeck(generateNewCardDeckIds);
                PhotonSenderReciver.instance.CreateNewCardDeck(generateNewCardDeckIds);
            }
        }
        else
        {
            Server._LoadingEnd();
            MessageBox.Instance.Show(createTableRoot.message);
        }
    }
}

public enum State
{
    WAITING, INIT, READY, RE_SHUFFLE, POT_COLLECTION, INPLAY, FINISHED, WAITFORSTART
}

public enum GameMode
{
    PLAY_ONLINE, PLAY_WITH_FRIEND
}