using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// System for Rigidbody2D + trigger Collider2D on Damaging Environment objects
/// Handles dealing damage to the player character
public class DamagingEnvironment : MonoBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Projectile Parameters Data")]
    public DamagingEnvironmentParameters damagingEnvironmentParameters;

    [Tooltip("Damaging Environment Aesthetic Parameters Data")]
    public DamagingEnvironmentAestheticParameters damagingEnvironmentAestheticParameters;


    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        var go = gameObject;
        int layer = go.layer;
        Debug.AssertFormat(layer == ConstantsManager.Layers.DamagingEnvironment, this,
            "[DamagingEnvironment] Game object {0} is expected to be on layer {1}, " +
            "but it is on layer {2}.",
            go,
            LayerMask.LayerToName(ConstantsManager.Layers.DamagingEnvironment),
            LayerMask.LayerToName(layer));

        Debug.AssertFormat(damagingEnvironmentParameters, this,
            "[DamagingEnvironment] Damaging Environment Parameters not set on damaging environment {0}", this);
        Debug.AssertFormat(damagingEnvironmentAestheticParameters, this,
            "[DamagingEnvironment] Damaging Environment Aesthetic Parameters not set on damaging environment {0}", this);
        #endif
    }


    /* Collision methods */

    private void OnTriggerStay2D(Collider2D other)
    {
        // All destructible should have a rigidbody, even if they are not moving (static rigidbody).
        // This is to allow projectile to find the parent owning the HealthSystem.
        // Note: we don't check for Player Character here.
        // The physics 2D collision layer matrix verifies that.
        Rigidbody2D targetRigidbody = other.attachedRigidbody;
        if (targetRigidbody != null)
        {
            var targetHealthSystem = targetRigidbody.GetComponent<HealthSystem>();
            if (targetHealthSystem != null)
            {
                TryPeriodicDamage(targetHealthSystem);
            }
        }
    }

    /// Impact on target: damage it and self-destruct
    private void TryPeriodicDamage(HealthSystem targetHealthSystem)
    {
        bool didDamage = targetHealthSystem.TryTakePeriodicDamage(damagingEnvironmentParameters.damage, ElementType.Neutral);

        if (didDamage)
        {
            if (damagingEnvironmentAestheticParameters != null)
            {
                if (damagingEnvironmentAestheticParameters.sfxDamage != null)
                {
                    // Audio: play impact SFX
                    SfxPoolManager.Instance.PlaySfx(damagingEnvironmentAestheticParameters.sfxDamage);
                }
            }
        }
    }
}
