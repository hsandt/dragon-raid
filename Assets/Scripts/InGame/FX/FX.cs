using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// FX component for objects able to call Release by themselves (e.g. animated sprite with animation event at the end)
/// It is now a MasterBehaviour, only to auto-manage Animator and ParticleSystem for Pause/Resume, since FX
/// may use one or the other to represent a visual effect.
public class FX : MasterBehaviour, IPooledObject
{
    /* IPooledObject interface */

    public void Acquire()
    {
        gameObject.SetActive(true);
    }

    public bool IsInUse()
    {
        return gameObject.activeSelf;
    }

    public void Release()
    {
        gameObject.SetActive(false);
    }


    /* Own methods */

    public void Warp(Vector2 position)
    {
        transform.position = position;
    }
}
