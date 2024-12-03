using UnityEngine;
public class DoNotDestroy : MonoBehaviour
{
    [SerializeField]
    bool isDontDestroyAllow;
    void Awake()
    {
        if (isDontDestroyAllow)
        {
            DontDestroyOnLoad(this);
        }

    }

}
