using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event Trigger: Spatial Progress
/// Combine with an Event Effect
[AddComponentMenu("Game/Event Trigger: Spatial Progress")]
public class EventTrigger_SpatialProgress : MonoBehaviour
{
    // EDITOR note: we should add an Editor that shows conversion between time and spatial progress
    // (using scrolling speed as a factor) so designer can easily edit thinking in seconds or meters
    
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Spatial progress that must be reached by Scrolling Manager so the effect associated " +
                             "to this event can be triggered")]
    private float requiredSpatialProgress = 0f;
    
    /// Spatial progress that must be reached by Scrolling Manager so the effect associated
    /// to this event can be triggered
    public float RequiredSpatialProgress => requiredSpatialProgress;
}
