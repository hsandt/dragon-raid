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
        
        // pool starvation (error is already logged inside GetObject)
        return null;
    }
}
