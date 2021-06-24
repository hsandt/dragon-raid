using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

public class Throw : ClearableBehaviour
{
    // Animator hashes
    
    private static readonly int throwHash = Animator.StringToHash("Throw");
    
    
    [Header("Parameters data")]
    
    [Tooltip("Throw Parameters Data")]
    public ThrowParameters throwParameters;

    [Tooltip("Throw Aesthetic parameters Data")]
    public ThrowAestheticParameters throwAestheticParameters;
    
    
    [Header("Child references")]

    [Tooltip("Transform to spawn projectiles from")]
    public Transform throwAnchor;
    
    
    [Header("Parameters")]

    [SerializeField, Tooltip("Projectile name (must match resource prefab)")]
    private string projectileName = "Rock";
    
    
    /* Sibling components */
    
    private Animator m_Animator;
    private ThrowIntention m_ThrowIntention;
    
    
    /* State */
    
    /// True iff character is playing the animation to throw a projectile
    /// (may be before or after the actual projectile spawn)
    private bool m_IsThrowing;
    
    /// True iff character is playing the animation to throw a projectile (getter)
    public bool IsThrowing => m_IsThrowing;
    
    
    private void Awake()
    {
        m_Animator = this.GetComponentOrFail<Animator>();
        m_ThrowIntention = this.GetComponentOrFail<ThrowIntention>();
    }

    public override void Setup()
    {
        m_IsThrowing = false;
    }

    private void FixedUpdate()
    {
        // Check and consume start throw intention (no need to clear throw direction, it won't be used until next throw)
        if (ControlUtil.ConsumeBool(ref m_ThrowIntention.startThrow))
        {
            // Only allow throwing if not already throwing
            if (!m_IsThrowing)
            {
                StartThrow();
            }
        }
    }


    private void StartThrow()
    {
        m_IsThrowing = true;

        // Animation: play Throw animation
        m_Animator.SetTrigger(throwHash);
    }
    
    /// Animation Event callback: spawn projectile
    public void ThrowEvent_SpawnProjectile()
    {
        Vector2 projectileVelocity = throwParameters.projectileSpeed * m_ThrowIntention.throwDirection.normalized;
        ProjectilePoolManager.Instance.SpawnProjectile(projectileName, throwAnchor.position, projectileVelocity);
        
        if (throwAestheticParameters != null && throwAestheticParameters.sfxSpawnProjectile != null)
        {
            // Audio: play spawn projectile SFX
            // Note that projectiles also have their own Spawn SFX, so consider using that instead
            SfxPoolManager.Instance.PlaySfx(throwAestheticParameters.sfxSpawnProjectile);
        }
    }
    
    /// Animation Event callback: notify script that character has finished animation
    public void ThrowEvent_NotifyAnimationEnd()
    {
        m_IsThrowing = false;
    }
}
