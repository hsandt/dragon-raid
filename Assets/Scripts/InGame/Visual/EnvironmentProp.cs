using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// Add this component to Environment Prop sprite objects on the Midground Layer,
/// which is a parent with Kinematic Rigidbody moving all its children, meant to be unique environment props
/// for visuals only.
/// Apparently such children can still use a LivingZoneTracker and detect exiting the Living Zone individually,
/// but a check on X is still cheaper. Note that this assumes horizontal or almost horizontal scrolling.
public class EnvironmentProp : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField, Tooltip(
         "Half-width used to define visual bounds. When position X is out of screen by half-width or more, " +
         "the object is removed. If the object is asymmetrical, set the longest distance to the left or right edge.")]
    private float halfWidth = 1f;
    
    
    // The object should be a visual prop, so Update is fine 
    public void Update()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Check if object has left screen to th left side, with predefined margin
            // Remember to use camera position as we now move PC and camera within a static world
            // Add a margin of 1px just in case pixel perfect rounding made the visual still visible on screen
            // Remember that VisualLeftLimitX is already signed, so negative for left edge
            // We assume scrolling only goes left and all environment props follow scrolling,
            // so we don't have to mind about the right edge
            if (transform.position.x + halfWidth < mainCamera.transform.position.x + ScrollingManager.Instance.VisualLeftLimitX - 1f)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
