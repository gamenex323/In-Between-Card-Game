using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextGameWaiting : MonoBehaviour
{
    public static Action OnCountdownFinished;
    [SerializeField] Text countDownTxt;
     float timeRemaining;
    bool isTimerStart= false;
    private void OnEnable()
    {
        
        StartCountDown();
    }

    void StartCountDown()
    {
        timeRemaining = 5;
        isTimerStart = true;
    }

    private void Update()
    {
        if (!isTimerStart) return;
        timeRemaining -= Time.deltaTime;
        if (timeRemaining > 0)
        {
            countDownTxt.text = "Game start in " + (int)timeRemaining + " sec";
        }
        else
        {
            isTimerStart = false;
            OnCountdownFinished?.Invoke();
            //countDownTxt.text = "TIME'S UP!";

        }
    }
}
