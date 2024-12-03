using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningAnimationEvent : MonoBehaviour
{
    [SerializeField] PlayerObj playerObj;
    public void OnAnimationCompleted()
    {
        gameObject.SetActive(false);
        playerObj.OnWinAnimationCompleted();
    }
}
