using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransactionHistory : UIScreens
{
    [SerializeField]
    ScrollRect scrollView;
    
    
    
    [SerializeField]
    Button back;

    [SerializeField]
    Transform container;

    [SerializeField]
    GameObject transactionItem;

    [SerializeField]
    int maxItemAtTime = 10;

    int currentLot = 0;

    bool isLoadAllow;

    TranscationHistoryApiRoot getTransactionData;
    //*******************************************************************************
    private void Start()
    {
        isLoadAllow = false;
        scrollView.onValueChanged.AddListener(OnDragScroll);

        back.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Wallet);
        });

    }
    private void OnEnable()
    {
        LoadTransactionList();
    }

    private void OnDestroy()
    {
        scrollView.onValueChanged.RemoveAllListeners();
    }
    //*******************************************************************************
    void LoadTransactionList()
    {
        clearList();

        currentLot = 0;

        JSONObject data = new JSONObject();
        data.AddField("skip", (maxItemAtTime * currentLot).ToString());
        data.AddField("take", maxItemAtTime.ToString());
        Server.Instance.ApiRequest(typeof(TranscationHistoryApi), OnGetTransactionData, data);
    }

    void LoadMore()
    {

        JSONObject data = new JSONObject();
        data.AddField("skip", (maxItemAtTime * currentLot).ToString());
        data.AddField("take", maxItemAtTime.ToString());
        Server.Instance.ApiRequest(typeof(TranscationHistoryApi), OnGetTransactionData, data);
    }

    void OnGetTransactionData(JSONObject data)
    {
        isLoadAllow = true;
       getTransactionData = JsonUtility.FromJson<TranscationHistoryApiRoot>(data.Print());
        if (getTransactionData.success)
        {

            AddScrollItem();
            currentLot++;

        }
        else
        {
            isLoadAllow = false;
            //MessageBox.Instance.Show(getTransactionData.msg, MessageType.OKONLY);
        }
    }

    void AddScrollItem()
    {
        //clearList();
        for (int i = 0; i < getTransactionData.userData.Count; i++)
        {
            GameObject item = Instantiate(transactionItem, container);
            TransactionItem tItem = item.GetComponent<TransactionItem>();
            tItem.SetData(getTransactionData.userData[i]);
            //UserTranscationData tData = getTransactionData.userData[i];
            //tItem.GetComponent<Button>().onClick.AddListener(() => OnItemClicked(tData));
        }
    }


    /*void OnItemClicked(UserTranscationData tData)
    {
        string str = "Transaction id:" + tData.id + "\n\n";
        //str += "Transaction reason:" + tData.trans_reason + "\n\n";
        //str += "Status:" + tData.added_to_balance + "\n\n";
        str += "Transaction reason:" + tData.trans_reason + "\n\n";
        str += "Current Wallet Balance:" + tData.amount + "\n\n";
        //str += "Current Winning Balance:" + tData.current_winning_bal + "\n\n";

        MessageBox.Instance.Show(str, MsgType.OKONLY);
    }*/

    void OnDragScroll(Vector2 v)
    {
        //Debug.Log(scrollView.verticalNormalizedPosition);
        if (!isLoadAllow) return;
        if (scrollView.verticalNormalizedPosition >= 0.95f)
        {
            Debug.Log("scrolled near start");
        }
        else if (scrollView.verticalNormalizedPosition <= -0.0025f)
        {
            Debug.Log("scrolled near end");

            isLoadAllow = false;
            LoadMore();
        }
    }


    void clearList()
    {
        if (container.childCount == 0) return;
        foreach (Transform child in container)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
