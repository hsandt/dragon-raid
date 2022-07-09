using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// FX Pool Manager
/// SEO: after LocatorManager
public class FXPoolManager : MultiPoolManager<FX, FXPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = LocatorManager.Instance.FindWithTag(ConstantsManager.Tags.FXPool)?.transform;
        }

        base.Init();
    }

    /// Spawn FX whose prefab is named `resourceName`
    public FX SpawnFX(string resourceName, Vector2 position)
    {
        FX fx = AcquireFreeObject(resourceName);

        if (fx != null)
        {
            fx.Warp(position);
            return fx;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat("[FXPoolManager] SpawnFX: Cannot spawn FX '{0}' due to either " +
            "missing prefab or pool starvation. In case of pool starvation, consider setting " +
            "Consider setting instantiateNewObjectOnStarvation: true on FXPoolManager, or increasing its pool size.",
            resourceName);
        #endif

        return null;
    }

    public void PauseAllFX()
    {
        foreach (FX fx in GetObjectsInUseInAllPools())
        {
            fx.Pause();
        }
    }

    public void ResumeAllFX()
    {
        foreach (FX fx in GetObjectsInUseInAllPools())
        {
            fx.Resume();
        }
    }
}
