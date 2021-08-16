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
    
    
    /* State */

    /// Flag meant to track whether entity has been Setup but not Cleared
    /// so we don't process exiting Living Zone during non-ingame-related exit
    /// such as Restart Releasing => deactivating the pooled object
    private bool m_IsAlive;

    /// Getter for m_IsAlive
    public bool IsAlive => m_IsAlive;
    
    
    private void Awake()
    {
        // Currently, all objects tracking living zone are released via pooling
        m_PooledObject = GetComponent<IPooledObject>();
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_PooledObject != null, this,
            "[LivingZoneTracker] No component of type IPooledObject found on {0}", gameObject);
        #endif
        
        // Enemy Character Master is a Pooled Object, and there should only be one Pooled Object component per object
        // so if this object is an enemy, we already have the Enemy Character Master reference as pooled object,
        // and we just need to cast it. We do a dynamic cast, so if it's not an enemy, we just get null.
        m_EnemyCharacterMaster = m_PooledObject as EnemyCharacterMaster;
    }

    public override void Setup()
    {
        m_IsAlive = true;
    }

    public override void Clear()
    {
        m_IsAlive = false;
    }

    public void OnExitLivingZone()
    {
        if (m_EnemyCharacterMaster != null)
        {
            m_EnemyCharacterMaster.OnDeathOrExit();
        }

        // Always Release after other signals as those may need members cleared in Release
        m_PooledObject.Release();
    }
}
