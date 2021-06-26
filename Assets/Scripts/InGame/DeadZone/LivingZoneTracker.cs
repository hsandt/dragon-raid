using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// Add this component to any object that must be cleaned up after leaving the Living Zone.
/// It requires some IPooledObject component for pooled release.
public class LivingZoneTracker : ClearableBehaviour
{
    /* Sibling components */
    
    private IPooledObject m_PooledObject;
    
    
    private void Awake()
    {
        // Currently, all objects tracking living zone are released via pooling
        m_PooledObject = GetComponent<IPooledObject>();
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_PooledObject != null, this,
            "[LivingZoneTracker] No component of type IPooledObject found on {0}", gameObject);
        #endif
    }
    
    public void OnExitLivingZone()
    {
        m_PooledObject.Release();
    }
}
