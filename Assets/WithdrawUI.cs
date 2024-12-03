using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawUI : UIScreens
{
    public Text txt_amt;
    public InputField amount;
    public Button withdraw, back;
    private List<string> m_accounts = new List<string>();
    public Dropdown accountsDropdown;

    private void Start()
    {
        Listners();
        foreach (string accounts in DataManager.Instance.UserDetailsData.userdata.paypal_ids)
        {
            Debug.Log(accounts);
            m_accounts.Add(accounts);
            accountsDropdown.AddOptions(m_accounts);
        }
    }

    private void OnEnable()
    {
        txt_amt.text = "$ " + DataManager.Instance.UserDetailsData.userdata.wallet.ToString();
        accountsDropdown.ClearOptions();
        accountsDropdown.AddOptions(m_accounts);
    }

    private void Listners()
    {
        withdraw.onClick.AddListener(() =>
        {
            Debug.Log(accountsDropdown.options[accountsDropdown.value].text);
            WalletManager.instance.WithdrawRequest(amount.text, accountsDropdown.options[accountsDropdown.value].text);
        });

        back.onClick.AddListener(() =>
        {
            GameEvents.EventScreenshow?.Invoke(ScreenType.Home);
        });
    }
}