using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event Trigger: Enemy Wave Cleared
/// Combine with an Event Effect
[AddComponentMenu("Game/Event Trigger: Enemy Wave ")]
public class EventTrigger_EnemyWaveCleared : MonoBehaviour
{
    [Header("Parameters")]
    
    [Tooltip("Enemy wave for which event effect will trigger once all enemies have died or exited")]
    public EnemyWave enemyWave;

    
    private void Awake()
    {
        if (enemyWave != null)
        {
            var eventEffect = GetComponent<IEventEffect>();
            if (eventEffect != null)
            {
                enemyWave.RegisterOnWaveClearedEventEffect(eventEffect);
            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
            {
                Debug.LogErrorFormat(this, "{0} has no associated IEventEffect", this);
            }
            #endif
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "{0} has no Enemy Wave assigned", this);
        }
        #endif
    }
}
