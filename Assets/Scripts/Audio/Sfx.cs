using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// SFX component for objects able to call Release by themselves (e.g. animated sprite with animation event at the end)
public class Sfx : MonoBehaviour, IPooledObject
{
    /* Sibling components */

    private AudioSource m_AudioSource;


    private void Awake()
    {
        m_AudioSource = this.GetComponentOrFail<AudioSource>();
    }


    /* IPooledObject interface */

    public void Acquire() {}

    public bool IsInUse()
    {
        return m_AudioSource.isPlaying;
    }

    public void Release()
    {
        m_AudioSource.Stop();
    }


    /* Own methods */

    public void PlayOneShot(AudioClip clip)
    {
        m_AudioSource.PlayOneShot(clip);
    }
}
