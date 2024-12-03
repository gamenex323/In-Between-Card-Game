using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonSenderReciver : MonoBehaviourPun
{
    public static Action<string> OnRecivedNewCardDeck;
    public static Action<int> OnRecivedTurnId;
    public static Action<double> OnRecivedRequestPotCollection;
    public static Action<ButtonActionType, double> OnRecivedButtonAction;
    public static Action<bool> OnRecivedCurrentPlayerWinLose;
    public static PhotonSenderReciver instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance);
        }

    }



    //****************************************************************************************************************
    //SEND EVENTS*****************************************************************************************************
    //****************************************************************************************************************
    /*public void InitDeck()
    {
        if(PhotonNetwork.IsMasterClient)
        photonView.RPC(nameof(RpcInitDeck), RpcTarget.All, null);
    }*/
    public void CreateNewCardDeck(string generateNewCardDeckIds)
    {
        photonView.RPC(nameof(RpcCreateNewCardDeck), RpcTarget.All, generateNewCardDeckIds);
    }
    public void CollectPotAmountRequest(double amt)
    {
        photonView.RPC(nameof(RpcCollectPotAmountRequest), RpcTarget.All, amt);
    }
    public void SetTurn(int itd)
    {
       photonView.RPC(nameof(RpcSetTurnId), RpcTarget.All, itd);
    }
    
    public void ButtonSelected(int v, double amt)
    {
        photonView.RPC(nameof(RpcButtonSelected), RpcTarget.All, v, amt);
    }

    public void SendCurrentPlayerWinLose(bool v)
    {
        photonView.RPC(nameof(RpcCurrentPlayerWinLose), RpcTarget.All, v);
    }
    /*public void MoveTopThreeCardAndFlip()
    {
        photonView.RPC(nameof(RpcMoveTopThreeCardAndFlip), RpcTarget.All);
    }*/

    /*public void Send_PlayerDetail(PlayerData myPlayerData, PlayerData oppPlayerData)
    {

        string my = JsonUtility.ToJson(myPlayerData);
        string opp = JsonUtility.ToJson(oppPlayerData);

        object[] pData = new object[] { my, opp };

        photonView.RPC(nameof(RpcPlayerDetail), RpcTarget.Others, pData);
    }

    public void Send_Card_Ids(int[] myCardIds, int[] oppoCardIds, int game, int gameType)
    {

        object[] cardIds = new object[] { myCardIds, oppoCardIds, game, gameType };
        photonView.RPC(nameof(RpcSendRecivedCardIds), RpcTarget.Others, cardIds);
    }

    public void Send_Game_Ready()
    {

        photonView.RPC(nameof(RpcGameReady), RpcTarget.Others, null);
    }*/




    //******************************************************************************************************************
    //RECIVED EVENTS****************************************************************************************************
    //******************************************************************************************************************
    [PunRPC]
    void RpcCreateNewCardDeck(string newDeckStr)
    {
        OnRecivedNewCardDeck?.Invoke(newDeckStr);
    }

    [PunRPC]
    void RpcCollectPotAmountRequest(double amt)
    {
        Debug.Log("RpcCollectPotAmountRequest");
        OnRecivedRequestPotCollection?.Invoke(amt);
    }


    [PunRPC]
    void RpcSetTurnId(int tId)
    {
        Debug.Log("RpcSetTurnId");
       OnRecivedTurnId?.Invoke(tId);
    }

    


    [PunRPC]
    void RpcButtonSelected(int btnIndex, double amt)
    {
        ButtonActionType act = (ButtonActionType)btnIndex;
        OnRecivedButtonAction?.Invoke(act, amt);
    }

    [PunRPC]
    void RpcCurrentPlayerWinLose(bool isWinner)
    {

        OnRecivedCurrentPlayerWinLose?.Invoke(isWinner);
    }

    



    [PunRPC]
    void RpcPlayerDetail(object[] data)
    {

        // PlayerData myData = JsonUtility.FromJson<PlayerData>(data[0].ToString());
        //PlayerData oppoData = JsonUtility.FromJson<PlayerData>(data[1].ToString());


    }

    [PunRPC]
    void RpcSendRecivedCardIds(object[] data)
    {

    }

    
}  

