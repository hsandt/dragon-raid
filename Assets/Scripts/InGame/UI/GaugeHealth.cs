using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaugeHealth : Gauge
{
    /// Tracked health system
    private HealthSystem m_TrackedHealthSystem;

    public void RegisterHealthSystem(HealthSystem trackedHealthSystem)
    {
        // Note: on Restart, the 2 next lines do nothing
        m_TrackedHealthSystem = trackedHealthSystem;
        m_TrackedHealthSystem.RegisterObserver(this);
        RefreshGauge();
    }

    private void OnDestroy()
    {
        if (m_TrackedHealthSystem != null)
        {
            m_TrackedHealthSystem.UnregisterObserver(this);
        }
    }

    protected override float GetRatio()
    {
        return m_TrackedHealthSystem.GetRatio();
    }
    
    protected override string GetValueAsString()
    {
        return m_TrackedHealthSystem.GetValue().ToString("0");
    }
}
