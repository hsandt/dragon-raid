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
        Sfx sfx = AcquireFreeObject();

        if (sfx != null)
        {
            sfx.PlayOneShot(clip);
            return sfx;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogWarningFormat(this, "[SfxPoolManager] PlaySfx: Cannot play clip '{0}' due to either " +
            "missing prefab or pool starvation. In case of pool starvation, consider setting " +
            "Consider setting instantiateNewObjectOnStarvation: true on SfxPoolManager, or increasing its pool size.",
            clip);
        #endif

        return null;
    }
}
