using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Script responsible for playing a Behaviour Tree from its root Behaviour Action
/// SEO: at the same position as dedicated controllers, before any system script checking some Intention,
/// like MoveGrounded, MoveFlying, Shoot, etc. (placing it before Default Time like dedicated Controller
/// and EnemyBehaviour scripts should be enough)
public class BehaviourTreePlayer : ClearableBehaviour
{
    [Header("Parameters")]
    
    [Tooltip("Root of the Default Behaviour Tree to run")]
    public BehaviourAction defaultRootAction;

    
    /* Sibling components */

    private EnemyCharacterMaster m_EnemyCharacterMaster;
        
        
    /* State vars */
    
    /// Current Behaviour Tree root action: Default Behaviour Tree root by default, but can be overridden by Enemy Spawn Data
    private BehaviourAction m_CurrentRootAction;
    
    /// True when Behaviour Tree is running
    private bool m_IsRunning;
    
    
    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(defaultRootAction != null, this, "[BehaviourTreePlayer] Awake: Default Root Action not set on {0}", this);
        #endif
        
        m_EnemyCharacterMaster = this.GetComponentOrFail<EnemyCharacterMaster>();
    }
    
    public override void Setup()
    {
        m_CurrentRootAction = null;
        m_IsRunning = false;
    }

    /// Start the default or some override action root 
    /// It must be called manually after setup from Spawn code
    public void StartBehaviourTree(BehaviourAction overrideRootAction = null)
    {
        // Override Root Action is optional, and allows to customize enemy behavior at certain spawn points
        m_CurrentRootAction = overrideRootAction != null ? overrideRootAction : defaultRootAction;

        if (m_CurrentRootAction != null)
        {
            // Inject owner to every action script, recursively through the root action
            // This is done on Setup as owner may change when we allow spawn-time action sequence swapping
            m_CurrentRootAction.Init(m_EnemyCharacterMaster);
            
            // Start behaviour tree
            m_CurrentRootAction.OnStart();
            m_IsRunning = true;
        }
        // no need to log error in else case, as it can only happen if defaultActionSequence == null,
        // which is already asserted by Awake
    }

    private void FixedUpdate()
    {
        if (!m_IsRunning)
        {
            return;
        }

        if (m_CurrentRootAction.IsOverOrDeactivated())
        {
            // Call OnEnd to cleanup anything set by the m_CurrentRootAction we don't want anymore
            m_CurrentRootAction.OnEnd();

            // m_CurrentRootAction is over, so Behaviour Tree should stop running
            m_IsRunning = false;
        }
        else
        {
            m_CurrentRootAction.RunUpdate();
        }
    }
}
