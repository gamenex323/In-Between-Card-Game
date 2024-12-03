using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PotManager : MonoBehaviour
{
    [SerializeField] Transform betObj, potPos, betPos;
    public double totalPotAmount;
    public Text Text;
   
    public string NumberFormat = "N0";
        
    public static PotManager Instance;
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


    //********************************************************************************************
    public void CollectPotAmount()
    {
        double value = (double)(PhotonRoomManager.instance.GetMinimumBetAmount() * PhotonPlayerManager.instance.GetPlayers().Length);
                
        UpdateText((int)value);
        totalPotAmount = value;
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            PhotonRoomManager.instance.SetPotAmount(value);
        }
    }

    public void AddPotAmount(double amt)
    {
        //Debug.LogError("AddPotAmount");
        double newAmt = totalPotAmount + amt;
        //Debug.LogError(amt + " newAmt=" + newAmt);
        UpdateText((int)newAmt);
        //Debug.LogError("newAmt="+ newAmt);
        totalPotAmount = newAmt;
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            PhotonRoomManager.instance.SetPotAmount(newAmt);
            //API transaction Amount Update by dealer
        }

    }
    public void DeductPotAmount(double amt)
    {
        //Debug.LogError("DeductPotAmount");
        double newAmt = totalPotAmount - amt;
        UpdateText((int)newAmt);
        totalPotAmount = newAmt;
        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            PhotonRoomManager.instance.SetPotAmount(newAmt);
            //API transaction Amount Update by dealer
        }
    }

    //ANIMATION********************************************************************************************
    public void PlayWinAmoutPotToBetPosition(Action _callback)
    {
        //Debug.LogError("PlayWinAmoutPotToBetPosition");
        betObj.gameObject.SetActive(true);
        betObj.GetComponentInChildren<Text>().text = PhotonPlayerManager.instance.GetCurrentBetAmount().ToString();
        betObj.DOMove(potPos.position, 0);
        betObj.DOMove(betPos.position, GameSetting.Instance.potCollectionSpeed/2).OnComplete(() => { _callback?.Invoke(); betObj.gameObject.SetActive(false); });

        DeductPotAmount(PhotonPlayerManager.instance.GetCurrentBetAmount());
    }

    //********************************************************************************************

    private void UpdateText(int newValue)
    {
        LeanTween.value((float)totalPotAmount, newValue, GameSetting.Instance.potCollectionSpeed).setOnUpdate((float val) =>
        {
            UpdateTextUI((int)val);
            //Debug.Log("Elapsed Time: " + Time.time);
        });

    }

   
    public void ResetPot()
    {
        totalPotAmount = 0;
        UpdateTextUI((int)totalPotAmount);
        betObj.gameObject.SetActive(false);
    }


    private void UpdateTextUI(int value)
    {
        Text.text = Utility.AbbreviateNumber(value);
    }

    //GETTER**************************************************************
    public double GetPotAmount()
    {
        return totalPotAmount;
    }
}
