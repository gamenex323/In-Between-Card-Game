using System;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : UIScreens
{
    public static Action<GameMode> SetGameMode;
    [SerializeField] private Button btn_playWithFriends, btn_playOnline, btn_howToPlay, btn_settings, btn_addMoney, btn_wallet, btn_profile, btnAddChip, btn_addFriend;
    public Image imgProfile;
    [SerializeField] private Image mask;
    public Text profile_name;

    [SerializeField] private Text betAmtOnline, betAmtFrnd;

    private void OnEnable()
    {
        if (DataManager.Instance.GetProfileImage() != null)
            ShowLogo(DataManager.Instance.GetProfileImage());
        else
        {
            mask.gameObject.SetActive(false);
            mask.transform.GetChild(0).GetComponent<Image>().sprite = null;
        }

        profile_name.text = DataManager.Instance.UserDetailsData.userdata.name;
        betAmtOnline.text = betAmtFrnd.text = GameSetting.Instance.MinimumBetAmount.ToString() + "/" + (GameSetting.Instance.TempWalletAmount).ToString();
    }

    private void Start()
    {
        Listners();
    }

    private void Listners()
    {
        btn_profile.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Profile);
        });
        btn_addMoney.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.AddMoney);
        });
        btnAddChip.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.AddMoney);
        });

        btn_wallet.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Wallet);
        });

        btn_playOnline.onClick.AddListener(() =>
        {
            // GameEvents.EventScreenshow?.Invoke(ScreenType.Game);
            SetGameMode?.Invoke(GameMode.PLAY_ONLINE);
            PhotonRoomManager.instance.JoinRandomRoom();
        });

        btn_playWithFriends.onClick.AddListener(() =>
        {
            SetGameMode?.Invoke(GameMode.PLAY_WITH_FRIEND);
            // GameEvents.EventScreenshow?.Invoke(ScreenType.Game);
            PopupsManager.Instance.Show("CreateOrJoinPopup");
        });

        btn_howToPlay.onClick.AddListener(() =>
        {
            PopupsManager.Instance.Show("HowToPlay");
        });

        btn_settings.onClick.AddListener(() =>
        {
            PopupsManager.Instance.Show("Settings");
            // GameEvents.EventScreenshow?.Invoke(ScreenType.ChangePassword);
        });

        btn_addFriend.onClick.AddListener(() =>
        {
            PopupsManager.Instance.Show("Friends");
            // GameEvents.EventScreenshow?.Invoke(ScreenType.ChangePassword);
        });
    }

    private void ShowLogo(Sprite sprite)
    {
        mask.gameObject.SetActive(true);
        mask.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

        // placeHolder.SetActive(false);
        // container.enabled = true;
        // imgProfile.sprite = sprite;
        //imgProfile.SetNativeSize();
    }
}