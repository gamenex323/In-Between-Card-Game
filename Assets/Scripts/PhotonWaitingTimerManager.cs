using Photon.Pun;
using System;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class PhotonWaitingTimerManager : MonoBehaviourPunCallbacks
{
     
    [SerializeField] Text countDownTxt;
    public static Action OnWaitingTimeFinished;    
  
    bool startTimer = false;

    double timer = 20;
    double startTime;
    double timerIncrementValue;

    const string START_TIME = "start_time";
    const string START_STOP_COMAND = "start_stop_comand";
    public static PhotonWaitingTimerManager instance;
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
    //*********************************************************************************************************
    public void StartRoundTimer()
    {
        timer = PhotonRoomManager.instance.GetWaitingTime();

        startTimer = true;
        startTime = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            startTime = PhotonNetwork.Time;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {
                {START_TIME, startTime},
                {START_STOP_COMAND, startTimer}

            });
            Debug.Log("StartRoundTimer if==" + startTime);
        }
        else
        {
           
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties[START_TIME].ToString());
            startTimer = (bool)PhotonNetwork.CurrentRoom.CustomProperties[START_STOP_COMAND];
            Debug.Log("StartRoundTimer else==" + startTime);
            Debug.Log("StartRoundTimer else startTimer==" + startTimer);
        }
    }

    public void StopRoundTimer()
    {
        startTimer = false;

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            startTime = PhotonNetwork.Time;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {
                {START_STOP_COMAND, startTimer}

            });
        }
        else
        {   //if (PhotonNetwork.InRoom && GameManager.Instance.currentGameState == State.WAITING)
                startTimer = false;//(bool)PhotonNetwork.CurrentRoom.CustomProperties[START_STOP_COMAND];
        }
    }
    public void StopMyRoundTimer()
    {
        startTimer = false;
    }

    private void Update()
    {
        if (!startTimer) return;
        
        timerIncrementValue = PhotonNetwork.Time - startTime;

        string digit = ((int)(timer - timerIncrementValue)).ToString();
        countDownTxt.text = "Waiting for player(" + digit + ")...";
       
        //Debug.Log(timer);
        if (timerIncrementValue >= timer)
        {
            StopRoundTimer();
            //Timer Completed
           // Debug.Log("OnWaitingTimeFinished");
            OnWaitingTimeFinished?.Invoke();
        }
    }

    

}
