using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    //CardsData m_data;
    public GameObject mFront; //card front
    public GameObject mBack; //Back of card
    public CardState mCardState = CardState.Front; //Card current status, front or back?
    public float mTime = 0.3f;

    private bool isActive = false; //true represents that the flip is being performed and must not be interrupted
    //CardState currentState;
   
    public void InitCard()
    {
        mTime = GameSetting.Instance.cardFlipTime;
        mCardState = CardState.Back;
        isActive = false;
        if (mCardState == CardState.Front)
        {
            // If you start from the front, rotate the back 90 degrees so that you can't see the back.
            mFront.transform.eulerAngles = Vector3.zero;
            mBack.transform.eulerAngles = new Vector3(0, 90, 0);
        }
        else
        {
            // Starting from the back, the same thing
            mFront.transform.eulerAngles = new Vector3(0, 90, 0);
            mBack.transform.eulerAngles = Vector3.zero;
        }
    }
    /*public void StartBack()
    {
        if (isActive)
            return;
        StartCoroutine(ToBack());
    }*/
    
    
    /// Turn over to the back
   
    /*IEnumerator ToBack()
    {
        isActive = true;
        mFront.transform.DORotate(new Vector3(0, 90, 0), mTime);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
        mBack.transform.DORotate(new Vector3(0, 0, 0), mTime);
        isActive = false;

    }  */  
    /// Turn to the front    
    IEnumerator ToFront()
    {
        isActive = true;
        mBack.transform.DORotate(new Vector3(0, 90, 0), mTime);
        for (float i = mTime; i >= 0; i -= Time.deltaTime)
            yield return 0;
        mFront.transform.DORotate(new Vector3(0, 0, 0), mTime);
        isActive = false;
    }



    public void FlipFrontCard(Sprite s)
    {
        if (isActive)
            return;
        mFront.GetComponent<Image>().sprite = s;
        StartCoroutine(ToFront());
    }

    public void ResetCard()
    {
        mCardState = CardState.Back;
        isActive = false;
        mFront.transform.eulerAngles = new Vector3(0, 90, 0);
        mBack.transform.eulerAngles = Vector3.zero;
    }

}

public enum CardState
{
    Front,
    Back
}
