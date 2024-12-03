using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransactionItem : MonoBehaviour
{
    
    [SerializeField]
    Text dateTxt;

    [SerializeField]
    Text descTxt;

    [SerializeField]
    Text amountTxt;
    
    [SerializeField]
    Text transIdTxt;
    //[SerializeField]
    //Text statusTxt;


    public void SetData(UserTranscationData data)
    {

        DateTime oDate = DateTime.ParseExact(data.created_at, "yyyy-MM-dd HH:mm:ss", null);
        dateTxt.text = oDate.ToString("dddd, dd MMMM yyyy");

        //descTxt.text = data.trans_reason;
        //statusTxt.text = data.added_to_balance;

        if (data.txn_status == "debit")
        {
            amountTxt.text = "-$" +data.amount.ToString();          

        }
        else
        {
            amountTxt.text = "+$" + data.amount.ToString();            
        }

        if(data.type == "play")
        {
           if(data.txn_status == "debit")
            {
                descTxt.text = "Loss A Match";
            }
            else
            {
                descTxt.text = "Won A Match";
            }

        }else if (data.type == "add")
        {
            descTxt.text = "Amount Add to wallet";
        }else if (data.type == "redeem")
        {
            descTxt.text = "Amount Withdraw from wallet";
        }
        transIdTxt.text = "Transaction Id: " + data.id;

    }
}
