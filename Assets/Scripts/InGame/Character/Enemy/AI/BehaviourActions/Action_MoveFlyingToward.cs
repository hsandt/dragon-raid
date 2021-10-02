using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to move flying character toward given angle, at given speed
[AddComponentMenu("Game/Action: Move Flying Toward")]
public class Action_MoveFlyingToward : BehaviourAction
{
    [Header("Parent references")]
    
    [Tooltip("Move Flying Intention to set on Update")]
    public MoveFlyingIntention moveFlyingIntention;

    
    [Header("Parameters")]

    [SerializeField, Tooltip("Angle to shoot at, from the Left (degrees, 0 for Left, positive CCW)")]
    [Range(-180f, 180f)]
    private float angle;

    [SerializeField, Tooltip("Motion speed (m/s)")]
    [Min(0f)]
    private float speed = 1f;

    #if UNITY_EDITOR
    public float Angle { get => angle; set => angle = value; }
    public float Speed { get => speed; set => speed = value; }
    #endif
    
    
    /* Derived parameters */

    /// Target position = start position + move vector
    /// It is important to store on Setup as the start position is not remembered
    private Vector2 m_TargetPosition;
    
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    void Awake()
    {
        Debug.AssertFormat(moveFlyingIntention != null, this, "[Action_MoveFlyingBy] No Move Flying Intention component reference set on {0}.", this);
    }
    #endif
    
    
    public override void RunUpdate()
    {
        moveFlyingIntention.moveVelocity = speed * VectorUtil.Rotate(Vector2.left, angle);
    }

    public override bool IsOver()
    {
        // This action never ends, so this must be the last action.
        // Assuming speed != 0, the character should exit the Living Zone and disappear at some point.
        // For this reason, we don't override OnEnd to reset the velocity intention.
        return false;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
