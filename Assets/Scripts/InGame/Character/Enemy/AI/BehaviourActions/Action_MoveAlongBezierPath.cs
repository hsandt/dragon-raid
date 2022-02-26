using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to move flying character along a relative Bezier path
/// at constant curvilinear (path parameter) speed
[AddComponentMenu("Game/Action: Move Along Bezier Path")]
public class Action_MoveAlongBezierPath : BehaviourAction
{
    [Header("Parameters")]

    [Tooltip("Component containing Bezier path to follow")]
    public BezierPath2DComponent bezierPath2DComponent;

    [SerializeField, Tooltip("Duration of full motion along the Bezier path")]
    [Min(0.1f)]
    private float duration = 1f;

#if UNITY_EDITOR
    public float Duration { get => duration; set => duration = value; }
#endif


    /* Owner sibling components */

    private MoveFlyingIntention m_MoveFlyingIntention;


    /* Derived parameters */

    /// Number of curves in the Bezier path
    /// We could only work with Normalized Parameter so we don't have to use this, but it's a little easier
    /// to read a non-normalized parameter / curvilinear abscissa in debug (1 means we finished the first curve)
    /// than a normalized one (0.333... means we finished the first curve, if there are 3 curves...).
    private float m_CurvesCount;

    /// Curvilinear speed: derivative of path parameter over time
    /// It is constant and indicates the speed at which parameter increases from 0 to #curves,
    /// but is generally not proportional to world speed, which means the world speed may not be constant.
    /// In addition, the Bezier path is made of multiple chained curves, and we picked the convention that
    /// the moving point spends the same amount of time on each curve.
    /// This means that longer curves will be traveled faster in average, and that the world velocity may not
    /// be continuous at key points linking 2 curves.
    /// However, is the Bezier tangents are symmetrical at a given key point, then the world velocity is continuous
    /// at this key point.
    private float m_CurvilinearSpeed;


    /* State */

    /// Start position: either spawn position or end position of the previous action
    private Vector2 m_StartPosition;

    /// Current curvilinear abscissa () on the Bezier path (world units)
    private float m_CurvilinearAbscissa;


    protected override void OnInit()
    {
        m_MoveFlyingIntention = m_EnemyCharacterMaster.GetComponentOrFail<MoveFlyingIntention>();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(bezierPath2DComponent, this,
            "[Action_MoveAlongBezierPath] Bezier Path 2D Component not set on {0}", this);
        #endif

        // We are working with non-normalized path parameter, which evolves from 0 to #curves
        // Since we cached derived parameters on Init, do not add/remove key points while Behaviour Tree is running
        // (moving key points is okay, but may lead to high speed motions for catch-up)
        m_CurvesCount = bezierPath2DComponent.Path.GetCurvesCount();
        m_CurvilinearSpeed = m_CurvesCount / duration;
    }

    public override void OnStart ()
    {
        m_StartPosition = (Vector2) m_MoveFlyingIntention.transform.position;
        m_CurvilinearAbscissa = 0f;
    }

    public override void RunUpdate ()
    {
        // Currently, we only support constant curvilinear speed, not constant world speed, so increase
        // m_CurvilinearAbscissa at constant speed. See m_CurvilinearSpeed for more information.
        m_CurvilinearAbscissa += m_CurvilinearSpeed * Time.deltaTime;
        m_CurvilinearAbscissa = Mathf.Clamp(m_CurvilinearAbscissa, 0f, m_CurvesCount);

        // Determine target position for this frame. Remember that Bezier path is relative, so add start position
        Vector2 target = m_StartPosition + bezierPath2DComponent.Path.InterpolatePathByParameter(m_CurvilinearAbscissa);

        // Calculate vector from current position to target and set velocity so we arrive just on target next frame.
        // We assume we have a proper path that starts at (relative) (0, 0), so the entity position is continuous,
        // and the entity stays on track (cannot be pushed away by another entity), so we will never set a crazy speed
        // just to catch up a target position far away.
        // If this proves too unstable, prefer adding an alternative motion mode which takes a target position directly
        // and Rigidbody2D.MovePosition the entity to this target.
        Vector2 toTarget = target - (Vector2) m_MoveFlyingIntention.transform.position;
        Vector2 nextVelocity = toTarget / Time.deltaTime;

        m_MoveFlyingIntention.moveVelocity = nextVelocity;
    }

    protected override bool IsOver()
    {
        return m_CurvilinearAbscissa >= (float) m_CurvesCount;
    }

    public override void OnEnd()
    {
        m_MoveFlyingIntention.moveVelocity = Vector2.zero;
    }
}
