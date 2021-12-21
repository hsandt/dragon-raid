using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Health data component: handles any changes
public class HealthSystem : ClearableBehaviour
{
    [Header("Parameters data")]
    
    [Tooltip("Health Parameters Data")]
    public HealthParameters healthParameters;

    [Tooltip("Health Aesthetic Parameters Data")]
    public HealthAestheticParameters healthAestheticParameters;

    
    /* Cached asset references */

    private HealthSharedParameters m_healthSharedParameters;

    
    /* Dynamic external references */
    
    /// List of health gauges observing health data
    private readonly List<GaugeHealth> m_GaugeHealthList = new List<GaugeHealth>();
    
    /// Optional death event effect
    /// A given type of entity always has the same death event effect, set once on EventTrigger_EntityDeath.Awake,
    /// this is not reset on Clear so it can still be valid after despawn and respawn.
    private IEventEffect m_OnDeathEventEffect;

    
    /* Sibling components (required) */

    private IPooledObject m_PooledObject;
    private Health m_Health;
    private Brighten m_Brighten;
    
    
    /* Sibling components (optional) */

    private CharacterMaster m_CharacterMaster;
    private CookSystem m_CookSystem;
    
    
    /* State */
    
    /// Timer counting down toward end of invincibility
    private Timer m_InvincibilityTimer;

    /// Is the character invincible?
    private bool IsInvincible => m_InvincibilityTimer.HasTimeLeft;
    
    
    private void Awake()
    {
        // Currently, all objects with a Health system are released via pooling
        m_PooledObject = GetComponent<IPooledObject>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_PooledObject != null, this,
            "[HealthSystem] No component implementing IPooledObject found on {0}", gameObject);
        
        Debug.AssertFormat(healthParameters != null, this, "[HealthSystem] No Health Parameters asset set on {0}", this);
        #endif
        
        m_Health = this.GetComponentOrFail<Health>();
        m_Health.maxValue = healthParameters.maxHealth;
        
        m_Brighten = this.GetComponentOrFail<Brighten>();

        m_CharacterMaster = GetComponent<CharacterMaster>();
        m_CookSystem = GetComponent<CookSystem>();
        
        m_InvincibilityTimer = new Timer(callback: m_Brighten.ResetBrightness);
        
