using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Event effect: Start Enemy Wave
[AddComponentMenu("Game/Event Effect: Start Enemy Wave")]
public class EventEffect_StartEnemyWave : MonoBehaviour, IEventEffect
{
    /* Sibling components */
    
    /// Enemy wave to start
    private EnemyWave m_EnemyWave;

    
    private void Awake()
    {
        // To simplify, we always place the EnemyWave component on the same game object as this Event Effect component
        m_EnemyWave = this.GetComponentOrFail<EnemyWave>();
    }

    public void Trigger()
    {
        m_EnemyWave.StartWave();
    }
}
