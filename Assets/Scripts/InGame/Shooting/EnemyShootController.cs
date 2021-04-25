using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// System for ShootIntention on Enemy character: handles control
public class EnemyShootController : BaseShootController
{
    [Header("Parameters data")]
    
    [Tooltip("Enemy Shoot Parameters Data")]
    public EnemyShootParameters enemyShootParameters;

        
    private void FixedUpdate()
    {
        // for now, simple logic: enemy shoots as much as they can
        m_ShootIntention.holdFire = true;
        if (enemyShootParameters.shootDirectionMode == EnemyShootDirectionMode.FollowShootAnchor)
        {
            m_ShootIntention.fireDirection = m_Shoot.shootAnchor.right;
        }
        else
        {
            Vector3 playerCharacterPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            m_ShootIntention.fireDirection = (Vector2) (playerCharacterPosition - m_Shoot.shootAnchor.position);
        }
    }
}
