using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// System component for pick-up items
public class PickUp : MonoBehaviour, IPooledObject
{
    [Header("Parameters data")]
    
    [Tooltip("Pick-up Parameters Data")]
    public PickUpParameters pickUpParameters;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(pickUpParameters != null, this, "[PickUpSystem] No Pick-up Parameters asset set on {0}", this);
        #endif

        // Check that item is still alive, to avoid being picked twice in the same frame
        // (although it's not possible with a single player character anyway)
        if (IsInUse())
        {
            var pickUpCollector = other.GetComponent<PickUpCollector>();
            if (pickUpCollector != null)
            {
                pickUpCollector.Pick(this);
                Release();
            }
        }
    }

    
    /* IPooledObject interface */
    
    public void InitPooled()
    {
        // Nothing special for now
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
