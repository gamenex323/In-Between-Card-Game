using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateOrJoinPopup : MonoBehaviour
{
    [SerializeField]
    Button closeBtn;

    [SerializeField]
    Button createRoomBtn;

    [SerializeField]
    Button joinRoomBtn;

    private void Start()
    {
        createRoomBtn.onClick.AddListener(OnCreateRoomClicked);
        joinRoomBtn.onClick.AddListener(OnJoinRoomClicked);
        closeBtn.onClick.AddListener(OnBackClicked);
    }

    private void OnEnable()
    {
        /*if (GameSettings.instance.IsOnlinePlayer())
        {
            joinRoomBtn.gameObject.SetActive(false);
        }
        else
        {
            joinRoomBtn.gameObject.SetActive(true);
        }*/

        
    }

    void OnDestroy()
    {
        createRoomBtn.onClick.RemoveAllListeners();
        joinRoomBtn.onClick.RemoveAllListeners();
        closeBtn.onClick.RemoveAllListeners();
    }
    //***************************************************************************

    void OnCreateRoomClicked()
    {
        //PhotonRoomManager.instance.Call_CreateRoom();
        PhotonRoomManager.instance.CreatePrivateRoom();
    }



    void OnJoinRoomClicked()
    {
        //PopupsManager.Instance.Hide();
        PopupsManager.Instance.Show("JoinPopup");
    }

    void OnBackClicked()
    {
        PopupsManager.Instance.Hide();
        //PopupsManager.Instance.ShowHome();

    }
}
