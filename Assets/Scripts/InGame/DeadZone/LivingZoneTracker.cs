using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

public class LivingZoneTracker : ClearableBehaviour
{
    /* Sibling components */
    
    private IPooledObject m_PooledObject;
    
    
    private void Awake()
    {
        // Currently, all objects tracking living zone are released via pooling
        m_PooledObject = GetComponent<IPooledObject>();
        Debug.AssertFormat(m_PooledObject != null, this,
            "[LivingZoneTracker] No component of type IPooledObject found on {0}", gameObject);
    }
    
    public void OnExitLivingZone()
    {
        m_PooledObject.Release();
    }
}
