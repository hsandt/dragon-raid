using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// FX Pool Manager
/// SEO: after LocatorManager
public class FXPoolManager : MultiPoolManager<FX, FXPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = LocatorManager.Instance.FindWithTag(Tags.FXPool)?.transform;
        }

        base.Init();
    }

    /// Spawn FX whose prefab is named `resourceName`
    public FX SpawnFX(string resourceName, Vector2 position)
    {
        FX fx = GetObject(resourceName);
        
        if (fx != null)
        {
            fx.Spawn(position);
            return fx;
        }
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat("[FXPoolManager] SpawnFX: pool starvation! Cannot spawn FX '{0}'. " +
            "Consider setting instantiateNewObjectOnStarvation: true on FXPoolManager, or increasing its pool size.",
            resourceName);
        #endif
        
        return null;
    }
    
    public void PauseAllFX()
    {
        // TODO
        
    }
    
    public void ResumeAllFX()
    {
        // TODO
    }
}
