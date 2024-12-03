using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public bool canVibrate;


    [Space(5)]
    public List<Sound> sounds = new List<Sound>();


    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    private void Start()
    {
        CreateAudioSources();
        Button[] button = Resources.FindObjectsOfTypeAll<Button>();

        foreach (var item in button)
        {
            item.onClick.AddListener(() => AudioManager.Instance.PlaySound("ButtonClick"));
        }
    }


    void CreateAudioSources()
    {
        foreach (Sound s in sounds)
        {
            GameObject sound = new GameObject();
            sound.name = s.audioName;
            sound.transform.SetParent(this.transform);
            sound.AddComponent<AudioSource>();

            AudioSource source = sound.GetComponent<AudioSource>();

            source.clip = s.audioClip;
            source.volume = s.volume;
            source.playOnAwake = false;

            s.source = source;

        }
    }


    public void PlaySound(string Soundname)
    {
        //Sound s = Array.Find(sounds, sound => sound.audioName == Soundname);
        Sound s = sounds.Find(p => p.audioName == Soundname);
        if (s == null)
        {
           // Debug.LogWarning("Sound with " + Soundname + " not found!!");
            return;
        }
        //Debug.Log("Sound with " + Soundname + " playing!!");

        s.source.Play();
    }

    public AudioSource GetAudioSource(string Soundname)
    {
        Sound s = sounds.Find(p => p.audioName == Soundname);
        if (s == null)
        {
           // Debug.LogWarning("Sound with " + Soundname + " not found!!");
            return null;
        }

        return s.source;
    }


    public void StopSound(string soundName)
    {
        //Sound s = Array.Find(sounds, sound => sound.audioName == soundName);
        Sound s = sounds.Find(p => p.audioName == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound with " + soundName + " not found!!");
            return;
        }

        if (s.source.isPlaying)
            s.source.Stop();


    }

    public void AddSound(string soundName, AudioSource audioSource)
    {
        //Sound s = Array.Find(sounds, sound => sound.audioName == soundName);
        Sound s = sounds.Find(p => p.audioName == soundName);
        if (s == null)
        {
            Sound newSound = new Sound();
            newSound.audioName = soundName;
            newSound.source = audioSource;
            sounds.Add(newSound);
            return;
        }
        else
        {
            s.source = audioSource;
        }
    }

    public void StopAllSound()
    {
        foreach (Sound s in sounds)
        {
            if (s.source.isPlaying)
                s.source.Stop();
        }
    }


    public void MuteAllAudio()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.mute = true;
        }
    }

    public void UnMuteAllAudio()
    {
        foreach (Sound s in sounds)
        {
            if (s.source != null)
                s.source.mute = false;
        }
    }


    public void VibrationToggle(Toggle tg)
    {
        if (tg.isOn)
        {
            canVibrate = true;
            Handheld.Vibrate();
        }
        else
            canVibrate = false;
    }

    public void SoundToggle(Toggle tg)
    {
        if (tg.isOn)
        {
            UnMuteAllAudio();
        }
        else
        {
            StopAllSound();
            MuteAllAudio();
        }

    }

}
