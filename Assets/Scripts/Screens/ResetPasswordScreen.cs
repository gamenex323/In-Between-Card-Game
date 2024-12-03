using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetPasswordScreen : UIScreens
{

    [SerializeField] Button btn_back, btn_process;
    [SerializeField] InputField email_input;

    private void Start()
    {

        btn_process.onClick.AddListener(OnProceedClicked);

        btn_back.onClick.AddListener(() =>
        {


            GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

        });
    }

   

      

    private void OnEnable()
    {
        if (email_input != null)
        {
            email_input.text = "";
        }
    }
    void OnDestroy()
    {
        btn_process.onClick.RemoveAllListeners();
    }
    //**********************************************
    void OnProceedClicked()
    {
        if (email_input.text == "")
        {
            MessageBox.Instance.Show("Please enter email id.", MsgType.ALERT);
            return;
        }
        else if (!Utility.IsValidEmailAddress(email_input.text))
        {
            MessageBox.Instance.Show("Please enter valid email id", MsgType.ALERT);
            return;
        }

        JSONObject data = new JSONObject();
        data.AddField("email", email_input.text);

        Server.Instance.ApiRequest(typeof(ForgotPassword), OnForgotCompleted, data);
    }

    void OnForgotCompleted(JSONObject data)
    {
        ForgotPasswordDataRoot forgotPasswordData = JsonUtility.FromJson<ForgotPasswordDataRoot>(data.Print());
        //data from api

        if (forgotPasswordData.success)
        {
            DataManager.Instance.EmailId = email_input.text;
            GameEvents.EventScreenshow?.Invoke(ScreenType.OTP);

        }
        else
        {
            MessageBox.Instance.Show(forgotPasswordData.message);
        }
    }

}
