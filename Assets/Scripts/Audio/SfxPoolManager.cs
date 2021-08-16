using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

public class SfxPoolManager : PoolManager<Sfx, SfxPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            // just spawn SFX as children of the manager object
            poolTransform = transform;
        }

        base.Init();
    }

    /// Spawn SFX whose prefab is named `resourceName`
    public Sfx PlaySfx(AudioClip clip)
    {
        Sfx sfx = GetObject();
        
        if (sfx != null)
        {
            sfx.PlayOneShot(clip);
            return sfx;
        }
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarningFormat(this, "[SfxPoolManager] PlaySfx: pool starvation! Cannot play clip '{0}'. " +
            "Consider setting instantiateNewObjectOnStarvation: true on SfxPoolManager, or increasing its pool size.",
            clip);
        #endif
        
        return null;
    }
}
