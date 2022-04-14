using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsDebug;
using CommonsHelper;

/// Decorator node: Behaviour Action that repeats the decorated action N times (or indefinitely)
public class RepeatAction : BehaviourAction
{
    [Header("Parameters")]

    [SerializeField, Tooltip("Number of times to repeat the decorated action. If 0, repeat forever.")]
    [Min(0)]
    private int repeatCount = 0;


    /* Cached child references */

    /// Behaviour action on child (only single child supported, as Repeat is a decorator node)
    private BehaviourAction m_RepeatedAction;


    /* State vars */

    /// True iff child is not running yet, or has finished execution this frame
    /// (if there are still repetitions to do, we will re-run the child right on next frame)
    private bool m_IsChildRunning;

    /// Number of times we have completed the decorated action
    private int m_CompletedRunCount;


    protected override void OnInit()
    {
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            m_RepeatedAction = transform.GetChild(0).GetComponentOrFail<BehaviourAction>();

            DebugUtil.AssertFormat(childCount == 1, this,
                "[RepeatAction] There are {0} children, expected 1. Only the first child will be repeated.",
                childCount);
        }
        else
        {
            DebugUtil.LogError("[RepeatAction] There are no children, expected 1. " +
                "Cannot register repeated action, further execution will cause null reference exceptions.", this);
        }

        // Recurse Init on child
        m_RepeatedAction.Init(m_EnemyCharacterMaster);
    }

    public override void OnStart()
    {
        m_IsChildRunning = false;
        m_CompletedRunCount = 0;
    }

    public override void RunUpdate()
    {
        // Pattern is similar to RunActionSequence, but much simpler as run on a single action
        // However we must still temporize the calls, waiting for next frame if the action is over,
        // to avoid getting stuck in an infinite loop in case the child action is gonna be over forever.

        // No need to check for m_CompletedRunCount vs repeatCount here,
        // as we assume we've checked IsOver before calling this
        if (!m_IsChildRunning)
        {
            m_RepeatedAction.OnStart();
            m_IsChildRunning = true;
        }

        if (m_RepeatedAction.IsOverOrDeactivated())
        {
            // Call OnEnd to cleanup anything set by the m_RepeatedAction we don't want anymore
            m_RepeatedAction.OnEnd();

            // m_RepeatedAction is over, so flag action as stopped for this frame
            // (if we have still runs to do, we'll restart the action next frame)
            m_IsChildRunning = false;

            // Increment the run count if it matters
            if (repeatCount > 0)
            {
                ++m_CompletedRunCount;
            }
        }

        if (m_IsChildRunning)
        {
            m_RepeatedAction.RunUpdate();
        }
    }

    protected override bool IsOver()
    {
        // If repeat count is 0, never end
        // Else, end after the wanted number of repetitions
        return repeatCount > 0 && m_CompletedRunCount >= repeatCount;
    }
}
