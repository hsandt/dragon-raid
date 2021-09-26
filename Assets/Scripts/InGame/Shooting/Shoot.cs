using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

public class Shoot : ClearableBehaviour
{
    [Header("Parameters data")]
    
    [Tooltip("Shoot Parameters Data")]
    public ShootParameters shootParameters;

    
    [Header("Child references")]

    [Tooltip("Transform to spawn projectiles from")]
    public Transform shootAnchor;
    
    
    [Header("Parameters")]

    [SerializeField, Tooltip("Default projectile name (must match resource prefab)")]
    private string defaultProjectileName = "Fireball";
    
    
    /* Sibling components */
    
    private ShootIntention m_ShootIntention;
    public ShootIntention ShootIntention => m_ShootIntention;
    
    
    /* State */
    
    /// Time before next Fire is allowed
    private float m_FireCooldownTime;
    
    
    private void Awake()
    {
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
    }

    public override void Setup()
    {
        m_FireCooldownTime = 0f;
    }

    private void FixedUpdate()
    {
        // cool down
        if (m_FireCooldownTime > 0f)
        {
            m_FireCooldownTime = Mathf.Max(0f, m_FireCooldownTime - Time.deltaTime);
        }
        
        // do not put this in else case of countdown above, in case we've just ended cooldown this frame
        if (m_ShootIntention.holdFire && m_FireCooldownTime <= 0f)
        {
            // actually fire
        
            // set cooldown time to prevent firing immediately again
            m_FireCooldownTime = shootParameters.fireCooldownDuration;
            
            // spawn projectile with normalized direction and projectile speed
            Vector2 projectileVelocity = shootParameters.projectileSpeed * m_ShootIntention.fireDirection.normalized;
            ProjectilePoolManager.Instance.SpawnProjectile(defaultProjectileName, shootAnchor.position, projectileVelocity);
        }
        
        // check for single shot too
        if (ControlUtil.ConsumeBool(ref m_ShootIntention.fireOnce))
        {
            // only allow single shots if not already holding fire
            // however, do not check this above ConsumeBool, as we want to consume the flag every frame,
            // used or not
            if (!m_ShootIntention.holdFire)
            {
                // single shots do not use the fire cooldown time to allow freestyle patterns
                // otherwise, same principle as the continuous shot
                Vector2 projectileVelocity = shootParameters.projectileSpeed * m_ShootIntention.fireDirection.normalized;
                ProjectilePoolManager.Instance.SpawnProjectile(defaultProjectileName, shootAnchor.position, projectileVelocity);
            }
        }
    }
}
