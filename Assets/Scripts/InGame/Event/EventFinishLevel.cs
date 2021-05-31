using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event effect: Finish Level
/// It has no method, as this acts like a tag (empty component data) in ECS
public class EventFinishLevel : MonoBehaviour, IEventEffect
{
    public void Trigger()
    {
        // Start Finish Level sequence
        // This will disable the SpatialEventManager so you don't have to worry
        // about this being entered many times
        InGameManager.Instance.FinishLevel();
    }
}
