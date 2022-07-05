using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for CookStatus data component
/// It is used by a cookable enemy while still alive, to decide how to spawn the Cooked Enemy prefab
public class PreCookSystem : ClearableBehaviour, IDamageHandler, IDeathHandler
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


    /* IDamageHandler */

    public void OnDamage(DamageInfo damageInfo)
    {
        // If this entity is damaged by fire, advance cook progress to prepare spawning cooked enemy
        // (hence *pre*-cook system).
        // OnDamage is called before death check, so death will spawn any cooked enemy based on the latest cook progress
        if (damageInfo.elementType == ElementType.Fire)
        {
            int value = damageInfo.damage;
            if (value > 0)
            {
                // Advance cook progress with unclamped damage
                // This way, an overkill attack on an enemy with low HP will still cook a lot!
                m_CookStatus.cookProgress = Mathf.Min(m_CookStatus.cookProgress + value, m_CookStatus.maxCookProgress);
            }
        }
    }


    /* IDeathHandler */

    public void OnDeath(DamageInfo damageInfo)
    {
        // Spawn Cooked Enemy pickup based on current cook progress, or do nothing based on random result
        if (Random.value <= cookParameters.cookedEnemySpawnProbability)
        {
            // Spawn prefab as general pick ups
            PickUp pickUp = PickUpPoolManager.Instance.SpawnPickUp("CookedEnemy", transform.position);

            // Initialize CookedEnemy component
            var cookedEnemy = pickUp.GetComponentOrFail<CookedEnemy>();
            cookedEnemy.Init(cookParameters, m_CookStatus.cookProgress);
        }
    }
}
