using UnityEngine;
using UnityEngine.UI;

public class PlayerWaitingForFriends : MonoBehaviour
{
    [SerializeField] private Button shareBtn, inviteBtn, playBtn;
    [SerializeField] private Text codeTxt;
    private void Start()
    {
        shareBtn.onClick.AddListener(OnShareBtnClicked);
        inviteBtn.onClick.AddListener(OnInviteBtnClicked);
        playBtn.onClick.AddListener(OnPlayBtnClicked);

        PhotonPlayerManager.OnNewPlayerJoined += OnNewPlayerJoined;
    }

    private void OnEnable()
    {
        playBtn.interactable = false;
        codeTxt.text = PhotonRoomManager.instance.GetRoomName();

        if (PhotonLobbyManager.instance.IsIAmDealer())
        {
            playBtn.gameObject.SetActive(true);
        }
        else
        {
            playBtn.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        shareBtn.onClick.RemoveAllListeners();
        inviteBtn.onClick.RemoveAllListeners();
        playBtn.onClick.RemoveAllListeners();

        PhotonPlayerManager.OnNewPlayerJoined -= OnNewPlayerJoined;
    }

    //*******************************************************************************

    private void OnShareBtnClicked()
    {
    }

    private void OnInviteBtnClicked()
    {
    }

    private void OnPlayBtnClicked()
    {
        GameManager.Instance.StartGame();
    }

    private void OnNewPlayerJoined()
    {
        if (PhotonPlayerManager.instance.GetPlayers().Length > 1)
        {
            playBtn.interactable = true;
        }
        else
        {
            playBtn.interactable = false;
        }
    }
}