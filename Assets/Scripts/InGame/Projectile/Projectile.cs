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
    
    
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    
    private void Awake()
    {
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
        if (other.attachedRigidbody != null)
        {
            var targetHealthSystem = other.attachedRigidbody.GetComponent<HealthSystem>();
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

        m_Rigidbody2D.position = position;
        m_Rigidbody2D.velocity = velocity;
    }

    /// Impact on target: damage it and self-destruct
    private void Impact(HealthSystem targetHealthSystem)
    {
        targetHealthSystem.Damage(projectileParameters.damage);
        Release();
    }
}
