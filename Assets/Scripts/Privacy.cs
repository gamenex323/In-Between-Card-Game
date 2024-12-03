using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Privacy : MonoBehaviour
{
    Button helpBtn;
    void Start()
    {
        helpBtn = this.GetComponent<Button>();
        helpBtn.onClick.AddListener(() =>
        {
            Application.OpenURL("https://royalvgaming.com/privacy");
        });
    }

   
}
