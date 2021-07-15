using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event Trigger: Entity Death
/// Combine with an Event Effect
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
            var eventEffect = GetComponent<IEventEffect>();
            if (eventEffect != null)
            {
                healthSystem.RegisterOnDeathEventEffect(eventEffect);
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
