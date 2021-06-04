using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// System for Rigidbody2D on projectiles: handles pooling and impact
public class Projectile : MonoBehaviour, IPooledObject
{
    /* Parameters data */
    
    [Tooltip("Projectile Parameters Data")]
    public ProjectileParameters projectileParameters;
    
    [Tooltip("Projectile Visual Parameters Data")]
    public ProjectileAestheticParameters projectileAestheticParameters;
    
    
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    
    private void Awake()
    {
        Debug.AssertFormat(projectileAestheticParameters, this,
            "[Projectile] Projectile Aesthetic Parameters not set on projectile {0}", this);

        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
    }

    
    /* IPooledObject interface */
    
    public void InitPooled()
    {
    }

    public bool IsInUse()
    {
        return gameObject.activeSelf;
    }

    public void Release()
    {
        gameObject.SetActive(false);
    }
    
    
    /* Collision methods */

    private void OnTriggerEnter2D(Collider2D other)
    {
        // All destructible should have a rigidbody, even if they are not moving (static rigidbody).
        // This is to allow projectile to find the parent owning the HealthSystem.
        Rigidbody2D targetRigidbody = other.attachedRigidbody;
        if (targetRigidbody != null)
        {
            var targetHealthSystem = targetRigidbody.GetComponent<HealthSystem>();
            if (targetHealthSystem != null)
            {
                Impact(targetHealthSystem);
            }
        }
    }
    
    
    /* Own methods */

    public void Spawn(Vector2 position, Vector2 velocity)
    {
        gameObject.SetActive(true);

        // Experimental hotfix: if you notice relative jittering between projectiles spawned frequently at the same speed,
        // due to pixel perfect camera, use this code to synchronize the sub-pixel at which they are spawned base on
        // current Time
        /*
        float offsetX = velocity.x * Time.time; 
        offsetX = offsetX % (1f/16f);
        position.x = Mathf.Round(position.x * 16f) / 16f + offsetX;
        */
        
        m_Rigidbody2D.position = position;
        m_Rigidbody2D.velocity = velocity;
        
        if (projectileAestheticParameters != null && projectileAestheticParameters.sfxSpawn != null)
        {
            // Audio: play spawn SFX
            SfxPoolManager.Instance.PlaySfx(projectileAestheticParameters.sfxSpawn);
        }
    }

    /// Impact on target: damage it and self-destruct
    private void Impact(HealthSystem targetHealthSystem)
    {
        bool didDamage = targetHealthSystem.TryOneShotDamage(projectileParameters.damage);

        if (didDamage)
        {
            // Our current convention is that projectiles that cannot hit the target goes throught it
            // This corresponds to the character being intangible as in traditional shmup post-respawn
            // safety mechanic. In some cases, it may look better to make them block the projectiles
            // (true invincibility), but in this case we'll have to distinguish the types of invincibility
            // (with some enum member) like Smash.
            Release();
    
            if (projectileAestheticParameters != null)
            {
                // Visual: impact FX appears centered on projectile's last position
                FXPoolManager.Instance.SpawnFX(projectileAestheticParameters.fxName, transform.position);
                
                if (projectileAestheticParameters.sfxImpact != null)
                {
                    // Audio: play impact SFX
                    SfxPoolManager.Instance.PlaySfx(projectileAestheticParameters.sfxImpact);
                }
            }
        }
    }
}
