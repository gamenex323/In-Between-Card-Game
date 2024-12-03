using UnityEngine;
using UnityEngine.UI;

public class WalletScreen : UIScreens
{
    public Button btn_AddMoney, btn_Widraw, btnBack, transBtn, helpBtn;
    public Text txt_Amount;

    private void Start()
    {
        Listeners();
    }

    private void OnEnable()
    {
        txt_Amount.text = Utility.AbbreviateNumber((float)WalletManager.instance.GetWalletAmount());
    }

    private void Listeners()
    {
        btn_AddMoney.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.AddMoney);
        });
        btnBack.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
        });

        transBtn.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Transaction);
        });
        btn_Widraw.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Withdraw);
        });
        helpBtn.onClick.AddListener(() =>
        {
            Application.OpenURL("mailto:rvgamingllc@gmail.com");
        });
    }
}