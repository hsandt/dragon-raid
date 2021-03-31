using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D on projectiles: handles pooling and impact
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
