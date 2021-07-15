using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// System for ShootIntention on Enemy character: handles control
/// SEO: before Shoot
public class EnemyShootController : BaseShootController
{
    [Header("Parameters data")]
    
    [Tooltip("Enemy Shoot Parameters Data")]
    public EnemyShootParameters enemyShootParameters;

        
    private void FixedUpdate()
    {
        m_ShootIntention.holdFire = true;
        if (enemyShootParameters.shootDirectionMode == EnemyShootDirectionMode.ShootAnchorForward)
        {
            // Shoot strait using shoot anchor's 2D forward
            m_ShootIntention.fireDirection = m_Shoot.shootAnchor.right;
        }
        else
        {
            // Shoot at the player character (fire direction will be normalized before shooting)
            Vector3 playerCharacterPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            m_ShootIntention.fireDirection = (Vector2) (playerCharacterPosition - m_Shoot.shootAnchor.position);
        }
    }
}
