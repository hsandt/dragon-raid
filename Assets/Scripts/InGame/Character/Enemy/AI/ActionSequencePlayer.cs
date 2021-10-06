using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Script responsible for playing an Action Sequence on a scripted enemy
/// SEO: at the same position as dedicated controllers, before any system script checking some Intention,
/// like MoveGrounded, MoveFlying, Shoot, etc. (placing it before Default Time like dedicated Controller
/// and EnemyBehaviour scripts should be enough)
public class ActionSequencePlayer : ClearableBehaviour
{
    [Header("Parameters")]
    
    [Tooltip("Sequence of actions to execute")]
    public ActionSequence actionSequence;

    
    /* State vars */
    
    /// True when action sequence is playing (it plays immediately on Master setup, and only stops after last action)
    private bool m_IsPlaying;
    
    /// Index of action currently active in the sequence
    private int m_CurrentActionIndex;

    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    private void Awake()
    {
        Debug.AssertFormat(actionSequence != null, this, "[ActionSequencePlayer] Awake: Action Sequence not set on {0}", this);
    }
    #endif
    
    public override void Setup()
    {
        // Enable script FixedUpdate (should start enabled, but important to do after one Clear on next Setup)
        // Set it before calling TryProceedToNextAction as the latter may set it back to false
        enabled = true;
        
        // Start at index -1 so the next action is the first one, at index 0
        m_CurrentActionIndex = -1;

        // Inject owner to every action script
        // This is done on Setup as owner may change when we allow spawn-time action sequence swapping
        var enemyCharacterMaster = this.GetComponentOrFail<EnemyCharacterMaster>();
        
        foreach (var action in actionSequence)
        {
            action.Init(enemyCharacterMaster);
            action.OnInit();
        }
    }

    private void FixedUpdate ()
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
            // esp. when action.Update() set rigidbody velocity and the position is only updated next frame.
            // Note that if we are here, enabled == true, so m_CurrentActionIndex < actionSequence.Length,
            // and in addition, the last call to TryProceedToNextAction skipped null actions,
            // so we can retrieve and use action safely.
            BehaviourAction action = actionSequence[m_CurrentActionIndex];
            
            if (action.IsOver())
            {
                // Call OnEnd to cleanup anything set by the action we don't want anymore
                action.OnEnd();
                
                // Last action is over, so proceed to next one, if any
                TryProceedToNextAction();
            }
        }

        // If we called TryProceedToNextAction, it may have incremented the index beyond the limit,
        // so make sure we are still inside the sequence (we could also check if (enabled))
        if (m_CurrentActionIndex < actionSequence.Count)
        {
            BehaviourAction action = actionSequence[m_CurrentActionIndex];
            action.RunUpdate();
        }
    }

    private void TryProceedToNextAction()
    {
        while (true)
        {
            ++m_CurrentActionIndex;
            if (m_CurrentActionIndex < actionSequence.Count)
            {
                // There is still a next action
                BehaviourAction action = actionSequence[m_CurrentActionIndex];
                if (action != null)
                {
                    // Call OnStart immediately, as IsOver may rely on it
                    action.OnStart();
                    
                    if (!action.IsOver())
                    {
                        Debug.LogFormat("Start action #{0}", m_CurrentActionIndex);
                        break;
                    }
                    // else, action is over: continue so we can immediately go on with the next action 
                    Debug.LogFormat("Action already over! #{0}", m_CurrentActionIndex);
                }
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                else
                {
                    // action is null: continue so we can immediately go on with the next action
                    // (but you should fix your data) 
                    Debug.LogErrorFormat("On {0}, action #{1} is null", actionSequence, m_CurrentActionIndex);
                }
                #endif
            }
            else
            {
                // There are no more actions, stop sequence
                Debug.LogFormat("No more actions for #{0}, stop sequence", m_CurrentActionIndex);
                enabled = false;
                break;
            }
        }
    }
}
