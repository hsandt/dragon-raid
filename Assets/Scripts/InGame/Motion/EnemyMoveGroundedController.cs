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
                if (IsPlayerCharacterCloseEnoughToCheckJump(targetPosition) && ShouldJump(targetPosition, out float jumpSpeedImpulse))
                {
                    OrderJump(jumpSpeedImpulse);
                }
            }
        }
    }
    
    private float GetGravity()
    {
        return -Physics2D.gravity.y * m_Rigidbody2D.gravityScale;
    }
    
    /// Return the jump speed needed to reach height relative to current Y
    /// considering gravity * gravity scale
    private float CalculateJumpSpeedNeededToReachHeight(float height)
    {
        float gravity = GetGravity();
        if (gravity > 0 && height > 0)
        {
            // V_y0 = sqrt(2 * g * height)
            return Mathf.Sqrt(2 * gravity * height);
        }

        return 0f;
    }

    /// Return true is enemy is approaching target, in a range relevant to start computing whether
    /// they can jump and reach target or not. This acts as a pre-check for ShouldJump, which is more expensive.
    private bool IsPlayerCharacterCloseEnoughToCheckJump(Vector2 targetPosition)
    {
        float xToTarget = targetPosition.x - transform.position.x;
        if (xToTarget != 0f && Mathf.Sign(xToTarget) == Mathf.Sign(moveGroundedParameters.maxGroundSpeed))
        {
            // remember that convention is that maxGroundSpeed > 0 for enemies moving to the left
            // target is on the right of enemy (if enemy moves to the left), or on the left (if moves to the right),
            // so enemy can never reach target (we don't count of their body hitbox being lengthy either, so they
            // don't try hitting the target with their back)
            return false;
        }
        
        // Enemy moves at ground speed, so they must jump from the correct distance on X to reach target,
        // and that distance depends on the jump height, which depends on the target position. So we can only calculate
        // it exactly later. But for the pre-check, just take the maximum jump speed (i.e. that can reach max height)
        // possible, so you get an upper bound on the distance on X from target from which it is relevant to start
        // checking if we should jump now.
        float maxDistanceXToJump = moveGroundedParameters.maxGroundSpeed * moveGroundedParameters.maxJumpSpeed / GetGravity();
        return Mathf.Abs(xToTarget) < maxDistanceXToJump;
    }

    /// Return true if enemy thinks they can reach the target by jumping now
    /// Set jump speed impulse required to reach target along Y
    private bool ShouldJump(Vector2 targetPosition, out float jumpSpeedImpulse)
    {
        // Calculate jump speed required to reach the target
        float requiredJumpSpeed = CalculateJumpSpeedNeededToReachHeight(targetPosition.y - transform.position.y);

        // If gravity is messed up or target is below this enemy, do not jump at all
        if (requiredJumpSpeed > 0f)
        {
            // If required jump speed is greater than max speed allowed, jump at max speed
            jumpSpeedImpulse = Mathf.Min(moveGroundedParameters.maxJumpSpeed, requiredJumpSpeed);
            
            // now we can recalculate the distance on X from which we should jump as in IsPlayerCharacterCloseEnoughToCheckJump,
            // but with the exact jump speed this time (if we clamped jump speed above, it's actually the distance
            // to reach the jump's apogee, and a fictive target at that point; target may move down and still get hit
            // after all)
            float firstDistanceXToJump = moveGroundedParameters.maxGroundSpeed * requiredJumpSpeed / GetGravity();

            // we tolerate any lateness from the moment we reached the ideal distance to jump onto target
            // as long as we didn't move past the target on X (IsPlayerCharacterCloseEnoughToCheckJump already
            // does that check)
            float xToTarget = targetPosition.x - transform.position.x;
            if (Mathf.Abs(xToTarget) <= firstDistanceXToJump)
            {
                return true;
            }
        }

        jumpSpeedImpulse = 0f;
        return false;
    }

    /// Order character to jump
    private void OrderJump(float jumpSpeedImpulse)
    {
        m_MoveGroundedIntention.jumpSpeedImpulse = jumpSpeedImpulse;
        
        // don't try to jump again while jumping, nor after landing back
        m_HasTriedToJump = true;
    }
}
