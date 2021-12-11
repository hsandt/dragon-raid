using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;
using UnityEditor;

/// System component for pick-up items
public class PickUp : MonoBehaviour, IPooledObject
{
    private void OnTriggerEnter2D(Collider2D other)
    {
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
    }}
