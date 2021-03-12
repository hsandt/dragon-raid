using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

public class ProjectilePoolManager : MultiPoolManager<Projectile, ProjectilePoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = Locator.FindWithTag(Tags.ProjectilePool)?.transform;
        }

        base.Init();
    }

    /// Spawn projectile whose prefab is named `resourceName`
    public Projectile SpawnProjectile(string resourceName, Vector2 position, Vector2 velocity) {
        Projectile projectile = GetObject(resourceName);
        
        if (projectile != null)
        {
            projectile.Spawn(position, velocity);
            return projectile;
        }
        
        // pool starvation (error is already logged inside GetObject)
        return null;
    }
}
