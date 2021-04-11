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

    [Tooltip("Health Visual Parameters Data")]
    public HealthVisualParameters healthVisualParameters;


    /* Dynamic external references */
    
    /// List of health gauges observing health data
    private readonly List<GaugeHealth> m_GaugeHealthList = new List<GaugeHealth>();
    
    
    /* Sibling components */

    private CharacterMaster m_CharacterMaster;
    private Health m_Health;
    private Brighten m_Brighten;

    
    private void Awake()
    {
        m_CharacterMaster = this.GetComponentOrFail<CharacterMaster>();

        m_Health = this.GetComponentOrFail<Health>();
        m_Health.maxValue = healthParameters.maxHealth;
        
        m_Brighten = this.GetComponentOrFail<Brighten>();
        Debug.AssertFormat(healthVisualParameters, this, "No Health Visual Parameters found on {0}", this);
    }

    public override void Setup()
    {
        m_Health.value = m_Health.maxValue;
    }

    public float GetValue()
    {
        return m_Health.value;
    }

    public float GetRatio()
    {
        return (float) m_Health.value / m_Health.maxValue;
    }

    public void Damage(int value)
    {
        m_Health.value -= value;
        
        if (m_Health.value <= 0)
        {
            m_Health.value = 0;
            Die();
        }
        else
        {
            // if entity survived, play damage feedback
            m_Brighten.SetBrightnessForDuration(healthVisualParameters.damagedBrightness, healthVisualParameters.damagedBrightnessDuration);
        }

        NotifyValueChangeToObservers();
    }

    private void Die()
    {
        m_CharacterMaster.Release();
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
