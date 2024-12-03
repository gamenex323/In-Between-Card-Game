using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class Sound
{
    public string audioName;

    public AudioClip audioClip;

    [Range(0f, 1f)]
    public float volume;

    //[Range(0.1f, 3f)]
    //public float pitch;

    //public bool loop;

    //public bool playOnAwake;

    [HideInInspector]
    public AudioSource source;


    public void PlaySound()
    {
        source.Play();
    }

}
