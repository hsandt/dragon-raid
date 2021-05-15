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
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    
    
    /* State */

    /// True if character has tried to jump once
    private bool m_HasTriedToJump;

    
    protected override void Init()
    {
        base.Init();
        
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
    }

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
        if (!m_HasTriedToJump)
        {
            if (InGameManager.Instance.PlayerCharacterMaster != null)
            {
                Vector2 targetPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
                if (IsPlayerCharacterNearOnX(targetPosition))
                {
                    OrderJump(targetPosition);
                }
            }
        }
    }

    /// Return true is player character is less than [playerCharacterDetectionDistanceX] away from this enemy along X
    private bool IsPlayerCharacterNearOnX(Vector2 targetPosition)
    {
        float playerCharacterX = targetPosition.x;
        return Mathf.Abs(playerCharacterX - transform.position.x) < moveGroundedParameters.playerCharacterDetectionDistanceX;
    }

    /// Order character to jump
    private void OrderJump(Vector2 targetPosition)
    {
        // Calculate jump speed required to reach the target
        float requiredJumpSpeed = CalculateJumpSpeedNeededToReachHeight(targetPosition.y - transform.position.y);

        // If gravity is messed up or target is below this enemy, do not jump at all
        if (requiredJumpSpeed > 0f)
        {
            // If required jump speed is greater than max speed allowed, jump at max speed
            float jumpSpeedImpulse = Mathf.Min(moveGroundedParameters.maxJumpSpeed, requiredJumpSpeed);
            m_MoveGroundedIntention.jumpSpeedImpulse = jumpSpeedImpulse;
            
            // don't try to jump again while jumping, nor after landing back
            m_HasTriedToJump = true;
        }
    }

    /// Return the jump speed needed to reach height relative to current Y
    /// considering gravity * gravity scale
    private float CalculateJumpSpeedNeededToReachHeight(float height)
    {
        float gravity = -Physics2D.gravity.y * m_Rigidbody2D.gravityScale;
        if (gravity > 0 && height > 0)
        {
            // V_y0 = sqrt(2 * g * height)
            return Mathf.Sqrt(2 * gravity * height);
        }

        return 0f;
    }
}
