using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to shoot a single bullet in a given direction
[AddComponentMenu("Game/Action: Shoot Single")]
public class Action_ShootSingle : BehaviourAction
{
    [Header("Parent references")]
    
    [Tooltip("Shoot Intention to set on Update")]
    public ShootIntention m_ShootIntention;

    
    [Header("Parameters")]

    [SerializeField, Tooltip("Angle to shoot at (degrees, 0 for forward, positive CCW)")]
    [Range(0f, 360f)]
    private float angle;

    #if UNITY_EDITOR
    public float Angle { get => angle; set => angle = value; }
    #endif
    
    
    /* State */

    /// True when character has shot the single bullet
    private bool hasOrderedShot;
    
    public override void OnStart()
    {
        hasOrderedShot = false;
    }

    public override void RunUpdate()
    {
        // We're counting on IsOver to prevent double shooting, so no need to check hasOrderedShot here 
        m_ShootIntention.fireOnce = true;
        m_ShootIntention.fireDirection = VectorUtil.Rotate(Vector2.left, angle);
        hasOrderedShot = true;
    }
    
    public override bool IsOver()
    {
        return hasOrderedShot;
    }

    public override void OnEnd()
    {
        // Optional cleanup
        // No need to clear fireOnce, as it is consumed
        m_ShootIntention.fireDirection = Vector2.zero;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
