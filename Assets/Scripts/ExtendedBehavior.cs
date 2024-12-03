using System;
using System.Collections;
using UnityEngine;

public class ExtendedBehavior : MonoBehaviour
{
    public void Wait(float seconds, Action action)
    {
        StartCoroutine(_wait(seconds, action));
    }
    IEnumerator _wait(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }



    public void LoadImage(string url, Action<Sprite> sprite)
    {
        
        StartCoroutine(Load(url, sprite));
    }

    IEnumerator Load(string url, Action<Sprite> callback)
    {
        //Server._LoadingStart();
        WWW www = new WWW(url);
        yield return www;

        Texture2D texture = www.texture;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        callback(sprite);
        www.Dispose();
        www = null;
        //Server._LoadingEnd();
        
    }
}
