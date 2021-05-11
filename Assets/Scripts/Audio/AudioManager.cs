using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

public class AudioManager : SingletonManager<AudioManager>
{
    [Header("Children references")]
    
    [Tooltip("Audio Source for BGM")]
    public AudioSource bgmAudioSource;

    [Tooltip("Audio Source for SFX")]
    public AudioSource sfxAudioSource;


    public void PlayBgm(AudioClip bgm)
    {
        if (bgmAudioSource.clip != bgm)
        {
            bgmAudioSource.clip = bgm;
            bgmAudioSource.Play();
        }
    }

    public void PlaySfx(AudioClip sfx)
    {
        // stop any previous SFX (otherwise they can overlap)
        sfxAudioSource.Stop();
        sfxAudioSource.PlayOneShot(sfx);
    }
}