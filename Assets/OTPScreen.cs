using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OTPScreen : UIScreens
{
    [SerializeField]
    InputField codeInput;

    [SerializeField]
    Button confirmBtn, btn_back;

   

    void Start()
    {
        confirmBtn.onClick.AddListener(OnConfirmBtnClicked);

        btn_back.onClick.AddListener(() =>
        {


            GameEvents.EventScreenshow?.Invoke(ScreenType.Login);

        });
    }

    private void OnEnable()
    {
        if (codeInput != null)
        {
            codeInput.text = "";
        }
    }

    private void OnDestroy()
    {
        confirmBtn.onClick.RemoveAllListeners();
    }

    //***************************************************

    void OnConfirmBtnClicked()
    {
        if (codeInput.text == "")
        {
            MessageBox.Instance.Show("Please enter code number which is sent to your email id.", MsgType.OKONLY);
            return;
        }

        WWWForm data = new WWWForm();

        data.AddField("email", DataManager.Instance.EmailId);
        data.AddField("code", codeInput.text);

        Server.Instance.ApiRequest(typeof(MatchCode), OnMatchCodeCompleted, data);

    }

    void OnMatchCodeCompleted(JSONObject data)
    {
        MatchCodeData matchCodeData = JsonUtility.FromJson<MatchCodeData>(data.Print());
        //data from api

        if (matchCodeData.success)
        {
            DataManager.Instance.AuthKey = matchCodeData.otpdata.auth_key;
            Debug.Log(" matchCodeData.auth_key" + matchCodeData.otpdata.auth_key);
            GameEvents.EventScreenshow?.Invoke(ScreenType.ChangePassword);

        }
        else
        {
            MessageBox.Instance.Show(matchCodeData.message);
        }
    }
}
