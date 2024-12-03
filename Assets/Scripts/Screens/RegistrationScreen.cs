using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RegistrationScreen : UIScreens
{
    [SerializeField] Button btn_register, btn_login;
    [SerializeField] InputField username_input, email_input, password_input , confirm_input;

    void Start()
    {
        btn_register.onClick.AddListener(OnRegisterBtnClicked);
        btn_login.onClick.AddListener(OnLoginClicked);
        MessageBox.Instance.Show("Please enter a valid email address and ensure the password is at least 8 characters long.", MsgType.OKONLY);

    }

    private void OnEnable()
    {
        if (username_input != null)
        {
            username_input.text = "";
            email_input.text = "";
            password_input.text = "";
            confirm_input.text = "";

        }
    }

    private void OnDestroy()
    {
        btn_login.onClick.RemoveAllListeners();
        btn_register.onClick.RemoveAllListeners();

    }

    //***********************************************************************************
    //void OnNormalClick(bool on)
    //{
    //    if (on)
    //    {
    //        role = "user";
    //    }
    //}

    //void OnBusinessClick(bool on)
    //{
    //    if (on)
    //    {
    //        role = "company";
    //    }
    //}

    void OnRegisterBtnClicked()
    {
        OnLoginButtonClick();
        if (username_input.text == "")
        {
            MessageBox.Instance.Show("Please enter user name.", MsgType.ALERT);
            return;
        }
        else if (email_input.text == "")
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

        else if (confirm_input.text == "")
        {
            MessageBox.Instance.Show("Please enter confirm password", MsgType.ALERT);
            return;
        }
        else if (confirm_input.text != password_input.text)
        {
            MessageBox.Instance.Show("Password not matched", MsgType.ALERT);
            return;
        }
        //GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

        JSONObject data = new JSONObject();
        data.AddField("name", username_input.text);
        data.AddField("email", email_input.text);
        data.AddField("password", password_input.text);


        Server.Instance.ApiRequest(typeof(Register), OnRegisterCompleted, data);
    }

    public void OnLoginButtonClick()
    {
        string username = username_input.text;
        string password = password_input.text;

        // Check if the email is in English and contains the "@" symbol
        if (IsEmailValid(email_input.text) )
        {
            // Check if the input contains non-English characters
            if (IsPasswordValid(password))
            {
                MessageBox.Instance.Show("Please enter password more than 8 characters.", MsgType.ALERT);
                return;
            }


           
            if (ContainsNonEnglishCharacters(username)) {
                MessageBox.Instance.Show("Password in English only.", MsgType.ALERT);
                return;
            }
                
                
            if(ContainsNonEnglishCharacters(password))
            {
                // Display a message to the user
                MessageBox.Instance.Show("Please enter usrname in English only.", MsgType.ALERT);
                return;

            }
            if (!IsPasswordValid(password))
            {
                MessageBox.Instance.Show("Please enter password more than 8 Char.", MsgType.ALERT);
                return;
            }
        }
        else
        {
            // Display a message to the user about the email and password requirements
            MessageBox.Instance.Show("Please enter a valid email address", MsgType.ALERT);
            return;
            //feedbackText.text = "Please enter a valid email address and ensure the password is at least 8 characters long.";
        }
    }

    // Check if a given string contains non-English characters
    private bool ContainsNonEnglishCharacters(string input)
    {
        foreach (char c in input)
        {
            if (c > 127) // Assuming ASCII characters for English
            {
                return true;
            }
        }
        return false;
    }

    // Check if the password is greater than 8 characters
    private bool IsPasswordValid(string password)
    {
        print("Password length is: " + password.Length);
        return password.Length > 8;
    }

    // Check if the email is in English and contains the "@" symbol
    private bool IsEmailValid(string email)
    {
        return !ContainsNonEnglishCharacters(email) && email.Contains("@");
    }

    void OnRegisterCompleted(JSONObject data)
    {

        //loginData = JsonUtility.FromJson<Api_Login>(status.Print());
        //data from api

        RegisterData registerData = JsonUtility.FromJson<RegisterData>(data.Print());

        Debug.Log("OnLoginCompleted");
        //DataManager.Instance.LoginData = registerData;
        //data from api

        if (registerData.success)
        {
            Debug.Log("success");

            MessageBox.Instance.Show("Registered sucessfully", MsgType.OKONLY);
            MessageBox.Instance.setOkText("Sign in");
            MessageBox.Instance.OnOkClicked += onOkClicked;
            //AutoLoginManager.Instance.UpdateLoginStatus(1, username, password);
            //CallGetUserDetail();
        }
        else
        {
            MessageBox.Instance.Show(registerData.message, MsgType.ALERT);

        }

    }

    void onOkClicked()
    {
        MessageBox.Instance.OnOkClicked -= onOkClicked;
        GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

    }
    void OnLoginClicked()
    {

        GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

    }
}
