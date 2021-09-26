using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Action to move flying character by given vector at given speed
[AddComponentMenu("Game/Action: Move Flying By")]
public class Action_MoveFlyingBy : BehaviourAction
{
    [Header("Parent references")]
    
    [Tooltip("Move Flying Intention to set on Update")]
    public MoveFlyingIntention m_MoveFlyingIntention;

    
    [Header("Parameters")]

    [SerializeField, Tooltip("Vector to move by, from the last position")]
    private Vector2 moveVector = Vector2.zero;
    
    [SerializeField, Tooltip("Motion speed (m/s)")]
    [Min(0f)]
    private float speed = 1f;

    #if UNITY_EDITOR
    public Vector2 MoveVector { get => moveVector; set => moveVector = value; }
    public float Speed { get => speed; set => speed = value; }
    #endif
    
    
    /* Derived parameters */

    /// Target position = start position + move vector
    /// It is important to store on Setup as the start position is not remembered
    private Vector2 m_TargetPosition;
    
    
    public override void OnStart()
    {
        m_TargetPosition = (Vector2) m_MoveFlyingIntention.transform.position + moveVector;
    }

    public override void RunUpdate()
    {
        Vector2 nextVelocity;
        
        Vector2 toTarget = m_TargetPosition - (Vector2) m_MoveFlyingIntention.transform.position;
        float toTargetDistance = toTarget.magnitude;
        if (toTargetDistance < speed * Time.deltaTime)
        {
            // Target at less than 1 frame of motion ahead at this speed, use lower speed to arrive
            // right on the target next frame (we must use this trick because we don't support setting
            // direct target position on MoveFlyingIntention currently, only velocity)
            // We shouldn't fear precision issues because IsOver will return false and stop the action
            // if we arrived very close to the target last frame
            nextVelocity = toTarget / Time.deltaTime;
        }
        else
        {
            // Target is more than 1 frame ahead, go full speed
            // Note that toTarget / toTargetDistance = moveVector.normalized, it's just cheaper to reuse
            // square root already computed
            nextVelocity = speed * toTarget / toTargetDistance;
        }
        
        m_MoveFlyingIntention.moveVelocity = nextVelocity;
    }

    public override bool IsOver()
    {
        // Consider move over when character needs to move by less than a frame's move distance
        // Note that the higher the speed is, the less precise 
        Vector2 toTarget = m_TargetPosition - (Vector2) m_MoveFlyingIntention.transform.position;
        float moveDistancePerFrame = speed * Time.deltaTime;
        return toTarget.sqrMagnitude < moveDistancePerFrame * moveDistancePerFrame;
    }

    public override void OnEnd()
    {
        m_MoveFlyingIntention.moveVelocity = Vector2.zero;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
