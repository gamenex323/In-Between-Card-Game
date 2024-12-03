using UnityEngine;
using UnityEngine.UI;

public class InviteFrinds : MonoBehaviour
{
    public Text indexing, playerNameText;
    public Button btn_invite;
    public int id;

    private void Start()
    {
        btn_invite.onClick.AddListener(() =>
        {
            SendCode();
        });
    }

    private void SendCode()
    {
        JSONObject data = new JSONObject();
        data.AddField("join_users", id.ToString());
        data.AddField("code", PhotonRoomManager.instance.GetRoomName());
        Server.Instance.ApiRequest(typeof(SendCode), OnSendCodeDone, data);
    }

    private void OnSendCodeDone(JSONObject data)
    {
        SendCodeRoot sendCode = JsonUtility.FromJson<SendCodeRoot>(data.Print());

        if (sendCode.success)
        {
            Debug.Log(PhotonRoomManager.instance.GetRoomName());
            PopupsManager2.Instance.Hide();
            MessageBox.Instance.Show(sendCode.message, MsgType.OKONLY);

        }
        else
        {
            MessageBox.Instance.Show(sendCode.message, MsgType.OKONLY);
        }
    }
}