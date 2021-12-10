using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for CookStatus data component
public class CookSystem : ClearableBehaviour
{
    [Header("Parameters data")]
    
    [Tooltip("Cook Parameters Data")]
    public CookParameters cookParameters;
    
    
    /* Sibling components (required) */

    private CookStatus m_CookStatus;

    
    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(cookParameters != null, this, "[CookSystem] No Cook Parameters asset set on {0}", this);
        #endif
        
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
            if (m_CookStatus.cookProgress >= cookParameters.wellDoneThreshold)
            {
                Debug.Log("Well done!");
            }
        }
    }
}
