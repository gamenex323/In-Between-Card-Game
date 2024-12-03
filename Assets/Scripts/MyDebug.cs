using UnityEngine;

public class MyDebug : MonoBehaviour
{
    public bool isDebugOn;
    private static bool show;

    private void Start()
    {
        if (isDebugOn)
        {
            show = true;
            return;
        }
        show = false;
    }

    public static void Log(object obj)
    {
        if (show)
        {
            Debug.Log(obj);
        }
    }
}