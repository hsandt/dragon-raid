using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to order a Throw
[AddComponentMenu("Game/Action: Throw")]
public class Action_Throw : BehaviourAction
{
    [Header("Parameters")]

    [SerializeField, Tooltip("Angle to throw at (degrees, 0 for forward, positive CW)")]
    [Range(-180f, 180f)]
    private float angle;

    [Tooltip("Initial projectile speed (m/s)")]
    [Min(0f)]
    public float throwSpeed = 6f;

    #if UNITY_EDITOR
    public float Angle { get => angle; set => angle = value; }
    public float ThrowSpeed { get => throwSpeed; set => throwSpeed = value; }
    #endif


    /* Owner sibling components */

    private Throw m_Throw;
    private ThrowIntention m_ThrowIntention;


    /* State */

    /// True when action has ordered throw
    private bool hasOrderedThrow;


    protected override void OnInit()
    {
        m_Throw = m_EnemyCharacterMaster.GetComponentOrFail<Throw>();
        m_ThrowIntention = m_Throw.ThrowIntention;
    }

    public override void OnStart()
    {
        hasOrderedThrow = false;
    }

    public override void RunUpdate()
    {
        m_ThrowIntention.startThrow = true;
        // angle is CW, so we rotate by -angle
        m_ThrowIntention.throwDirection = VectorUtil.Rotate(Vector2.left, -angle);
        m_ThrowIntention.throwSpeed = throwSpeed;

        hasOrderedThrow = true;
    }

    protected override bool IsOver()
    {
        return hasOrderedThrow;
    }

    #if UNITY_EDITOR
    public override string GetNodeName()
    {
        return $"Throw at {angle} degrees";
    }
    #endif
}
