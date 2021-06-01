using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event Trigger: Enemy Death
/// Combine with an Event Effect
[AddComponentMenu("Game/Event Trigger: Enemy Death")]
public class EventTrigger_EnemyDeath : MonoBehaviour
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Enemy wave for which every enemy will trigger the death event effect")]
    public EnemyWave enemyWave;

    private void Awake()
    {
        if (enemyWave != null)
        {
            var eventEffect = GetComponent<IEventEffect>();
            if (eventEffect != null)
            {
                enemyWave.RegisterOnDeathEventEffect(eventEffect);
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
            Debug.LogErrorFormat(this, "{0} has no Observed Health System set", this);
        }
        #endif
    }
}
