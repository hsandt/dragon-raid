using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

public class Shoot : MonoBehaviour
{
    /* Parameters data */
    
    [Tooltip("Shoot Parameters Data")]
    public ShootParameters shootParameters;

    
    /* Child references */

    [Tooltip("Transform to spawn projectiles from")]
    public Transform shootAnchor;
    
    
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    private ShootIntention m_ShootIntention;
    
    
    /* Parameters */

    [SerializeField, Tooltip("Default projectile name (must match resource prefab)")]
    private string defaultProjectileName = "Fireball";
    
    
    private void Awake()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
        m_ShootIntention = this.GetComponentOrFail<ShootIntention>();
    }

    private void FixedUpdate()
    {
        if (m_ShootIntention.fire)
        {
            // In this shmup, the character always fire toward the right
            Vector2 projectileVelocity = shootParameters.projectileSpeed * Vector2.right;
            ProjectilePoolManager.Instance.SpawnProjectile(defaultProjectileName, shootAnchor.position, projectileVelocity);
        }
    }
}
