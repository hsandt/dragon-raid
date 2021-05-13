using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// FX component for objects with Particle System (auto-Release when stopped)
public class FXWithParticleSystem : FX
{
    /* Sibling components */
    
    private ParticleSystem m_ParticleSystem;
    
        
    private void Awake()
    {
        m_ParticleSystem = this.GetComponentOrFail<ParticleSystem>();
    }
    
    private void FixedUpdate()
    {
        // Unlike animated sprites, Particle System cannot define events, so 
        if (m_ParticleSystem.isStopped)
        {
            Release();
        }
    }
}
