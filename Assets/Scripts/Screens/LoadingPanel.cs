using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject rootContainer;

    [SerializeField]
    private RectTransform rectComponent;


    private float rotateSpeed = 250f;

    bool isActive;

    private void Awake()
    {
        Server.LoadingStart += OnLoadingStart;
        Server.LoadingEnd += OnLoadingEnd;
        rootContainer.SetActive(false);
        isActive = false;
    }
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    private void OnDestroy()
    {

        Server.LoadingStart -= OnLoadingStart;
        Server.LoadingEnd -= OnLoadingEnd;
    }

    private void Update()
    {
        if (isActive)
        {
            rectComponent.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);
        }
    }

    void OnLoadingStart()
    {
        //Debug.Log("OnLoadingStart");
        rootContainer.SetActive(true);
        isActive = true;
        StartCoroutine(startLoadingAfterWait());

    }

    void OnLoadingEnd()
    {
        rootContainer.SetActive(false);
        isActive = false;
        StartCoroutine(endLoadingAfterWait());
    }

    IEnumerator startLoadingAfterWait()
    {
        yield return new WaitForEndOfFrame();
        rootContainer.SetActive(true);
        isActive = true;
    }

    IEnumerator endLoadingAfterWait()
    {
        yield return new WaitForEndOfFrame();
        rootContainer.SetActive(false);
        isActive = false;
    }
}
