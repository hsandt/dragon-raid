using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// System for Health data component: handles damage
public class HealthSystem : MonoBehaviour
{
    /* Sibling components */
    
    private Health m_Health;
    
    
    private void Awake()
    {
        m_Health = this.GetComponentOrFail<Health>();
    }

    public void Damage(int value)
    {
        m_Health.value -= value;
        if (m_Health.value <= 0)
        {
            m_Health.value = 0;
        }
    }
}
