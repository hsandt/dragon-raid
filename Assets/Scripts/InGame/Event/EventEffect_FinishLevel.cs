using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event effect: Finish Level
[AddComponentMenu("Game/Event Effect: Finish Level")]
public class EventEffect_FinishLevel : MonoBehaviour, IEventEffect
{
    public void Trigger()
    {
        // Start Finish Level sequence
        // This will disable the SpatialEventManager so you don't have to worry
        // about this being entered many times
        InGameManager.Instance.FinishLevel();
    }
}
