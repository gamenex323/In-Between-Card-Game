using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public static GameSetting Instance;
    public double MinimumBetAmount = 10;
    public double TempWalletAmount = 10;
    //public double MinimumWalletAmount = 20;

    public double PlayerWaitingTime = 30f;// 30sec
    public double PlayerTime = 30f;// 30sec
    public byte maxPlayers = 6;
    public float cardTransistionGap = 0.5f;
    public float cardFlipTime = 0.3f;
    public float cardDealTime = 1f;
    public float cardShuffleTime = 5f;
    public float potCollectionSpeed = 1f;
    public float betSpeed = 0.5f;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);

        
    }

}
