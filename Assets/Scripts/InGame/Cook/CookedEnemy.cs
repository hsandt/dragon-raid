using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for CookStatus data component
/// It contains the behaviour for an actual CookedEnemy prefab instance
public class CookedEnemy : MonoBehaviour, IPickUpEffect, IProjectileImpactHandler
{
    /* Animator hashes */

    private static readonly int cookLevelHash = Animator.StringToHash("CookLevel");


    [Header("Injected parameters data")]

    [ReadOnlyField, Tooltip("Cook Parameters Data. Should be injected by spawning code.")]
    public CookParameters cookParameters;


    /* Sibling components */

    private IPooledObject m_PooledObject;
    private Animator m_Animator;
    private CookStatus m_CookStatus;


    /* Cached state */

    /// Cook level (cached from cook progress)
    private CookLevel m_CookLevel;


    private void Awake()
    {
        m_PooledObject = GetComponent<IPooledObject>();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_PooledObject != null, gameObject,
            "[CookedEnemy] No component implementing IPooledObject found on {0}", gameObject);
        #endif

        m_Animator = this.GetComponentOrFail<Animator>();
        m_CookStatus = this.GetComponentOrFail<CookStatus>();
    }


    /* Own methods */

    /// Initialise instance parameters
    /// Note that we must already have spawned the object with PickUpPoolManager.SpawnPickUp at a given position
    /// so we only need to set the dynamic parameters and initialize the state now.
    public void Init(CookParameters dyingEnemyCookParameters, int cookProgress)
    {
        // Inject dynamic parameter
        cookParameters = dyingEnemyCookParameters;

        // Same as PreCookSystem.Setup
        m_CookStatus.maxCookProgress = cookParameters.cookLevelThresholds[cookParameters.cookLevelThresholds.Length - 1];

        // Transfer existing progress from dying enemy, and set cook level based on it
        m_CookStatus.cookProgress = cookProgress;
        InitCookLevel();
    }

    private bool CanBeDamaged()
    {
        return m_PooledObject.IsInUse() && InGameManager.Instance.CanAnyEntityBeDamagedOrHealed;
    }

    /// Apply one-shot damage and return whether it worked or not
    /// It is similar to HealthSystem.TryTakeOneShotDamage, but it only tracks cook progress
    public bool TryTakeOneShotDamage(DamageInfo damageInfo)
    {
        if (!CanBeDamaged())
        {
            return false;
        }

        TakeDamage(damageInfo);

        return true;
    }

    /// Low-level function to deal damage, check death and update observers
    /// This is private as you should always check CanBeDamaged via the Try...Damage methods
    /// It is similar to HealthSystem.TryTakeOneShotDamage, but it only tracks cook progress
    private void TakeDamage(DamageInfo damageInfo)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(CanBeDamaged());
        #endif

        if (damageInfo.damage > 0)
        {
            // If this entity is cookable and damaged by fire, advance cook progress
            if (damageInfo.elementType == ElementType.Fire)
            {
                AdvanceCookProgress(damageInfo.damage);
                RefreshCookLevel();
            }
        }
    }

    private void AdvanceCookProgress(int value)
    {
        // Same as PreCookSystem.AdvanceCookProgress
        if (value > 0)
        {
            m_CookStatus.cookProgress = Mathf.Min(m_CookStatus.cookProgress + value, m_CookStatus.maxCookProgress);
        }
    }

    // Return cook level expected from current cook progress
    private CookLevel GetRequiredCookLevel()
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

    /// Set cook level from current progress, no matter what
    private void InitCookLevel()
    {
        CookLevel newCookLevel = GetRequiredCookLevel();
        SetCookLevel(newCookLevel);
    }

    /// Set cook level from current progress, if it changed (for performance when called often)
    private void RefreshCookLevel()
    {
        CookLevel newCookLevel = GetRequiredCookLevel();
        if (m_CookLevel != newCookLevel)
        {
            SetCookLevel(newCookLevel);
        }
    }

    private void SetCookLevel(CookLevel cookLevel)
    {
        m_CookLevel = cookLevel;

        // Update sprite
        m_Animator.SetInteger(cookLevelHash, (int) cookLevel);
    }


    /* IProjectileImpactHandler */

    public bool OnProjectileImpact(DamageInfo damageInfo)
    {
        return TryTakeOneShotDamage(damageInfo);
    }


    /* IPickUpEffect */

    public void OnPick(PickUpCollector pickUpCollector)
    {
        pickUpCollector.TryRecover(cookParameters.healthRecoveryValues[(int) m_CookLevel]);
    }
}
