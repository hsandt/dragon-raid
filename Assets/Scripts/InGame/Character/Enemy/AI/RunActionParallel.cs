using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsDebug;

/// Composite node: Behaviour Action that runs multiple actions in parallel, stored as children
public class RunActionParallel : BehaviourAction
{
    /* Cached child references */

    /// List of behaviour actions on children
    private List<BehaviourAction> m_BehaviourActions;


    /* State vars */
    private readonly List<bool> m_IsActionRunningPerActionIndex = new List<bool>();


    protected override void OnInit()
    {
        m_BehaviourActions = new List<BehaviourAction>(transform.childCount);

        // Store and init every child action (even if inactive at this point)
        foreach (Transform child in transform)
        {
            var action = child.GetComponent<BehaviourAction>();
            if (action != null)
            {
                m_BehaviourActions.Add(action);
                action.Init(m_EnemyCharacterMaster);
            }
            else
            {
                DebugUtil.LogErrorFormat(child, "[RunActionSequence] Awake: '{0}' has child '{1}' with no BehaviourAction",
                    this, child);
            }
        }
    }

    public override void OnStart()
    {
        m_IsActionRunningPerActionIndex.Clear();

        foreach (var action in m_BehaviourActions)
        {
            // Start action and flag it as running
            action.OnStart();
            m_IsActionRunningPerActionIndex.Add(true);
        }
    }

    public override void RunUpdate()
    {
        for (int index = 0; index < m_BehaviourActions.Count; index++)
        {
            BehaviourAction action = m_BehaviourActions[index];

            // Parallel actions may end at different times, so if one ends but others are still running,
            // we must keep running this meta-action, only updating actions that are still running.
            if (m_IsActionRunningPerActionIndex[index])
            {
                // Check IsOverOrDeactivated before calling RunUpdate as usual, just in case action is already
                // over on start
                if (!action.IsOverOrDeactivated())
                {
                    action.RunUpdate();
                }
                else
                {
                    // In this context, the action was still flagged to be running, so it just became over
                    // (this frame if starting, or last frame if it was already running), so we should
                    // call end callback and update the flag.
                    action.OnEnd();
                    m_IsActionRunningPerActionIndex[index] = false;
                }
            }
        }
    }

    protected override bool IsOver()
    {
        // This meta-action is over when no sub-actions are still running
        return !m_IsActionRunningPerActionIndex.Contains(true);
    }
}
