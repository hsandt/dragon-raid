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

    private EnemyCharacterMaster m_EnemyCharacterMaster;

    
    /* State */
    
    /// Timer counting down toward end of invincibility
    private Timer m_InvincibilityTimer;

    /// Is the character invincible?
    public bool IsInvincible => m_InvincibilityTimer.HasTimeLeft;
    
    
    private void Awake()
    {
        // Currently, all objects with a Health system are released via pooling
        m_PooledObject = GetComponent<IPooledObject>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_PooledObject != null, this,
            "[HealthSystem] No component of type IPooledObject found on {0}", gameObject);
        #endif
        
        m_Health = this.GetComponentOrFail<Health>();
        m_Health.maxValue = healthParameters.maxHealth;
        
        m_Brighten = this.GetComponentOrFail<Brighten>();

        m_EnemyCharacterMaster = GetComponent<EnemyCharacterMaster>();
        
        m_InvincibilityTimer = new Timer(callback: m_Brighten.ResetBrightness);
        
        // Relies on InGameManager being ready, thanks to its SEO being before Character Pool Managers who will
        // Awake this component immediately on pooled object creation in their Init
        m_healthSharedParameters = InGameManager.Instance.healthSharedParameters;
    }

    public override void Setup()
    {
        m_Health.value = m_Health.maxValue;
        // no need to setup m_Brighten, it is another slave managed by Character Master
        
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
    /// This is private as you should always check for invincibility and apply visual feedbackk
    /// via the Try...Damage methods
    private void Damage(int value)
    {
        // Only damage if character is not already dead i.e. HP are not already at 0
        // This will prevent unwanted redundant calls to Die causing weird things like Clear-ing
        // EnemyCharacterMaster.m_EnemyWave then erroring in OnDeathOrExit because m_EnemyWave == null 
        if (value > 0)
        {
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
    public void Kill()
    {
        Damage(m_Health.value);
    }
    #endif

    /// Apply one-shot damage and return whether it worked or not
    public bool TryOneShotDamage(int value)
    {
        if (IsInvincible)
        {
            return false;
        }

        Damage(value);
        
        if (m_Health.value > 0)
        {
            // Entity survived, so play damage feedback
            m_Brighten.SetBrightnessForDuration(m_healthSharedParameters.damagedBrightness, m_healthSharedParameters.damagedBrightnessDuration);
        }

        return true;
    }

    /// Apply periodic damage and return whether it worked or not
    public bool TryPeriodicDamage(int value)
    {
        if (IsInvincible)
        {
            return false;
        }
        
        Damage(value);
        
        if (m_Health.value > 0)
        {
            // Entity survived, so start invincibility phase
            // Indeed, this is periodic damage (like body attacks) so if the character stays
            // under a certain area they'll keep getting hit, and without the invincibility
            // timer they would get hit every frame and die too fast.
            // Note that this can lead to odd behaviors like surviving longer by staying in a
            // danger zone because it helps you not getting hit by many projectiles.
            m_InvincibilityTimer.SetTime(m_healthSharedParameters.postBodyAttackInvincibilityDuration);
            
            // Set the brightness without timer: the invincibility timer will take care of resetting it
            m_Brighten.SetBrightness(m_healthSharedParameters.damagedBrightness);
        }

        return true;
    }

    private void Die()
    {
        if (m_EnemyCharacterMaster)
        {
            m_EnemyCharacterMaster.OnDeathOrExit();
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
