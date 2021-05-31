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
    
    
    /* Sibling components */

    private CharacterMaster m_CharacterMaster;
    private Health m_Health;
    private Brighten m_Brighten;

    
    /* Custom components */
    
    /// Timer counting down toward end of invincibility
    private Timer m_InvincibilityTimer;

    /// Is the character invincible?
    public bool IsInvincible => m_InvincibilityTimer.HasTimeLeft;
    
    
    private void Awake()
    {
        // Cached asset references
        
        m_healthSharedParameters = InGameManager.Instance.healthSharedParameters;
        
        
        // Components
        
        m_CharacterMaster = this.GetComponentOrFail<CharacterMaster>();

        m_Health = this.GetComponentOrFail<Health>();
        m_Health.maxValue = healthParameters.maxHealth;
        
        m_Brighten = this.GetComponentOrFail<Brighten>();
        Debug.AssertFormat(healthAestheticParameters, this, "No Health Aesthetic Parameters found on {0}", this);
        
        m_InvincibilityTimer = new Timer(callback: m_Brighten.ResetBrightness);
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

    public float GetValue()
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
        m_Health.value -= value;
        
        if (m_Health.value <= 0)
        {
            m_Health.value = 0;
            Die();
        }

        NotifyValueChangeToObservers();
    }

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
        m_CharacterMaster.Release();
        
        // Visual: play death FX
        FXPoolManager.Instance.SpawnFX("EnemyDeath", transform.position);
        
        if (healthAestheticParameters != null && healthAestheticParameters.sfxDeath != null)
        {
            // Audio: play death SFX
            SfxPoolManager.Instance.PlaySfx(healthAestheticParameters.sfxDeath);
        }
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
}
