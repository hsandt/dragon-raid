using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// FX component for objects able to call Release by themselves (e.g. animated sprite with animation event at the end)
public class FX : MonoBehaviour, IPooledObject
{
    /* IPooledObject interface */
    
    public void InitPooled()
    {
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

    public void Spawn(Vector2 position)
    {
        gameObject.SetActive(true);
        
        transform.position = position;
    }
}
