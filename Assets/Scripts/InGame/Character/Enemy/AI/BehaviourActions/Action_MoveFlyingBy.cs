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

    /// Move distance
    private float m_MoveDistance;
    
    /// Normalized move direction
    private Vector2 m_MoveDirection;
    
    
    /* State */
    
    /// Signed distance left to travel
    private float m_DistanceLeft;
    
    
    protected override void OnInit()
    {
        m_MoveFlyingIntention = m_EnemyCharacterMaster.GetComponentOrFail<MoveFlyingIntention>();
        
        // Precompute derived parameters
        m_MoveDistance = moveVector.magnitude;
        m_MoveDirection = moveVector / m_MoveDistance;
    }
    
    public override void OnStart()
    {
        m_DistanceLeft = m_MoveDistance;
    }

    public override void RunUpdate()
    {
        Vector2 nextVelocity;

        if (Mathf.Abs(m_DistanceLeft) < speed * Time.deltaTime)
        {
            // Target at less than 1 frame of motion ahead at this speed, use lower speed to arrive
            // right on the target next frame (we must use this trick because we don't support setting
            // direct target position on MoveFlyingIntention currently, only velocity)
            // We shouldn't fear precision issues because IsOver will return false and stop the action
            // if we arrived very close to the target last frame
            nextVelocity = m_MoveDirection * m_DistanceLeft / Time.deltaTime;
            m_DistanceLeft = 0f;
        }
        else
        {
            // Target is more than 1 frame ahead, go full speed
            nextVelocity = m_MoveDirection * speed;
            
            // Decrease signed distance left by the progress done this frame
            m_DistanceLeft = Mathf.MoveTowards(m_DistanceLeft, 0f, speed * Time.deltaTime);
        }
        
        m_MoveFlyingIntention.moveVelocity = nextVelocity;
    }

    protected override bool IsOver()
    {
        // Consider move over when character needs to move by less than a frame's move distance
        // Note that the higher the speed is, the less precise 
        return m_DistanceLeft == 0f;
    }

    public override void OnEnd()
    {
        m_MoveFlyingIntention.moveVelocity = Vector2.zero;
    }
}
