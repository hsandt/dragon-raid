using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

public class RepeatAction : BehaviourAction
{
    /* Cached child references */
    
    /// Behaviour action on child (only single child supported, as Repeat is a decorator node
    private BehaviourAction m_RepeatedAction;

    
    /* State vars */

    /// True iff child is not running yet, or has finished execution this frame
    /// (Repeat never stops, so it will re-run the child right on next frame)
    private bool m_IsChildRunning;

    
    private void Awake()
    {
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            m_RepeatedAction = transform.GetChild(0).GetComponentOrFail<BehaviourAction>();
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.AssertFormat(childCount == 1, this,
                "[RepeatAction] There are {0} children, expected 1. Only the first child will be repeated.",
                childCount);
            #endif
        }
    }

    protected override void OnInit()
    {
        // Recurse Init on child
        m_RepeatedAction.Init(m_EnemyCharacterMaster);
    }

    public override void OnStart()
    {
        m_IsChildRunning = false;
    }

    public override void RunUpdate()
    {
        // Pattern is similar to RunActionSequence, but much simpler as run on a single action
        // and never ending
        // However we must still temporize the calls, waiting for next frame if the action is over,
        // to avoid getting stuck in an infinite loop in case it's gonna be over forever.
        
        if (!m_IsChildRunning)
        {
            m_RepeatedAction.OnStart();
            m_IsChildRunning = true;
        }
        
        if (m_RepeatedAction.IsOverOrDeactivated())
        {
            // Call OnEnd to cleanup anything set by the m_RepeatedAction we don't want anymore
            m_RepeatedAction.OnEnd();

            // m_RepeatedAction is over, so stop RepeatAction for this frame
            // (but we'll restart it next frame)
            m_IsChildRunning = false;
        }

        if (m_IsChildRunning)
        {
            m_RepeatedAction.RunUpdate();
        }
    }

    protected override bool IsOver()
    {
        // Repeat never ends
        return false;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
