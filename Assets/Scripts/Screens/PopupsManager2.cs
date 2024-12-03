using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class PopupsManager2 : MonoBehaviour
{
    [SerializeField]
    private List<Transform> allPopups;

    public GameObject lastScreenRef;
    public GameObject activePopup;

    private bool isShowing;
    private static PopupsManager2 _Instance;
    public static PopupsManager2 Instance
    { get { return _Instance; } }

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        HideAll();
        //ClearAll();
    }

    private void Start()
    {
        isShowing = false;
        activePopup = null;
    }

    //*****************************************************************************
    private void HideAll()
    {
        allPopups = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            allPopups.Add(transform.GetChild(i));
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    //***************************************************************************

    public void Show(string screenName)
    {
        if (isShowing || activePopup == GetScreenByName(screenName)) return;
        LastHide();
        activePopup = GetScreenByName(screenName);
        CurrentShow();
    }

    public void Hide()
    {
        Debug.Log("Hide called");
        if (activePopup == null) return;

        RectTransform rt = activePopup.GetComponent<RectTransform>();

        //rt.localScale = Vector3.zero;
        rt.DOScale(0, .3f).SetEase(Ease.InBack).OnComplete(() => { activePopup.SetActive(false); activePopup = null; });
    }

    //*********************************************************************************

    //******************************************************************************

    private void LastHide()
    {
        if (activePopup == null) return;
        RectTransform rt = activePopup.GetComponent<RectTransform>();
        lastScreenRef = activePopup;
        activePopup.SetActive(false);
        isShowing = false;
        lastScreenRef = null;
    }

    private void CurrentShow()
    {
        if (activePopup == null) return;
        isShowing = true;
        RectTransform rt = activePopup.GetComponent<RectTransform>();

        rt.localScale = Vector3.zero;
        rt.DOScale(1, .5f).SetEase(Ease.OutBack).OnComplete(OnMoveComplete);

        activePopup.SetActive(true);
        activePopup.transform.SetAsLastSibling();
    }

    private void OnMoveComplete()
    {
        isShowing = false;
        if (lastScreenRef != null)
            lastScreenRef.SetActive(false);
        lastScreenRef = null;
    }

    //********************************************************************************

    private GameObject GetScreenByName(string _name)
    {
        for (int i = 0; i < allPopups.Count; i++)
        {
            if (allPopups[i].gameObject.name == _name)
            {
                return allPopups[i].gameObject;
            }
        }
        return null;
    }
}