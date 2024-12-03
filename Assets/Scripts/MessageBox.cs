using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{

    [SerializeField]
    GameObject msgBox;

    [SerializeField]
    Text msgTxt;

    [SerializeField]
    Button okBtn;

    [SerializeField]
    Button cancelBtn;

    public Action OnOkClicked;
    private static MessageBox _Instance;
    public static MessageBox Instance { get { return _Instance; } }
    //------------------------------------------------------
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);           
        }
        okBtn.onClick.AddListener(OnOk);
        cancelBtn.onClick.AddListener(OnClose);
    }

    private void OnDestroy()
    {
        okBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
    }

    //--------------------------------------------------------------
    MsgType msgType;
    public void Show(string _msg = "", MsgType _popupType = MsgType.NONE)
    {
        print("Showing Popup");
        HideAll();
        if(_popupType == MsgType.ALERT)
        {
            FindObjectOfType<Alert>().alTxt.text = _msg;
            StartCoroutine(AutoAlertHide());
            return;
        }
        msgType = _popupType;
        msgBox.SetActive(true);
        msgBox.transform.localScale = Vector3.zero;
        msgBox.transform.DOScale(1, .5f).SetEase(Ease.OutBack);
       
        //Debug.Log(_msg);
        msgTxt.text = _msg;
      
        setOkText("OK");
        switch (_popupType)
        {
            case MsgType.NONE:
                //okBtn.gameObject.SetActive(true);
                StartCoroutine(AutoHide());
                break;
            case MsgType.OKONLY:
                okBtn.gameObject.SetActive(true);                
                break;
            case MsgType.OKCANCELBOTH:
                setOkText("OK");
                setCancelText("Cancel");
                okBtn.gameObject.SetActive(true);
                cancelBtn.gameObject.SetActive(true);
                break;
            case MsgType.YESNOBOTH:
                setOkText("Yes");
                setCancelText("No");
                okBtn.gameObject.SetActive(true);
                cancelBtn.gameObject.SetActive(true);
                break;

        }

       
    }

    public void setOkText(string _str)
    {
        okBtn.GetComponentInChildren<Text>().text = _str;
    }
    public void setCancelText(string _str)
    {
        cancelBtn.GetComponentInChildren<Text>().text = _str;
    }

    IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(3f);
        OnClose();
    }

    IEnumerator AutoAlertHide()
    {
        yield return new WaitForSeconds(1.5f);
        FindObjectOfType<Alert>().alTxt.text = "";
    }

    void OnOk()
    {
        if(OnOkClicked!= null)
        {
            OnOkClicked();
        }

        OnClose();
    }

    void OnClose()
    {
        OnOkClicked = null;

        msgBox.SetActive(false);
    }


    void HideAll()
    {
        okBtn.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
    }


}
