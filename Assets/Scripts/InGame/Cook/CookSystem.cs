using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for CookStatus data component
public class CookSystem : ClearableBehaviour
{
    /* Sibling components (required) */

    private CookStatus m_CookStatus;

    
    private void Awake()
    {
        m_CookStatus = this.GetComponentOrFail<CookStatus>();
    }
    
    public override void Setup()
    {
        m_CookStatus.maxCookProgress = 10;
        m_CookStatus.cookProgress = 0;
    }

    public void AdvanceCookProgress(int value)
    {
        if (value > 0)
        {
            m_CookStatus.cookProgress += value;
            Debug.LogFormat("Cook progress: {0}", m_CookStatus.cookProgress);
        }
    }
}
