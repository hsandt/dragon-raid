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
            int maxCookLevelThreshold = cookParameters.cookLevelThresholds[cookParameters.cookLevelThresholds.Length - 1];
            m_CookStatus.cookProgress = Mathf.Min(m_CookStatus.cookProgress + value, maxCookLevelThreshold); 
        }
    }

    /// Spawn Cooked Enemy pickup for the current progress, or do nothing based on random result
    /// Call this when the enemy dies
    public void RandomSpawnCookedEnemyForCurrentProgress()
    {
        if (Random.value <= cookParameters.cookedEnemySpawnProbability)
        {
            CookLevel cookLevel = GetCurrentCookLevel();
            
            // Ex: "CookedEnemy_CookLevel1"
            PickUpPoolManager.Instance.SpawnPickUp($"CookedEnemy_CookLevel{(int) cookLevel}", transform.position);
        }
    }

    private CookLevel GetCurrentCookLevel()
    {
        // Threshold pattern
        for (int i = 0; i < cookParameters.cookLevelThresholds.Length; ++i)
        {
            int threshold = cookParameters.cookLevelThresholds[i];

            // Upper threshold excludes current level, so <
            if (m_CookStatus.cookProgress < threshold)
            {
                return (CookLevel) i;
            }
        }

        // Last threshold reached
        return CookLevel.Carbonized;
    }
}