        // Relies on InGameManager being ready, thanks to its SEO being before Character Pool Managers who will
        // Awake this component immediately on pooled object creation in their Init
        m_healthSharedParameters = InGameManager.Instance.healthSharedParameters;
    }

    public override void Setup()
    {
        m_Health.value = m_Health.maxValue;
        NotifyValueChangeToObservers();  // only needed on Respawn
        
        m_InvincibilityTimer.Stop();
    }

    private void FixedUpdate()
    {
        m_InvincibilityTimer.CountDown(Time.deltaTime);
    }

    public int GetValue()
    {
        return m_Health.value;
    }

    public float GetRatio()
    {
        return (float) m_Health.value / m_Health.maxValue;
    }

    /// Low-level function to deal damage, check death and update observers
    /// This is private as you should always check for invincibility and apply visual feedback
    /// via the Try...Damage methods
    private void TakeDamage(int value, ElementType elementType)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(CanBeDamaged());
        #endif
        
        if (value > 0)
        {
            // If this entity is cookable and damaged by fire, advance cook progress
            // We must do this before death check as death must spawn cooked enemy based on the latest cook progress
            if (m_CookSystem != null && elementType == ElementType.Fire)
            {
                // Note that this is the unclamped value
                // This way, an overkill attack on an enemy with low HP will still cook a lot!
                m_CookSystem.AdvanceCookProgress(value);
            }
            
            m_Health.value -= value;

            if (m_Health.value <= 0)
            {
                m_Health.value = 0;
                Die();
            }
    
            NotifyValueChangeToObservers();
        }
    }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    /// Kill unit instantly
    public void Cheat_TryBeKilled()
    {
        if (!CanBeDamaged())
        {
            return;
        }
        
        TakeDamage(m_Health.value, ElementType.Neutral);
    }
    #endif

    /// Apply one-shot damage and return whether it worked or not
    public bool TryTakeOneShotDamage(int value, ElementType elementType)
    {
        if (!CanBeDamaged())
        {
            return false;
        }

        TakeDamage(value, elementType);
        
        if (m_Health.value > 0)
        {
            // Entity survived, so play damage feedback
            m_Brighten.SetBrightnessForDuration(m_healthSharedParameters.damagedBrightness, m_healthSharedParameters.damagedBrightnessDuration);
        }

        return true;
    }

    /// Apply periodic damage and return whether it worked or not
    public bool TryTakePeriodicDamage(int value, ElementType elementType)
    {
        if (!CanBeDamaged())
        {
            return false;
        }
        
        TakeDamage(value, elementType);
        
        if (m_Health.value > 0)
        {
            // Entity survived, so start invincibility phase
            // Indeed, this is periodic damage (like body attacks) so if the character stays
            // under a certain area they'll keep getting hit, and without the invincibility
            // timer they would get hit every frame and die too fast.
            // Note that this can lead to odd behaviors like surviving longer by staying in a
            // danger zone because it helps you not getting hit by many projectiles.
            // This can be avoided by always allowing one-shot damages, but currently periodic invincibility
            // applies to all further incoming damages.
            m_InvincibilityTimer.SetTime(m_healthSharedParameters.postPeriodicDamageInvincibilityDuration);
            
            // Set the brightness without timer: the invincibility timer will take care of resetting it
            m_Brighten.SetBrightness(m_healthSharedParameters.damagedBrightness);
        }

        return true;
    }

    private bool CanBeDamaged()
    {
        // Verify that the pooled object is active (a bit more generic than health > 0,
        // as it also covers other reasons to Release than dying, e.g. exiting Living Zone;
        // although this particular one has its SEO set after Default Time, so melee attacks
        // and projectiles would have priority when hitting target on the same frame as exit)
        // Also verify that the health system is not currently invincible,
        // and that we are not playing a flow sequence that should not allow damage.
        return m_PooledObject.IsInUse() && !IsInvincible && InGameManager.Instance.CanAnyEntityBeDamagedOrHealed;
    }

    private bool CanRecover()
    {
        return m_PooledObject.IsInUse() && InGameManager.Instance.CanAnyEntityBeDamagedOrHealed;
    }

    private void Die()
    {
        if (m_CharacterMaster)
        {
            m_CharacterMaster.OnDeathOrExit();
        }
        
        if (healthAestheticParameters != null)
        {
            if (healthAestheticParameters.fxDeath != null)
            {
                // Visual: play death FX
                // Note that we only care about the name because pooling stores resources by name,
                // but we keep fxDeath as a GameObject field to force designer to select an existing object
                FXPoolManager.Instance.SpawnFX(healthAestheticParameters.fxDeath.name, transform.position);
            }
            
            if (healthAestheticParameters.sfxDeath != null)
            {
                // Audio: play death SFX
                SfxPoolManager.Instance.PlaySfx(healthAestheticParameters.sfxDeath);
            }
        }
        
        m_OnDeathEventEffect?.Trigger();
        
        // Always Release after other signals as those may need members cleared in Release
        m_PooledObject.Release();
        
        // If cookable and cooked enough, spawn cooked enemy with randomness
        if (m_CookSystem != null)
        {
            m_CookSystem.RandomSpawnCookedEnemyForCurrentProgress();
        }
    }

    private void Recover(int value)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(CanRecover());
        #endif

        // Recover health up to max value
        m_Health.value = Mathf.Min(m_Health.value + value, m_Health.maxValue);
        
        NotifyValueChangeToObservers();
    }
    
    public void TryRecover(int value)
    {
        if (!CanRecover())
        {
            return;
        }
        
        Recover(value);
    }
    
    /* Observer pattern */
    
    public void RegisterObserver(GaugeHealth gaugeHealth)
    {
        if (!m_GaugeHealthList.Contains(gaugeHealth))
        {
             m_GaugeHealthList.Add(gaugeHealth);
        }
    }
    
    public void UnregisterObserver(GaugeHealth gaugeHealth)
    {
        if (m_GaugeHealthList.Contains(gaugeHealth))
        {
            m_GaugeHealthList.Remove(gaugeHealth);
        }
    }
    
    private void NotifyValueChangeToObservers()
    {
        foreach (GaugeHealth gaugeHealth in m_GaugeHealthList)
        {
            gaugeHealth.RefreshGauge();
        }
    }

    public void RegisterOnDeathEventEffect(IEventEffect eventEffect)
    {
        m_OnDeathEventEffect = eventEffect;
    }
}
