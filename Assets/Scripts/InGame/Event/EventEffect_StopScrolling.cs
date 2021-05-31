using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event effect: Stop Scrolling
[AddComponentMenu("Game/Event Effect: Stop Scrolling")]
public class EventEffect_StopScrolling : MonoBehaviour, IEventEffect
{
    public void Trigger()
    {
        ScrollingManager.Instance.enabled = false;
    }
}
