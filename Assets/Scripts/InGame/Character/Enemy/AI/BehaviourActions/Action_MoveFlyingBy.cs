using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to move flying character by given vector at given speed
[AddComponentMenu("Game/Action: Move Flying By")]
public class Action_MoveFlyingBy : BehaviourAction
{
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
    
    
    /* Owner sibling components */
    
    private MoveFlyingIntention m_MoveFlyingIntention;


    /* Derived parameters */

    // Consider changing this to a distance left tracker like Action_MoveGroundedBy
    // as it's more fitting for relative move esp. if character can be blocked by some things
    
    /// Target position = start position + move vector
    /// It is important to store on Setup as the start position is not remembered
    private Vector2 m_TargetPosition;
    
    
    protected override void OnInit()
    {
        m_MoveFlyingIntention = m_EnemyCharacterMaster.GetComponentOrFail<MoveFlyingIntention>();
    }
    
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

    protected override bool IsOver()
    {
        // Consider move over when character needs to move by less than a frame's move distance
        // Note that the higher the speed is, the less precise the arrival is
        // Consider checking (clamped) distance left == 0f like Action_MoveGroundedBy
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
