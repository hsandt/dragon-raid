using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;


public class MoveAlongBezierPathAction : BehaviourAction
{
    [Tooltip("Component containing Bezier path to follow")]
    public BezierPath2DComponent bezierPath2DComponent;

    [SerializeField, Tooltip("Should the path coordinates be interpreted relatively to the previous action's actual end position?")]
    private bool relative = false;
    public bool Relative
    {
        get { return relative; }
    }

    [SerializeField, Tooltip("Constant speed of the ship along the Bezier path")]
    private float speed = 2f;

#if UNITY_EDITOR
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }
#endif


    /* Owner sibling components */
        
    private MoveFlyingIntention m_MoveFlyingIntention;


    /* State */

    /// End position of the previous action
    private Vector2 m_PreviousActionEndPosition;

    /// Current curvilinear abscissa on the Bezier path (world units)
    private float m_CurvilinearAbscissa;

    public override void OnStart () {
        // store end position of previous action for relative move
        // (still stored if not relative to allow live debugging by toggling `relative` later)
        m_PreviousActionEndPosition = (Vector2) m_MoveFlyingIntention.transform.position;
        m_CurvilinearAbscissa = 0f;
    }

    public override void RunUpdate ()
    {
        m_CurvilinearAbscissa += speed * Time.deltaTime;
        m_CurvilinearAbscissa = Mathf.Clamp01(m_CurvilinearAbscissa);
//                Vector2 target = BezierPath2DComponent.Path.InterpolatePath(curvilinearAbscissa);
        // for now, use the non-curvilinear abscissa evaluation (each curve has the same length percentage
        // of the whole path)
        // interpret curvilinearAbscissa as a ratio (adapt speed in consequence)
        Vector2 target = bezierPath2DComponent.Path.InterpolatePathByNormalizedParameter(m_CurvilinearAbscissa);

        // in relative motion, start from the end position of the previous action
        // (ignore path offset from origin completely)
        if (relative) {
            // doesn't seem to work
            // also, maybe we should preserve offset (first point) but add a Button on Bezier Component
            // to offset whole path so first point matches origin?
            target += m_PreviousActionEndPosition;
        }

        Vector2 toTarget = target - (Vector2) m_MoveFlyingIntention.transform.position;
        float toTargetDistance = toTarget.magnitude;
        Vector2 nextVelocity;
        if (toTargetDistance < speed * Time.deltaTime)
        {
            // target is close, use exact speed to arrive next frame
            // don't like dividing by deltaTime, consider a targetWithSpeed Motor Mode that stops precisely at target
            nextVelocity = toTarget / Time.deltaTime;
        }
        else
        {
            // target is more than 1 frame ahead, go full speed
            nextVelocity = speed * toTarget / toTargetDistance;
        }
        m_MoveFlyingIntention.moveVelocity = nextVelocity;
        Debug.LogFormat("velocity: {0}", nextVelocity);
    }
    
    protected override bool IsOver()
    {
        throw new System.NotImplementedException();
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos ()
    {
        if (Application.isPlaying)
        {
            // Show current target on path
            Vector2 target = bezierPath2DComponent.Path.InterpolatePathByNormalizedParameter(m_CurvilinearAbscissa);
            GizmosUtil.DrawLocalBox2D(target, 1.0f * Vector2.one, transform, Color.green);
        }
    }
#endif
}
