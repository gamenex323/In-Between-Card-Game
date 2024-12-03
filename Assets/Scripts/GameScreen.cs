using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreen : UIScreens
{
    public static Action OnStartWaitingTimer;
    [SerializeField] Button backBtn;
    private void Start()
    {
        backBtn.onClick.AddListener(OnBackButtonClicked);
        PhotonWaitingTimerManager.OnWaitingTimeFinished += ShowBackBtn;
    }
    
    private void OnEnable()
    {

        OnStartWaitingTimer?.Invoke();
        backBtn.gameObject.SetActive(false);
    }
    public void ShowBackBtn() {
        backBtn.gameObject.SetActive(true);
    }

    public void OnBackButtonClicked()
    {
        MessageBox.Instance.Show("Are you sure you want to quit?", MsgType.YESNOBOTH);
        MessageBox.Instance.OnOkClicked += OnYesClicked;
    }
    void OnYesClicked()
    {
        MessageBox.Instance.OnOkClicked -= OnYesClicked;
        PhotonRoomManager.instance.Call_LeaveRoom();
      //  GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
    }

    private void OnDestroy()
    {
        backBtn.onClick.RemoveAllListeners();
        PhotonWaitingTimerManager.OnWaitingTimeFinished -= ShowBackBtn;
    }
}
