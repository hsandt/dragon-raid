using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

public class FXPoolManager : MultiPoolManager<FX, FXPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = Locator.FindWithTag(Tags.FXPool)?.transform;
        }

        base.Init();
    }

    /// Spawn FX whose prefab is named `resourceName`
    public FX SpawnFX(string resourceName, Vector2 position) {
        FX projectile = GetObject(resourceName);
        
        if (projectile != null)
        {
            projectile.Spawn(position);
            return projectile;
        }
        
        // pool starvation (error is already logged inside GetObject)
        return null;
    }
}
