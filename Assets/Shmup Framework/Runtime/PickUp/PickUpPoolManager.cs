using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// PickUp Pool Manager
/// SEO: after LocatorManager
public class PickUpPoolManager : MultiPoolManager<PickUp, PickUpPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = LocatorManager.Instance.FindWithTag(ConstantsManager.Tags.PickUpPool)?.transform;
        }

        base.Init();
    }

    /// Spawn PickUp whose prefab is named `resourceName`
    public PickUp SpawnPickUp(string resourceName, Vector2 position)
    {
        PickUp pickUp = AcquireFreeObject(resourceName);

        if (pickUp != null)
        {
            pickUp.Warp(position);
            return pickUp;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat("[PickUpPoolManager] SpawnPickUp: Cannot spawn PickUp '{0}' due to either " +
            "missing prefab or pool starvation. In case of pool starvation, consider setting " +
            "Consider setting instantiateNewObjectOnStarvation: true on PickUpPoolManager, or increasing its pool size.",
            resourceName);
        #endif

        return null;
    }

    public void PauseAllPickUp()
    {
        foreach (PickUp pickUp in GetObjectsInUseInAllPools())
        {
            // if pick-ups get an animation then make them MasterBehaviour and call Pause on them
            // pickUp.Pause();
        }
    }

    public void ResumeAllPickUp()
    {
        foreach (PickUp pickUp in GetObjectsInUseInAllPools())
        {
            // pickUp.Resume();
        }
    }
}
