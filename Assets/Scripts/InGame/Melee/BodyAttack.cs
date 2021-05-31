using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;

/// System that deals damage to an opponent's Health on body contact
/// This should be placed on the move box game object
/// Collision layers: EnemyBodyHitBox collides with PlayerHurtBox, so only works for enemy hurting player character 
public class BodyAttack : MonoBehaviour
{
    /* Parameters data */
    
    [Tooltip("Body Attack Parameters Data")]
    public BodyAttackParameters bodyAttackParameters;
    
    [Tooltip("Body Attack Visual Parameters Data")]
    public BodyAttackAestheticParameters bodyAttackAestheticParameters;
    
    
    private void Awake()
    {
        Debug.AssertFormat(bodyAttackParameters, this,
            "[BodyAttack] Body Attack Parameters not set on body attack component {0}", this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // All destructible should have a rigidbody, even if they are not moving (static rigidbody).
        // This is to allow body attack to find the parent owning the HealthSystem.
        if (other.attachedRigidbody != null)
        {
            var targetHealthSystem = other.attachedRigidbody.GetComponent<HealthSystem>();
            if (targetHealthSystem != null)
            {
                TryPeriodicDamage(targetHealthSystem);
            }
        }
    }

    /// Damage target
    private void TryPeriodicDamage(HealthSystem targetHealthSystem)
    {
        bool didDamage = targetHealthSystem.TryPeriodicDamage(bodyAttackParameters.damage);

        if (didDamage)
        {
            if (bodyAttackAestheticParameters != null && bodyAttackAestheticParameters.sfxHit != null)
            {
                // Audio: play hit SFX
                SfxPoolManager.Instance.PlaySfx(bodyAttackAestheticParameters.sfxHit);
            }
        }
    }
}
