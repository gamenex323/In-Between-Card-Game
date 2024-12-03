using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public List<UIScreens> m_Screens;
    public Queue<UIScreens> m_OpenedScreens = new Queue<UIScreens>();
    public  static UIController instance;
    public Text displayMoney;
   

    private void OnEnable()
    {
        GameEvents.EventScreenshow += ShowScreen;
    }

    private void OnDisable()
    {
        GameEvents.EventScreenshow -= ShowScreen;
    }
    private void Start()
    {
        instance = this;
    }

    private void ShowScreen(ScreenType m_Type)
    {
       
        if (m_OpenedScreens.Count != 0)

        if (  m_OpenedScreens.Count > 0)
        {
            m_OpenedScreens.Dequeue()?.gameObject.SetActive(false);
        }

        m_OpenedScreens.Enqueue(m_Screens.Find(tempScreen => tempScreen.m_type == m_Type));
        m_OpenedScreens.Peek()?.gameObject.SetActive(true);
    }

   
}

public enum ScreenType
{
   Registration,
   Login,
   ResetPassword,
   ChangePassword,
   Home,
   OTP,
   Game,Profile,
    Wallet,
    AddMoney,
    Friends,
    Transaction,
    Withdraw

}