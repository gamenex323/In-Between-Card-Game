using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WalletManager : ExtendedBehavior
{
    public static Action OnAmoutDeducted;
    public static Action OnWalletUpated;
    
    public bool backButtonPressed = false;
    [SerializeField] private double balance = 100;
    [SerializeField] private double tempWallet = 0;
    [SerializeField] private Text amountTxt;

    [SerializeField]
    private UniWebView webView;

    public static WalletManager instance;

    private void Awake()
    {

        print("Balance is: " + balance);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
        Invoke(nameof(UpdateUI), 0.1f);
    }

    private void Start()
    {
        PhotonSenderReciver.OnRecivedRequestPotCollection += OnRecivedRequestPotCollection;
    }

    private void OnDestroy()
    {
        PhotonSenderReciver.OnRecivedRequestPotCollection -= OnRecivedRequestPotCollection;
    }

    //Recived from Photon
    private void OnRecivedRequestPotCollection(double amt)
    {
        Debug.Log("OnRecivedRequestPotCollection");
        DeductAmount(amt);
    }

    //METHOD*********************************************************************
    int convertDoubletoInt(double val)
    {
        int intVal = Mathf.FloorToInt((float)val); // Convert and round down to int
        return intVal;
    }
    public void AddWinngAmount(double amt)
    {
        //API Wallet deduct transation
        balance += amt;
        tempWallet += amt;

        WalletBalanceManager.walletBalance = convertDoubletoInt(balance);

        WalletBalanceManager.instance.SaveWalletBalance(convertDoubletoInt(balance));
        OnAddDeductCompleted();
    }

    public void DeductAmount(double amt)
    {
        //temp
        balance -= amt;
        tempWallet -= amt;

        WalletBalanceManager.walletBalance = convertDoubletoInt(balance);
        //API Wallet deduct transation
        OnAddDeductCompleted();
    }

    public void OnAddDeductCompleted()
    {
        Debug.Log("OnDeductCompleted");
        OnAmoutDeducted?.Invoke();
        OnWalletUpated?.Invoke();

        UpdateUI();
    }

    private void UpdateUI()
    {
        print("Update UI balance: " + balance);
        amountTxt.text = Utility.AbbreviateNumber((float)balance);
        
    }

    //SETTER**********************************************************************
    public void SetWalletBalance(int amt)
    {
        balance = amt;
        UpdateUI();
    }
    public void SetTempWalletBalance(double amt)
    {
        tempWallet = amt;
        
    }
    //GETTER*******************************************************************
    public double GetTempWalletAmount()
    {
        //return balance;
        return tempWallet;
    }
    public double GetWalletAmount()
    {
        //return balance;
        return balance;
    }
    //wallet Update API *************************************************************
    public void UpdateWallet(string amount)
    {
        backButtonPressed = false;
        webView.SetBackButtonEnabled(false);
        WWWForm formData = new WWWForm();
        formData.AddField("amount", amount);
        Debug.Log("add money wallet api" + formData.data);


        Server.Instance.ApiRequest(typeof(Wallet), OnWalletUpdated, formData, false);
        Server._LoadingStart();
    }

    private void OnWalletUpdated(JSONObject data)
    {
        Debug.Log("Onwalletupdated");
        WalletRoot walletRoot = JsonUtility.FromJson<WalletRoot>(data.Print());
        DataManager.Instance.WalletData = walletRoot;

        walletRoot.success = true;
        if (walletRoot.success)
        {
            LoadWebView(walletRoot.paymentdata.payment_url);
            //string str = "window.open(\"" + walletRoot.paymentdata.payment_url + "\",\"_blank\")";
            //Application.ExternalCall(str);
            Wait(3f, () =>
            {
                print("Encrypted Sting: " + walletRoot.paymentdata.encrypt_string);
                UpdatePayment(walletRoot.paymentdata.encrypt_string);
            });

            // ShowLogo(sprite);

            Debug.Log(walletRoot.paymentdata.payment_url);
            //MessageBox.Instance.Show(walletRoot.message);
        }
        else
        {
            Server._LoadingEnd();
            MessageBox.Instance.Show(walletRoot.message);
        }
    }

    //wallet Update API *************************************************************

    public void UpdatePayment(string encrypted_string)
    {
        Debug.Log("encrypted_string==" + encrypted_string);
        WWWForm formData = new WWWForm();
        formData.AddField("encrypted_string", encrypted_string);
        //Debug.Log("add money wallet api" + formData.data);
        //Server.Instance.ApiRequest(typeof(PaymentCheck), OnUpdatePaymentStatusUpdated, formData, false);
        StartCoroutine(Call_API_Check_Pament_Status(formData, OnUpdatePaymentStatusUpdated));
    }

    public IEnumerator Call_API_Check_Pament_Status(WWWForm data, System.Action<JSONObject> callback)
    {
        Hashtable headers = new Hashtable();
        headers.Add("Authorization", "Bearer " + DataManager.Instance.GetAuthKey());
        string url = Server_Constants.serverApi_BaseUrl + "/api/check_payment_status";

        WWW www = new WWW(url, data.data, headers);
        yield return www;
        Debug.Log("What is this: " + www.text);
        JSONObject status_text = new JSONObject(www.text);
        callback(status_text);
    }

    private void OnUpdatePaymentStatusUpdated(JSONObject data)
    {
        PaymentCheckRoot paymentCheckRoot = JsonUtility.FromJson<PaymentCheckRoot>(data.Print());
        DataManager.Instance.paymentCheck = paymentCheckRoot;
        paymentCheckRoot.paymentData.payment_status = 1;
        Debug.Log("OnUpdatePaymentStatusUpdated= " + paymentCheckRoot.paymentData.payment_status);
        paymentCheckRoot.success = true;
        if (paymentCheckRoot.success)
        {
            print("Check success condition");
            //Debug.Log(paymentCheckRoot.paymentData.payment_status+ "---------------------");

            if (paymentCheckRoot.paymentData.payment_status == 2)
            {
                Debug.Log("status 2");

                MessageBox.Instance.Show(paymentCheckRoot.message, MsgType.OKONLY);
                webView.Hide();
                Server._LoadingEnd();
                return;
            }
            else if (paymentCheckRoot.paymentData.payment_status == 1)
            {
                Debug.Log("status 1");

                MessageBox.Instance.Show(paymentCheckRoot.message, MsgType.OKONLY);
                Server._LoadingEnd();
                webView.Hide();
                Server.Instance.OnUserDetailUpdateCall();

                return;
            }
            else if (paymentCheckRoot.paymentData.payment_status == 0)
            {
                Debug.Log("Status 0");
                    Wait(3f, () =>
                    {
                        //Debug.Log("status 0");
                        //if (paymentCheckRoot.paymentData.payment_status != 1 && paymentCheckRoot.paymentData.payment_status != 2)
                        
                            UpdatePayment(DataManager.Instance.WalletData.paymentdata.encrypt_string);
                        
                    });
               
            }
        }
        else
        {
            MessageBox.Instance.Show(paymentCheckRoot.message);
        }
    }
   

    private void Update()
    {
        if (backButtonPressed) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backButtonPressed = true;
        }
    }
    //withdraw api
    public void WithdrawRequest(string amount, string email)
    {
        Debug.Log("CallWithdrawreq");
        WWWForm formData = new WWWForm();
        formData.AddField("amount", amount);
        formData.AddField("receiver_email", email);
        Server.Instance.ApiRequest(typeof(Withdraw), OnWithdrawRequestMade, formData, false);
    }

    private void OnWithdrawRequestMade(JSONObject data)
    {
        WithdrawRoot withdrawData = JsonUtility.FromJson<WithdrawRoot>(data.Print());
        if (withdrawData.success)
        {
            MessageBox.Instance.Show(withdrawData.message);
        }
        else
        {
            MessageBox.Instance.Show(withdrawData.message);
        }
    }

    private void LoadWebView(string link)
    {
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height);
        webView.Load(link);
        webView.Show();
    }
}