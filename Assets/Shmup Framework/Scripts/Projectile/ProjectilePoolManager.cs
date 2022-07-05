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
    public Projectile SpawnProjectile(string resourceName, Vector2 position, Vector2 velocity, Faction attackerFaction)
    {
        Projectile projectile = AcquireFreeObject(resourceName);

        if (projectile != null)
        {
            projectile.WarpAndSetup(position, velocity, attackerFaction);
            return projectile;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat("[ProjectilePoolManager] SpawnProjectile: Cannot spawn projectile '{0}' due to either " +
            "missing prefab or pool starvation. In case of pool starvation, consider setting " +
            "instantiateNewObjectOnStarvation: true on ProjectilePoolManager, or increasing its pool size.",
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
