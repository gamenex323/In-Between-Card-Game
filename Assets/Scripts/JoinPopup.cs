using UnityEngine;
using UnityEngine.UI;

public class JoinPopup : MonoBehaviour
{
    [SerializeField]
    private Button closeBtn;

    [SerializeField]
    private InputField codeTxt;

    [SerializeField]
    private Button joinBtn;

    private void Start()
    {
        joinBtn.onClick.AddListener(OnJoinClicked);
        closeBtn.onClick.AddListener(OnCloseClicked);
    }

    private void OnEnable()
    {
        codeTxt.text = "";
    }

    private void OnDestroy()
    {
        joinBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }

    //*************************************************************
    private void OnJoinClicked()
    {
        if (codeTxt.text == "")
        {
            MessageBox.Instance.Show("Please enter private code ", MsgType.OKONLY);
            return;
        }

        PhotonRoomManager.instance.JoinPrivateRoom(codeTxt.text);
    }

    private void OnCloseClicked()
    {
        PhotonRoomManager.instance.Call_LeaveRoom();
    }
}