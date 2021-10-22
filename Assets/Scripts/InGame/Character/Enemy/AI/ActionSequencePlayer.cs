using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;
using UnityEngine.Serialization;

/// Script responsible for playing an Action Sequence on a scripted enemy
/// SEO: at the same position as dedicated controllers, before any system script checking some Intention,
/// like MoveGrounded, MoveFlying, Shoot, etc. (placing it before Default Time like dedicated Controller
/// and EnemyBehaviour scripts should be enough)
public class ActionSequencePlayer : ClearableBehaviour
{
    [Header("Parameters")]
    
    [Tooltip("Sequence of actions to execute")]
    [FormerlySerializedAs("actionSequence")]
    public ActionSequence defaultActionSequence;

    
    /* Sibling components */

    private EnemyCharacterMaster m_EnemyCharacterMaster;
        
        
    /* State vars */
    
    /// Current action sequence: default action sequence by default, but can be overridden by Enemy Spawn Data
    private ActionSequence m_CurrentActionSequence;
    
    /// True when action sequence is playing (it plays immediately on Master setup, and only stops after last action)
    private bool m_IsPlaying;
    
    /// Index of action currently active in the sequence
    private int m_CurrentActionIndex;

    
    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(defaultActionSequence != null, this, "[ActionSequencePlayer] Awake: Default Action Sequence not set on {0}", this);
        #endif
        
        m_EnemyCharacterMaster = this.GetComponentOrFail<EnemyCharacterMaster>();
    }
    
    public override void Setup()
    {
        m_CurrentActionSequence = null;
        m_IsPlaying = false;
        m_CurrentActionIndex = -1;
    }

    /// Start the default or some override action sequence
    /// It must be called manually after setup from Spawn code
    public void StartActionSequence(ActionSequence overrideActionSequence = null)
    {
        // Override Action Sequence is optional, and allows to customize enemy behavior at certain spawn points
        m_CurrentActionSequence = overrideActionSequence != null ? overrideActionSequence : defaultActionSequence;

        if (m_CurrentActionSequence != null)
        {
            // Inject owner to every action script
            // This is done on Setup as owner may change when we allow spawn-time action sequence swapping
            foreach (var action in m_CurrentActionSequence)
            {
                action.Init(m_EnemyCharacterMaster);
                action.OnInit();
            }
            
            // Start sequence
            m_IsPlaying = true;
            
            // Start at index -1 so the next action is the first one, at index 0
            m_CurrentActionIndex = -1;
        }
        // no need to log error in else case, as it can only happen if defaultActionSequence == null,
        // which is already asserted by Awake
    }

    private void FixedUpdate ()
    {
        if (!m_IsPlaying)
        {
            return;
        }
        
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
            // Note that if we are here, m_IsPlaying is true, so m_CurrentActionIndex < actionSequence.Length,
            // and in addition, the last call to TryProceedToNextAction skipped null actions,
            // so we can retrieve and use action safely.
            BehaviourAction action = m_CurrentActionSequence[m_CurrentActionIndex];
            
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
        if (m_CurrentActionIndex < m_CurrentActionSequence.Count)
        {
            BehaviourAction action = m_CurrentActionSequence[m_CurrentActionIndex];
            action.RunUpdate();
        }
    }

    private void TryProceedToNextAction()
    {
        while (true)
        {
            ++m_CurrentActionIndex;
            if (m_CurrentActionIndex < m_CurrentActionSequence.Count)
            {
                // There is still a next action
                BehaviourAction action = m_CurrentActionSequence[m_CurrentActionIndex];
                if (action != null)
                {
                    // Call OnStart immediately, as IsOverOrDeactivated may rely on it
                    action.OnStart();
                    
                    if (!action.IsOverOrDeactivated())
                    {
                        break;
                    }
                    // else, action is over: continue so we can immediately go on with the next action
                    // normally actions should not be over right after start, but it's possible if pre-conditions
                    // cannot fulfilled (e.g. target is already behind so we can never shoot it), and this can also
                    // be entered if the action was manually deactivated in the editor, so do not log error here
                }
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                else
                {
                    // action is null: continue so we can immediately go on with the next action
                    // (but you should fix your data) 
                    Debug.LogErrorFormat("[ActionSequencePlayer] On {0}, action #{1} is null", m_CurrentActionSequence, m_CurrentActionIndex);
                }
                #endif
            }
            else
            {
                // There are no more actions, stop sequence
                m_IsPlaying = false;
                break;
            }
        }
    }
}
