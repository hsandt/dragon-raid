using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// Add this component to any object that must be cleaned up after leaving the Living Zone.
/// It requires some IPooledObject component for pooled release.
public class LivingZoneTracker : ClearableBehaviour
{
    /* Sibling components (required) */

    private IPooledObject m_PooledObject;
    
    
    /* Sibling components (optional) */

    private EnemyCharacterMaster m_EnemyCharacterMaster;
    private IPausable m_Pausable;
    
    
    /* State */

    public bool IsAlive => m_PooledObject.IsInUse();
    
    
    private void Awake()
    {
        // Currently, all objects tracking living zone are released via pooling
        m_PooledObject = GetComponent<IPooledObject>();
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_PooledObject != null, this,
            "[LivingZoneTracker] No component implementing IPooledObject found on {0}", gameObject);
        #endif

        if (m_PooledObject == null)
        {
            return;
        }
        
        // Enemy Character Master is a Pooled Object, and there should only be one Pooled Object component per object
        // so if this object is an enemy, we already have the Enemy Character Master reference as pooled object,
        // and we just need to cast it. We do a dynamic cast, so if it's not an enemy, we just get null.
        m_EnemyCharacterMaster = m_PooledObject as EnemyCharacterMaster;
        
        // Make sure to always make pooled objects IPausable so they can act like master scripts
        // (often MasterBehaviour, but not necessarily), be paused by InGameManager and recognized here)
        m_Pausable = m_PooledObject as IPausable;
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_Pausable != null, this,
            "[LivingZoneTracker] Component implementing IPooledObject {0} is not implementing IPausable on {1}",
            m_PooledObject, gameObject);
        #endif
    }

    public void OnExitLivingZone()
    {
        // Only allow Release on living zone exit when:
        // 1. Entity is active, i.e. Setup but not Cleared yet, i.e. "alive"
        // (this avoids trying to process an entity that was already killed by attack earlier on the same frame,
        // or Released by Restart, causing it to disable its trigger and exit the Living Zone unrelated to gameplay)
        // 2. Entity is "running" i.e. not paused
        // (pausing stops rigidbody simulation which also makes objects leave the living zone,
        // and we must completely ignore this as it's not a gameplay-related exit)
        if (m_PooledObject.IsInUse() && !m_Pausable.IsPaused())
        {
            if (m_EnemyCharacterMaster != null)
            {
                m_EnemyCharacterMaster.OnDeathOrExit();
            }

            // Always Release after other signals as those may need members cleared in Release
            m_PooledObject.Release();
        }
    }
}
