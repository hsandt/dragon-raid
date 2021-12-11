using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// System component for entity picking up items on touch
public class PickUpCollector : MonoBehaviour
{
    public void Pick(PickUp pickUp)
    {
        Debug.LogFormat("Pick: {0}", pickUp);
    }
}
