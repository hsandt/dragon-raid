using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event Trigger: Entity Death
/// Combine with an Event Effect
/// It is pretty hard to use in a context where most entities are dynamically spawned, since it's not possible
/// to associate it to a Health System present in the scene, since no enemies is present in the editor scene
/// (except for Workshop purpose). It is also only triggered on Death, not Living Zone Exit.
/// So, it is mostly superseded by EventTrigger_EnemyWaveCleared, but was kept just in case.
[AddComponentMenu("Game/Event Trigger: Entity Death")]
public class EventTrigger_EntityDeath : MonoBehaviour
{
    [Header("Parameters")]

    [Tooltip("Health system of entity whose death will trigger the event effect. Must be located on the same entity prefab.")]
    public HealthSystem healthSystem;

    private void Awake()
    {
        if (healthSystem != null)
        {
            var eventEffectOnDamage = GetComponent<IEventEffectOnDamage>();
            if (eventEffectOnDamage != null)
            {
                healthSystem.RegisterOnDeathEventEffect(eventEffectOnDamage);
            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
            {
                Debug.LogErrorFormat(this, "{0} has no associated IEventEffectOnDamage", this);
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
