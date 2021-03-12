using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

public class Shoot : MonoBehaviour
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
    
    private Rigidbody2D m_Rigidbody2D;
    private ShootIntention m_ShootIntention;
    
    
    /* State */
    
    /// Time before next Fire is allowed
    private float m_FireCooldownTime;
    
    
    private void Awake()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
    }
    
    private void Start()
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
            
            // Shot Anchor must be rotated so the right (X) axis points toward the character
            Vector2 projectileVelocity = shootParameters.projectileSpeed * shootAnchor.right;
            ProjectilePoolManager.Instance.SpawnProjectile(defaultProjectileName, shootAnchor.position, projectileVelocity);
        }
    }
}
