using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSet : MonoBehaviour
{
    public static CardSet Instance;
    public Sprite ABC;
    // Start is called before the first frame update
   
    public void SetParam(string s)
    {
        GetComponent<Animator>().runtimeAnimatorController.animationClips[0].events[0].stringParameter = s;
    }
    public void ChangeSprite(string spriteName)
    {
        Debug.Log("sprite Name = " + spriteName);
        ABC = Resources.Load<Sprite>("Images/"+spriteName);
        gameObject.GetComponent<Image>().sprite = ABC; //Resources.Load("ABC") as Sprite;
    }
}
