using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
    public static Action<ButtonActionType> OnClicked;
    public static Action<double> OnRaiseAmountChanged;

    [SerializeField] Button foldBtn, drawBtn, raiseBtn, maxBetBtn;
    [SerializeField] GameObject raisePanel;
    [SerializeField] Button minusBtn, plusBtn;
    [SerializeField] Text amtTxt;

    bool isOpen;

    double raiseValue;
    void Start()
    {
        isOpen = false;
        raisePanel.SetActive(false);
        foldBtn.onClick.AddListener(() => { OnClicked(ButtonActionType.FOLD); DisableBtns(); });
        drawBtn.onClick.AddListener(() => { OnClicked(ButtonActionType.DRAW); DisableBtns();});
        raiseBtn.onClick.AddListener(() => { isOpen = !isOpen; OnRaisePanelOpenClose(isOpen); });
        maxBetBtn.onClick.AddListener(() => { OnClicked(ButtonActionType.MAXBET); DisableBtns(); });

        minusBtn.onClick.AddListener(OnMinusClicked);
        plusBtn.onClick.AddListener(OnPlusClicked);
        DisableBtns();

       
    }

    private void OnDestroy()
    {
        foldBtn.onClick.RemoveAllListeners();
        drawBtn.onClick.RemoveAllListeners();
        raiseBtn.onClick.RemoveAllListeners();
        maxBetBtn.onClick.RemoveAllListeners();

        minusBtn.onClick.RemoveAllListeners();
        plusBtn.onClick.RemoveAllListeners();
    }

    public void EnableBtns()
    {
        foldBtn.interactable = true;
        drawBtn.interactable = true;
        //oncondition
        raiseBtn.interactable = true;

        maxBetBtn.interactable = true;//(WalletManager.instance.GetTempWalletAmount() >= PotManager.Instance.totalPotAmount) ? true : false;
       
    }

    public void DisableBtns()
    {
        foldBtn.interactable = false;
        drawBtn.interactable = false;        
        raiseBtn.interactable = false;
        maxBetBtn.interactable = false;
        isOpen = false;
        OnRaisePanelOpenClose(isOpen);
    }

   //******************************************************************************************************************
   void OnRaisePanelOpenClose(bool isOpen)
    {
        raisePanel.SetActive(isOpen);
        if (isOpen)
        {
            raiseValue = PhotonRoomManager.instance.GetMinimumBetAmount();

        }
        else
        {

        }

        UpdateTextUI();
    }
    void OnPlusClicked()
    {
        raiseValue+= PhotonRoomManager.instance.GetMinimumBetAmount();
        
        if (WalletManager.instance.GetTempWalletAmount() > PotManager.Instance.totalPotAmount)
        {
            
            if (raiseValue > PotManager.Instance.totalPotAmount)
            {
                raiseValue = PotManager.Instance.totalPotAmount;
               
            }
        }
        else
        {
           
            if (raiseValue > WalletManager.instance.GetTempWalletAmount())
            {
                
                raiseValue -= PhotonRoomManager.instance.GetMinimumBetAmount();
            }
        }
        OnRaiseAmountChanged?.Invoke(raiseValue);
        UpdateTextUI();

    }
    void OnMinusClicked()
    {
        raiseValue -= PhotonRoomManager.instance.GetMinimumBetAmount();
        if (raiseValue < PhotonRoomManager.instance.GetMinimumBetAmount())
        {
            raiseValue = PhotonRoomManager.instance.GetMinimumBetAmount();
        }
        OnRaiseAmountChanged?.Invoke(raiseValue);
        UpdateTextUI();
    }

    void UpdateTextUI()
    {
        amtTxt.text = Utility.AbbreviateNumber((float)raiseValue);
        
    }
}
public enum ButtonActionType{
    FOLD=0,
    DRAW=1,
    RAISE=2,
    MAXBET=3
}