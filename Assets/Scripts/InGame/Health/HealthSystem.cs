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

    private HealthSharedParameters m_HealthSharedParameters;


    /* Dynamic external references */

    /// List of health gauges observing health data
    private readonly List<GaugeHealth> m_GaugeHealthList = new List<GaugeHealth>();

    /// Optional death event effect
    /// A given type of entity always has the same death event effect, set once on EventTrigger_EntityDeath.Awake,
    /// this is not reset on Clear so it can still be valid after despawn and respawn.
    /// Note that it is similar to m_DeathHandlers, but such effects are generally stored outside this game object,
    /// as part of enemy-wave-specific events.
    private IEventEffectOnDamage m_OnDeathEventEffect;


    /* Sibling components (required) */

    private IPooledObject m_PooledObject;
    private Health m_Health;
    private Brighten m_Brighten;


    /* Sibling components (optional) */

    private CharacterMaster m_CharacterMaster;
    private IDamageHandler[] m_DamageHandlers;
    private IDeathHandler[] m_DeathHandlers;


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
        m_DamageHandlers = GetComponents<IDamageHandler>();
        m_DeathHandlers = GetComponents<IDeathHandler>();

        m_InvincibilityTimer = new Timer(callback: m_Brighten.ResetBrightness);

        // Relies on InGameManager being ready, thanks to its SEO being before Character Pool Managers who will
        // Awake this component immediately on pooled object creation in their Init
        m_HealthSharedParameters = InGameManager.Instance.healthSharedParameters;
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

    private void StartInvincibility(float invincibilityDuration, float brightness)
    {
        m_InvincibilityTimer.SetTime(invincibilityDuration);

        // Set the brightness without timer: the invincibility timer will take care of resetting it
        m_Brighten.SetBrightness(brightness);
    }

    public void StartRespawnInvincibility()
    {
        StartInvincibility(m_HealthSharedParameters.postRespawnInvincibilityDuration,
            m_HealthSharedParameters.damagedBrightness);
    }

    /// Low-level function to deal damage, check death and update observers
    /// This is private as you should always check for invincibility and apply visual feedback
    /// via the Try...Damage methods
    private void TakeDamage(DamageInfo damageInfo)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(CanBeDamaged());
        #endif

        if (damageInfo.damage > 0)
        {
            // Apply damage handler
            // Do this before death check as Die may rely on some changes done by OnDamage,
            // esp. with IDeathHandler.OnDeath calls
            foreach (IDamageHandler damageHandler in m_DamageHandlers)
            {
                damageHandler.OnDamage(damageInfo);
            }

            m_Health.value -= damageInfo.damage;

            if (m_Health.value <= 0)
            {
                m_Health.value = 0;
                Die(damageInfo);
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

        DamageInfo damageInfo = new DamageInfo
        {
            damage = m_Health.value
        };
        TakeDamage(damageInfo);
    }
    #endif

    /// Apply one-shot damage and return whether it worked or not
    /// Pass element type and hit direction of the attack that dealt damage
    /// (if nothing specific, just pass ElementType.Neutral and HorizontalDirection.None)
    public bool TryTakeOneShotDamage(DamageInfo damageInfo)
    {
        if (!CanBeDamaged())
        {
            return false;
        }

        TakeDamage(damageInfo);

        if (m_Health.value > 0)
        {
            // Entity survived, so play damage feedback
            m_Brighten.SetBrightnessForDuration(m_HealthSharedParameters.damagedBrightness, m_HealthSharedParameters.damagedBrightnessDuration);
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

        DamageInfo damageInfo = new DamageInfo
        {
            damage = value,
            elementType = elementType
        };
        TakeDamage(damageInfo);

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
            // However, you'd need to distinguish the different types of invincibility, as respawn invincibility
            // should protected from all types of damage
            StartInvincibility(m_HealthSharedParameters.postPeriodicDamageInvincibilityDuration,
                m_HealthSharedParameters.damagedBrightness);
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

    private void Die(DamageInfo lastDamageInfo)
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

        m_OnDeathEventEffect?.Trigger(lastDamageInfo);

        // Always Release after other signals as those may need members cleared in Release
        m_PooledObject.Release();

        foreach (IDeathHandler deathHandler in m_DeathHandlers)
        {
            deathHandler.OnDeath(lastDamageInfo);
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

    public void RegisterOnDeathEventEffect(IEventEffectOnDamage eventEffectOnDamage)
    {
        m_OnDeathEventEffect = eventEffectOnDamage;
    }
}
