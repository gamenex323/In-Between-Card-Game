using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoginScreen : UIScreens
{

    [SerializeField] Button btn_register, btn_login, btn_forgot;
    [SerializeField] InputField email_input, password_input;
    [SerializeField] Toggle toggle_remember;


    void Start()
    {



        btn_register.onClick.AddListener(() =>
        {
            OnRegisterBtnClicked();


        });

        btn_forgot.onClick.AddListener(() =>
        {
            OnForgotPasswordClicked();


        });
        
        btn_login.onClick.AddListener(() =>
        {
            OnLoginClicked();
        });
    }


    //////void FirstCome()
    //////{
    //////    if (PlayerPrefs.GetInt("firstCome") == 0)
    //////    {
    //////        print("first come");
    //////        Server.Instance.UpdateBalancePurchase(0);
    //////        PlayerPrefs.SetInt("" + email_input.text, 10);
    //////        PlayerPrefs.SetInt("firstCome", 1);
    //////    }
    //////    //Server.Instance.UpdateBalanceLocally();

    //////}

    private void OnEnable()
    {
        if (email_input != null)
        {
            email_input.text = "";
            password_input.text = "";
        }

    }

    private void OnDestroy()
    {
        btn_login.onClick.RemoveAllListeners();
        btn_register.onClick.RemoveAllListeners();
        btn_forgot.onClick.RemoveAllListeners();
        toggle_remember.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle_remember);
        });
    }

    //***********************************************************************************
    void OnLoginClicked()
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
        else if (password_input.text == "")
        {
            MessageBox.Instance.Show("Please enter password", MsgType.ALERT);
            return;
        }
        ///GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
        ///
        //Invoke(nameof(FirstCome),2f);
        //Server.Instance.UpdateBalanceApp();

         Server.Instance.CallLogIn(email_input.text, password_input.text);
    }

    void OnRegisterBtnClicked()
    {

        email_input.text = "";
        password_input.text = "";
        GameEvents.EventScreenshow?.Invoke(ScreenType.Registration);
       
    }

    void OnForgotPasswordClicked()
    {
        GameEvents.EventScreenshow?.Invoke(ScreenType.ResetPassword);


    }
    void ToggleValueChanged(bool value)
    {
        AutoLoginManager.Instance.AllowAutoLogin = value;
    }




}
