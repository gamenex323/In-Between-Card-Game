using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;

//using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObj : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Player pPlayer;

    [SerializeField]
    private string playerId, pName;

    public int PlayerSeatId = -1;//no ant set assign coz seat id is strat fron 0

    [SerializeField] private Text m_playerNameTxt;
    [SerializeField] private Image m_ProfilePic;
    [SerializeField] private Image m_TurnTimer;
    [SerializeField] private Text m_TotalCoinsTxt;
    public Transform potPos, betPos;
    public Transform betObj;
    [SerializeField] private GameObject winnerAnimation;
    [SerializeField] private CanvasGroup cg;

    [SerializeField] private State state;
    public bool isMyTurn;
    public bool isActive;

    private void Start()
    {
        PhotonRoundTimerManager.OnRoundTimeUpdate += OnRoundTimeUpdate;
        //SetEnableDisable(false);
    }

    private void OnEnable()
    {
        UpdateCoin();
        //SetTurn(false);
    }

    private void OnDisable()
    {
        PlayerSeatId = -1;
        SetEnableDisable(false);
        m_ProfilePic.gameObject.SetActive(false);
        UpdateRoundTimer(0);
    }

    private void OnDestroy()
    {
        PhotonRoundTimerManager.OnRoundTimeUpdate -= OnRoundTimeUpdate;
    }

    //*************************************************************************************************************
    //PLAYER EVENT ***********************************************************************************************
    //*************************************************************************************************************
    public void InitPlayer()
    {
        m_playerNameTxt.text = GetMyName();
        playerId = GetMyId();
        m_ProfilePic.gameObject.SetActive(false);
        pName = pPlayer.NickName;
        SetEnableDisable(true);
        string url = (string)pPlayer.CustomProperties[PhotonPlayerManager.PLAYER_IMAGE_URL];
        if (url.Trim() != "")
        {
            //Debug.Log(url.Trim());
            LoadImage(url, OnImageLoaded);
        }
        UpdateCoin();
    }

    private void OnImageLoaded(Sprite obj)
    {
        m_ProfilePic.transform.GetChild(0).GetComponent<Image>().sprite = obj;
        m_ProfilePic.gameObject.SetActive(true);
    }

    public void SetEnableDisable(bool _enable)
    {
        //Debug.Log("Player id=" + GetMyId() + " SetEnableDisable=" + _enable);
        isActive = _enable;
        if (isActive)
        {
            cg.alpha = 1f;
            cg.interactable = true;
        }
        else
        {
            cg.alpha = .5f;
            cg.interactable = false;
        }
    }

    private void OnRoundTimeUpdate(float t)
    {
        //Debug.Log("OnRoundTimeUpdate");
        if (isMyTurn)
        {
            double per = (t) / PhotonRoomManager.instance.GetRoundTime();
            UpdateRoundTimer(per);
        }
    }

    //ANIMATION********************************************************************************************
    //COLLECT*********************************************************
    public void PlayCoinPotCollectionAnimation(Action _callback)
    {
        if (!isActive) return;
        betObj.gameObject.SetActive(true);
        betObj.GetComponentInChildren<Text>().text = PhotonRoomManager.instance.GetMinimumBetAmount().ToString();
        betObj.DOMove(transform.position, 0);
        betObj.DOMove(potPos.position, GameSetting.Instance.potCollectionSpeed).OnComplete(() => { if (IsMyPlayer()) { _callback(); }; betObj.gameObject.SetActive(false); });
    }

    //BET*********************************************************
    public void PlayCoinBetAnimation(double amt, Action _callback)
    {
        betObj.gameObject.SetActive(true);
        betObj.GetComponentInChildren<Text>().text = Utility.AbbreviateNumber((float)amt);
        betObj.DOMove(transform.position, 0);
        betObj.DOMove(betPos.position, GameSetting.Instance.betSpeed).OnComplete(() => { _callback(); });
    }

    //WIN*********************************************************
    public void PlayCoinWinCollectAnimation(double amt, Action _callback)
    {
        betObj.gameObject.SetActive(true);
        betObj.GetComponentInChildren<Text>().text = Utility.AbbreviateNumber((float)amt);
        //betObj.DOMove(transform.position, 0);
        betObj.DOMove(transform.position, GameSetting.Instance.betSpeed).OnComplete(() => { _callback(); betObj.gameObject.SetActive(false); });
    }

    //LOSS*********************************************************
    public void PlayCoinLossAnimation(double amt, Action _callback)
    {
        //Debug.LogError("PlayCoinLossAnimation=" + amt);
        betObj.gameObject.SetActive(true);
        betObj.GetComponentInChildren<Text>().text = Utility.AbbreviateNumber((float)amt);
        //betObj.DOMove(transform.position, 0);
        betObj.DOMove(potPos.position, GameSetting.Instance.betSpeed).OnComplete(() => { _callback(); betObj.gameObject.SetActive(false); });
    }

    private Action callback;

    public void PlayWinLoseAnimation(Action _callback = null)
    {
        winnerAnimation.SetActive(true);
        callback = _callback;
    }

    public void OnWinAnimationCompleted()
    {
        callback?.Invoke();
    }

    public void ResetPlayer()
    {
    }

    public void CheckCoinsToPlay()
    {
        if (GetMyCoin() >= (PhotonRoomManager.instance.GetMinimumBetAmount()))
        {
            SetEnableDisable(true);
        }
        else
        {
            SetEnableDisable(false);
        }
    }

    //*************************************************************************************************************
    //PHOTON EVENT ***********************************************************************************************
    //*************************************************************************************************************
    public void PlayerPropertiesUpdate(Hashtable changedProps)
    {
        if (changedProps.ContainsKey(PhotonPlayerManager.PLAYER_STATE))
        {
            state = (State)(int)changedProps[PhotonPlayerManager.PLAYER_STATE];
        }

        if (changedProps.ContainsKey(PhotonPlayerManager.PLAYER_COIN))
        {
            Debug.Log(changedProps.ContainsKey(PhotonPlayerManager.PLAYER_COIN));

            UpdateCoin();
        }

       /* if (changedProps.ContainsKey(PhotonPlayerManager.PLAYER_SEAT_ID))
        {
            PlayerSeatId = (int)changedProps[PhotonPlayerManager.PLAYER_SEAT_ID];
           
        }*/
    }

    //*************************************************************************************************************
    //SETTER ***********************************************************************************************
    //*************************************************************************************************************

    public void SetTurn(bool _value)
    {
        isMyTurn = _value;
    }

    public void UpdateRoundTimer(double _value)
    {
        m_TurnTimer.fillAmount = (float)_value;
    }

    //*************************************************************************************************************
    //GETTER ***********************************************************************************************
    //*************************************************************************************************************

    public string GetMyName()
    {
        return pPlayer.NickName;
    }

    public string GetMyId()
    {
        return PhotonNetwork.AuthValues.UserId;
    }

    public bool IsMyPlayer()
    {
        return PhotonNetwork.LocalPlayer == pPlayer;
    }

    public double GetMyCoin()
    {
        return (double)pPlayer.CustomProperties[PhotonPlayerManager.PLAYER_COIN];
    }

    //*************************************************************************************************************
    //UPDATE ***********************************************************************************************
    //*************************************************************************************************************
    private void UpdateCoin()
    {
        if (pPlayer.CustomProperties.ContainsKey(PhotonPlayerManager.PLAYER_COIN))
        {
            double amt = ((double)pPlayer.CustomProperties[PhotonPlayerManager.PLAYER_COIN]);
            m_TotalCoinsTxt.text = Utility.AbbreviateNumber((float)amt);
        }
    }

    //*************************************************************************************************************
    //Image Load ***********************************************************************************************
    //*************************************************************************************************************
    public void LoadImage(string url, Action<Sprite> sprite)
    {
        StartCoroutine(Load(url, sprite));
    }

    private System.Collections.IEnumerator Load(string url, Action<Sprite> callback)
    {
        //Server._LoadingStart();
        WWW www = new WWW(url);
        yield return www;

        Texture2D texture = www.texture;
        //Debug.Log(url);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        callback(sprite);
        www.Dispose();
        www = null;
        //Server._LoadingEnd();
    }
}