using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPasswordScreen : UIScreens
{

    [SerializeField] Button btn_back, btn_save;
    [SerializeField] InputField password_input ,confirm_input;

    void Start()
    {
        btn_save.onClick.AddListener(OnSaveBtnClicked);

        btn_back.onClick.AddListener(() =>
        {


            GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

        });
    }

    private void OnEnable()
    {
        if (password_input != null)
        {
            password_input.text = "";
            confirm_input.text = "";
        }
    }

    private void OnDestroy()
    {
        btn_save.onClick.RemoveAllListeners();
    }

    //***************************************************

    void OnSaveBtnClicked()
    {
        if (password_input.text == "")
        {
            MessageBox.Instance.Show("Please enter password.", MsgType.ALERT);
            return;
        }
        else if (confirm_input.text == "")
        {
            MessageBox.Instance.Show("Please enter confirm password", MsgType.ALERT);
            return;
        }
        else if (password_input.text != confirm_input.text)
        {
            MessageBox.Instance.Show("Password doesn't match", MsgType.ALERT);
            return;
        }
        GameEvents.EventScreenshow?.Invoke(ScreenType.Login);


        JSONObject data = new JSONObject();

        data.AddField("password", password_input.text);

        Server.Instance.ApiRequest(typeof(ResetPassword), OnResetPasswordCompleted, data);

    }

    void OnResetPasswordCompleted(JSONObject data)
    {

        ResetPasswordData resetPasswordData = JsonUtility.FromJson<ResetPasswordData>(data.Print());

        Debug.Log("OnLoginCompleted");
        //DataManager.Instance.LoginData = registerData;
        //data from api

        if (resetPasswordData.success)
        {
            MessageBox.Instance.Show(resetPasswordData.msg, MsgType.ALERT);
            MessageBox.Instance.setOkText("LOGIN");
            MessageBox.Instance.OnOkClicked += onOkClicked;

        }
        else
        {
            MessageBox.Instance.Show(resetPasswordData.msg, MsgType.ALERT);

        }

    }
    void onOkClicked()
    {
        MessageBox.Instance.OnOkClicked -= onOkClicked;
        GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

    }
}
