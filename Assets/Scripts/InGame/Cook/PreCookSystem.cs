using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for CookStatus data component
/// It is used by a cookable enemy while still alive, to decide how to spawn the Cooked Enemy prefab
public class PreCookSystem : ClearableBehaviour
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
        m_CookStatus.maxCookProgress = cookParameters.cookLevelThresholds[cookParameters.cookLevelThresholds.Length - 1];
        m_CookStatus.cookProgress = 0;
    }

    public void AdvanceCookProgress(int value)
    {
        if (value > 0)
        {
            m_CookStatus.cookProgress = Mathf.Min(m_CookStatus.cookProgress + value, m_CookStatus.maxCookProgress); 
        }
    }

    /// Spawn Cooked Enemy pickup for the current progress, or do nothing based on random result
    /// Call this when the enemy dies
    public void RandomSpawnCookedEnemyForCurrentProgress()
    {
        if (Random.value <= cookParameters.cookedEnemySpawnProbability)
        {
            // Spawn prefab as general pick up
            PickUp pickUp = PickUpPoolManager.Instance.SpawnPickUp("CookedEnemy", transform.position);
            
            // Initialize CookedEnemy component
            var cookedEnemy = pickUp.GetComponentOrFail<CookedEnemy>();
            cookedEnemy.Init(cookParameters, m_CookStatus.cookProgress);
        }
    }
}
