using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// System component for entity picking up items on touch
public class PickUpCollector : MonoBehaviour
{
    /* Sibling components */

    private HealthSystem m_HealthSystem;

    private void Awake()
    {
        m_HealthSystem = this.GetComponentOrFail<HealthSystem>();
    }

    public void TryRecover(int value)
    {
        m_HealthSystem.TryRecover(value);
    }
}
