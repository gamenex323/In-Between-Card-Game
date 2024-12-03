using UnityEngine.UI;

public class AddMoneyScreen : UIScreens
{
    public Text txt_Amount;

    public Button Btn_AddMoney, btnBack, btn10, btn50, btn100;

    private void Start()
    {
        Listeners();
    }

    private void OnEnable()
    {
        OnWalletAmountUpdated();
    }

    private void Listeners()
    {
        Btn_AddMoney.onClick.AddListener(() =>
        {
            //Application.OpenURL("");
        });
        btnBack.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
        });

        btn10.onClick.AddListener(() =>
        {
            //Server.Instance.UpdateBalancePurchase(0);
            //WalletManager.instance.UpdateWallet("10");
        });
        btn50.onClick.AddListener(() =>
        {
            //Server.Instance.UpdateBalancePurchase(1);

            //WalletManager.instance.UpdateWallet("50");
        });
        btn100.onClick.AddListener(() =>
        {
            //Server.Instance.UpdateBalancePurchase(2);

            //WalletManager.instance.UpdateWallet("100");
        });
    }

    private void OnWalletAmountUpdated()
    {
        txt_Amount.text = Utility.AbbreviateNumber((float)WalletManager.instance.GetWalletAmount());
    }
}