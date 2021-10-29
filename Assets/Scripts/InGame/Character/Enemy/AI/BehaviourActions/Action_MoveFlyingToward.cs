using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to move flying character toward given angle, at given speed
[AddComponentMenu("Game/Action: Move Flying Toward")]
public class Action_MoveFlyingToward : BehaviourAction
{
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
    
    
    /* Owner sibling components */
    
    private MoveFlyingIntention m_MoveFlyingIntention;

    
    /* Derived parameters */

    /// Target position = start position + move vector
    /// It is important to store on Setup as the start position is not remembered
    private Vector2 m_TargetPosition;
    
    
    protected override void OnInit()
    {
        m_MoveFlyingIntention = m_EnemyCharacterMaster.GetComponentOrFail<MoveFlyingIntention>();
    }
    
    public override void RunUpdate()
    {
        m_MoveFlyingIntention.moveVelocity = speed * VectorUtil.Rotate(Vector2.left, angle);
    }

    protected override bool IsOver()
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
