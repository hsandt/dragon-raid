using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to move grounded character by given signed distance at given speed
[AddComponentMenu("Game/Action: Move Grounded By")]
public class Action_MoveGroundedBy : BehaviourAction
{
    [Header("Parameters")]

    [SerializeField, Tooltip("Signed distance to move by along X on the ground (positive to go right)")]
    private float signedDistance = 0f;

    [SerializeField, Tooltip("Motion speed (m/s)")]
    [Min(0f)]
    private float speed = 1f;

    #if UNITY_EDITOR
    public float SignedDistance { get => signedDistance; set => signedDistance = value; }
    public float Speed { get => speed; set => speed = value; }
    #endif


    /* Owner sibling components */

    private MoveGroundedIntention m_MoveGroundedIntention;


    /* State */

    /// Signed distance left to travel
    private float m_SignedDistanceLeft;


    protected override void OnInit()
    {
        m_MoveGroundedIntention = m_EnemyCharacterMaster.GetComponentOrFail<MoveGroundedIntention>();
    }

    public override void OnStart()
    {
        m_SignedDistanceLeft = signedDistance;
    }

    public override void RunUpdate()
    {
        float nextSignedSpeed;

        if (Mathf.Abs(m_SignedDistanceLeft) < speed * Time.deltaTime)
        {
            // Target at less than 1 frame of motion ahead at this speed, use lower speed to arrive
            // right on the target next frame (we must use this trick because we don't support setting
            // direct target X on MoveGroundedIntention currently, only signed speed)
            // We shouldn't fear precision issues because IsOver will return false and stop the action
            // if we arrived very close to the target last frame
            nextSignedSpeed = m_SignedDistanceLeft / Time.deltaTime;
            m_SignedDistanceLeft = 0f;
        }
        else
        {
            // Target is more than 1 frame ahead, go full speed in the wanted direction
            nextSignedSpeed = Mathf.Sign(m_SignedDistanceLeft) * speed;

            // Decrease signed distance left by the progress done this frame
            m_SignedDistanceLeft = Mathf.MoveTowards(m_SignedDistanceLeft, 0f, speed * Time.deltaTime);
        }

        m_MoveGroundedIntention.signedGroundSpeed = nextSignedSpeed;
    }

    protected override bool IsOver()
    {
        // Consider move over when character needs to move by less than a frame's move distance
        // Note that the higher the speed is, the less precise
        return m_SignedDistanceLeft == 0f;
    }

    public override void OnEnd()
    {
        m_MoveGroundedIntention.signedGroundSpeed = 0f;
    }
}
