
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NativeGallery;

public class GallaryImagePicker : MonoBehaviour
{
    //[SerializeField]
    //List<string> loaclImagePath;

    public Action<Texture2D> OnImageSelected;

    private static GallaryImagePicker _Instance;
    public static GallaryImagePicker Instance { get { return _Instance; } }
    public bool isImageUploading;
    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        //IsPlayerRegisterFromOperator = false;
        isImageUploading = false;

    }
    
    //******************************************************************************
    public void LoadImageFromGallary()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            PickLocalImage();
        }
        else
        {
            PickImage(1024);
        }
    }
    //****************************************************************
    void PickLocalImage()
    {
        var extensions = new[] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),

        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
        
    }

    private IEnumerator OutputRoutine(string url)
    {
        var loader = new WWW(url);
        while (!loader.isDone)
            yield return null;

        Texture2D texture = (Texture2D)loader.texture;
        if (OnImageSelected != null)
        {
            OnImageSelected(texture);
        }
    }

    //********************************************************************************
    private IEnumerator TakeScreenshotAndSave()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        // Save the screenshot to Gallery/Photos
        Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(ss, "GalleryTest", "Image.png"));

        // To avoid memory leaks
        Destroy(ss);
    }




    private void PickImage(int maxSize)
    {
        NativeGallery.Permission per = NativeGallery.CheckPermission(PermissionType.Write, MediaType.Image);
        if (per == NativeGallery.Permission.ShouldAsk)
        {
            per = NativeGallery.RequestPermission(PermissionType.Write, MediaType.Image);
            Debug.Log("Asking");
        }

        isImageUploading = true;
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                if (OnImageSelected != null)
                {
                    OnImageSelected(texture);
                }

                isImageUploading = false;
                //Destroy(texture, 5f);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);
    }

    private void PickVideo()
    {
        NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
        {
            Debug.Log("Video path: " + path);
            if (path != null)
            {
                // Play the selected video
                Handheld.PlayFullScreenMovie("file://" + path);
            }
        }, "Select a video");

        Debug.Log("Permission result: " + permission);
    }
}
