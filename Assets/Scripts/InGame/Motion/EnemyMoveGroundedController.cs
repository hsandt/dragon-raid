using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;

/// System for MoveGroundedIntention on grounded Enemy: handles control
/// SEO: before MoveGrounded
public class EnemyMoveGroundedController : BaseMoveGroundedController
{ 
    /* State */

    /// True if character has tried to jump once
    private bool m_HasTriedToJump;


    public override void Setup()
    {
        base.Setup();
        
        m_HasTriedToJump = false;
    }

    private void FixedUpdate()
    {
        // no slopes for now, so moving on ground just means linear motion to the left
        m_MoveGroundedIntention.groundSpeed = -moveGroundedParameters.maxGroundSpeed;

        // jump as soon as player character is near on X, and hasn't tried to jump yet
        if (!m_HasTriedToJump && IsPlayerCharacterNearOnX())
        {
            OrderJump();
        }
    }

    // Return true is player character is less than [playerCharacterDetectionDistanceX] away from this enemy along X
    private bool IsPlayerCharacterNearOnX()
    {
        if (InGameManager.Instance.PlayerCharacterMaster != null)
        {
            float playerCharacterX = InGameManager.Instance.PlayerCharacterMaster.transform.position.x;
            return Mathf.Abs(playerCharacterX - transform.position.x) < moveGroundedParameters.playerCharacterDetectionDistanceX;
        }

        return false;
    }

    /// Order character to jump
    private void OrderJump()
    {
        m_MoveGroundedIntention.jump = true;
        
        // don't try to jump again while jumping, nor after landing back
        m_HasTriedToJump = true;
    }
}
