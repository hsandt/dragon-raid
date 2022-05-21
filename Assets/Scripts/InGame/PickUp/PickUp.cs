using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// System component for pick-up items
public class PickUp : MonoBehaviour, IPooledObject
{
    /* Sibling components */

    private IPickUpEffect m_PickUpEffect;


    private void Awake()
    {
        m_PickUpEffect = GetComponent<IPickUpEffect>();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_PickUpEffect != null, gameObject,
            "[PickUp] No component implemented IPickUpEffect found on {0}", gameObject);
        #endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check that item is still alive, to avoid being picked twice in the same frame
        // (although it's not possible with a single player character anyway)
        if (IsInUse())
        {
            var pickUpCollector = other.GetComponent<PickUpCollector>();
            if (pickUpCollector != null)
            {
                GetPickedBy(pickUpCollector);
            }
        }
    }

    public void GetPickedBy(PickUpCollector pickUpCollector)
    {
        m_PickUpEffect.OnPick(pickUpCollector);
        Release();
    }


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
