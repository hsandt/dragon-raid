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

#if UNITY_EDITOR
    /* Debug */

    /// Last AI behaviour chosen by this controller
    private string m_DebugLastAIBehaviourResult = "None";
    public string DebugLastAIBehaviourResult => m_DebugLastAIBehaviourResult;
#endif

    
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
        #if UNITY_EDITOR
        m_DebugLastAIBehaviourResult = "None";
        #endif
        
        // No slopes for now, so moving on ground just means linear motion to the left
        // Note that we only control the extra ground speed. Scrolling naturally moves all
        // grounded units to the opposite direction, even when they are airborne.
        m_MoveGroundedIntention.signedGroundSpeed = - moveGroundedParameters.maxGroundSpeed;

        // Only check for jump if max jump speed is positive
        if (moveGroundedParameters.maxJumpSpeed > 0f)
        {
            CheckShouldJump();
        }
    }

    private void CheckShouldJump()
    {
        // jump as soon as player character is near on X, and hasn't tried to jump yet
        if (!m_HasTriedToJump)
        {
            if (InGameManager.Instance.PlayerCharacterMaster != null)
            {
                Vector2 targetPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
                if (IsPlayerCharacterCloseEnoughToCheckJump(targetPosition))
                {
                    if (ShouldJump(targetPosition, out float jumpSpeedImpulse))
                    {
                        OrderJump(jumpSpeedImpulse);

                        #if UNITY_EDITOR
                        m_DebugLastAIBehaviourResult = "Jump";
                        #endif
                    }
                    #if UNITY_EDITOR
                    else
                    {
                        m_DebugLastAIBehaviourResult = "Not close enough for small jump";
                    }
                    #endif
                }
                #if UNITY_EDITOR
                else
                {
                    m_DebugLastAIBehaviourResult = "Not close enough";
                }
                #endif
            }
        }
        #if UNITY_EDITOR
        else
        {
            m_DebugLastAIBehaviourResult = "Already jumped";
        }
        #endif
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
    
    /// Return the max distance over X possible when moving at current speed intention and with hypothetical jump
    /// with jumpSpeed
    private float CalculateDistanceXToJump(float jumpSpeed)
    {
        // Take scrolling speed into account (and readjust sign) to get the actual speed
        // If you don't, enemies that don't move by themselves (only follow scrolling) won't even be able to jump,
        // and enemies who do will still be considered slower than they really are (if moving left).
        // Note that environment is now static, only player character and camera are moving along scrolling.
        // But it doesn't matter, as the relative velocity between player character and grounded enemy is the same.
        float totalSpeedX = ScrollingManager.Instance.ComputeTotalSpeedWithScrolling(m_MoveGroundedIntention.signedGroundSpeed);
        
        // We don't support moving to the right, so only works when moving left in total
        float speedTowardLeft = Mathf.Max(0f, -totalSpeedX);
        float maxDistanceXToJump = speedTowardLeft * jumpSpeed/ GetGravity();
        return maxDistanceXToJump;
    }

    /// Return true is enemy is approaching target, in a range relevant to start computing whether
    /// they can jump and reach target or not. This acts as a pre-check for ShouldJump, which is more expensive.
    private bool IsPlayerCharacterCloseEnoughToCheckJump(Vector2 targetPosition)
    {
        // same calculation as MoveGrounded.FixedUpdate (X positive convention)
        float totalSpeedX = ScrollingManager.Instance.ComputeTotalSpeedWithScrolling(m_MoveGroundedIntention.signedGroundSpeed);

        // If enemy is not moving on screen, just pretend it has a very small speed toward the left,
        // so at least it tries to jump when character is very close; else jump range X would be reduced to a dot,
        // and player character float position is unlikely to ever reach it.
        // Note that a grounded enemy not moving on screen is rare, but could happen if it's ignoring scrolling speed
        // for some reason and thus stationary on-screen, like a tank that would move exactly at scrolling speed to
        // track the player character; or if scrolling stopped and enemy is not moving on ground).
        if (totalSpeedX == 0f)
        {
            // with this, sign of totalSpeedX will always make sense
            totalSpeedX = - 0.1f;
        }

        float xToTarget = targetPosition.x - transform.position.x;
        if (xToTarget != 0f && Mathf.Sign(xToTarget) != Mathf.Sign(totalSpeedX))
        {
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
        float maxDistanceXToJump = CalculateDistanceXToJump(moveGroundedParameters.maxJumpSpeed);
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
            float firstDistanceXToJump = CalculateDistanceXToJump(requiredJumpSpeed);

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
