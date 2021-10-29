using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CommonsHelper;

/// Behaviour Action than runs a sequence of actions, stored as children
public class RunActionSequence : BehaviourAction
{
    /* Cached child references */
    
    /// List of behaviour actions on children
    private List<BehaviourAction> m_BehaviourActions;
    
        
    /* State vars */
    
    /// True when action sequence is running (it plays immediately on Master setup, and only stops after last action)
    private bool m_IsRunning;
    
    /// Index of action currently active in the sequence
    private int m_CurrentActionIndex;

    
    private void Awake()
    {
        // Linq statement to iterate on all children, get BehaviourAction component and generate a list
        m_BehaviourActions = transform.Cast<Transform>().Select(tr => tr.GetComponentOrFail<BehaviourAction>()).ToList();
    }
    
    protected override void OnInit()
    {
        // Recurse Init on every child
        foreach (var action in m_BehaviourActions)
        {
            action.Init(m_EnemyCharacterMaster);
        }
    }
    
    public override void OnStart()
    {
        m_IsRunning = true;
        m_CurrentActionIndex = -1;
    }

    public override void RunUpdate()
    {
        // On first update since Setup, m_CurrentActionIndex == -1 and we must proceed to the first action
        if (m_CurrentActionIndex < 0)
        {
            TryProceedToNextAction();
        }
        else
        {
            // On later updates, we must verify whether the current action is over,
            // which is done better on update start than update end, because it allows us to check up-to-date information,
            // esp. when action.RunUpdate() set rigidbody velocity and the position is only updated next frame.
            // Note that if we are here, m_IsPlaying is true, so m_CurrentActionIndex < actionSequence.Length,
            // and in addition, the last call to TryProceedToNextAction skipped null actions,
            // so we can retrieve and use action safely.
            BehaviourAction action = m_BehaviourActions[m_CurrentActionIndex];
            
            if (action.IsOverOrDeactivated())
            {
                // Call OnEnd to cleanup anything set by the action we don't want anymore
                action.OnEnd();
                
                // Last action is over, so proceed to next one, if any
                TryProceedToNextAction();
            }
        }

        // If we called TryProceedToNextAction, it may have incremented the index beyond the limit,
        // so make sure we are still inside the sequence (we could also check if (m_IsPlaying))
        if (m_IsRunning)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.AssertFormat(m_CurrentActionIndex < m_BehaviourActions.Count, this,
                "[RunActionSequence] Current action index is {0}, expected index < actions count {1} when still running",
                m_CurrentActionIndex, m_BehaviourActions.Count);
            #endif

            BehaviourAction action = m_BehaviourActions[m_CurrentActionIndex];
            action.RunUpdate();
        }
    }
    
    private void TryProceedToNextAction()
    {
        while (true)
        {
            ++m_CurrentActionIndex;
            if (m_CurrentActionIndex < m_BehaviourActions.Count)
            {
                // There is still a next action
                BehaviourAction action = m_BehaviourActions[m_CurrentActionIndex];
                if (action != null)
                {
                    // Call OnStart immediately, as IsOverOrDeactivated may rely on it
                    action.OnStart();

                    if (action.IsOverOrDeactivated())
                    {
                        // Action is over: continue loop so we can immediately go on with the next action
                        // (also clean with OnEnd just in case OnStart set some things we don't want)
                        // Normally, actions should not be over right after start, but it's possible if pre-conditions
                        // cannot fulfilled (e.g. target is already behind so we can never shoot it), and this can also
                        // be entered if the action was manually deactivated in the editor, so do not log error here
                        action.OnEnd();
                    }
                    else
                    {
                        // Action is not over right after start (normal case), we found a valid action, so break
                        break;
                    }
                }
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                else
                {
                    // action is null: continue so we can immediately go on with the next action
                    // (but you should fix your data) 
                    Debug.LogErrorFormat("[ActionSequencePlayer] On {0}, action #{1} is null", m_BehaviourActions, m_CurrentActionIndex);
                }
                #endif
            }
            else
            {
                // There are no more actions, stop sequence
                m_IsRunning = false;
                break;
            }
        }
    }

    protected override bool IsOver()
    {
        return !m_IsRunning;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
