
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonRoundTimerManager : MonoBehaviourPunCallbacks
{
    public static Action OnRoundTimeFinished;
    public static Action<float> OnRoundTimeUpdate;

    bool startTimer = false;

    double timer = 20;
    double startTime;
    double timerIncrementValue;

    const string START_ROUND_TIME = "start_Round_time";
    const string START_STOP_ROUND_COMAND = "start_stop_Round_comand";
    public static PhotonRoundTimerManager instance;
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
        timer = PhotonRoomManager.instance.GetRoundTime();

        startTimer = true;

        if (PhotonNetwork.IsMasterClient)
        {
            startTime = PhotonNetwork.Time + PhotonRoomManager.instance.GetRoundTime(); 
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {
                {START_ROUND_TIME, startTime},
                {START_STOP_ROUND_COMAND, startTimer}

            });
        }
        
    }

    public void StopRoundTimer()
    {
        Debug.Log("StopRoundTimer()");
        startTimer = false;

       if (PhotonNetwork.InRoom)
       {
            startTime = PhotonNetwork.Time;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable() {
                {START_STOP_ROUND_COMAND, startTimer}

            });
       }
        
    }


    private void Update()
    {

        if (!startTimer) return;

        //timerIncrementValue = PhotonNetwork.Time - startTime;
        //float digit = (float)(timer - timerIncrementValue);

        //Debug.Log(SecondsUntilItsTime + "==="+PhotonNetwork.Time + " === " +  " === " + startTime); 

        //OnRoundTimeUpdate?.Invoke((float)SecondsUntilItsTime);

        /*if (this.startTime > 0.001f)
         {
             StopRoundTimer();            
             OnRoundTimeFinished?.Invoke();
         }*/

        if (this.IsItTimeYet)
        {
            OnRoundTimeFinished?.Invoke();
            StopRoundTimer();
        }


        //Debug.Log("isStartTimer=" + isStartTimer);
        if (!this.IsItTimeYet && startTimer)
        {
           
             OnRoundTimeUpdate?.Invoke((float)this.SecondsUntilItsTime);

           
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        //Debug.Log("OnRoomPropertiesUpdate");
        if (propertiesThatChanged.ContainsKey(START_STOP_ROUND_COMAND))
        {
            this.startTimer = (bool)propertiesThatChanged[START_STOP_ROUND_COMAND];
            //Debug.Log("startTimer==" + startTimer);
        }

        if (propertiesThatChanged.ContainsKey(START_ROUND_TIME))
        {
            this.startTime = (double)propertiesThatChanged[START_ROUND_TIME];
            //Debug.Log("startTime==" + startTime);

        }
    }

    public bool IsItTimeYet
    {
        get { return IsTimeToStartKnown && PhotonNetwork.Time > this.startTime; }
    }
    public double SecondsUntilItsTime
    {
        get
        {
            if (this.startTime > 0.001f)
            {
                double delta = this.startTime - PhotonNetwork.Time;
                return (delta > 0.0f) ? delta : 0.0f;
            }
            else
            {
                return 0.0f;
            }
        }
    }
    public bool IsTimeToStartKnown
    {
        get { return this.startTime > 0.001f; }
    }
}

