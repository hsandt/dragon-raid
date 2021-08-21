using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Projectile Pool Manager
/// SEO: after LocatorManager
public class ProjectilePoolManager : MultiPoolManager<Projectile, ProjectilePoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = LocatorManager.Instance.FindWithTag(Tags.ProjectilePool)?.transform;
        }

        base.Init();
    }

    /// Spawn projectile whose prefab is named `resourceName`
    public Projectile SpawnProjectile(string resourceName, Vector2 position, Vector2 velocity)
    {
        Projectile projectile = GetObject(resourceName);
        
        if (projectile != null)
        {
            projectile.Spawn(position, velocity);
            return projectile;
        }
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat("[ProjectilePoolManager] SpawnProjectile: pool starvation! Cannot spawn projectile '{0}'. " +
            "Consider setting instantiateNewObjectOnStarvation: true on ProjectilePoolManager, or increasing its pool size.",
            resourceName);
        #endif
        
        return null;
    }
    
    public void PauseAllProjectiles()
    {
        foreach (Projectile projectile in GetObjectsInUseInAllPools())
        {
            projectile.Pause();
        }
    }
    
    public void ResumeAllProjectiles()
    {
        foreach (Projectile projectile in GetObjectsInUseInAllPools())
        {
            projectile.Resume();
        }
    }
}
