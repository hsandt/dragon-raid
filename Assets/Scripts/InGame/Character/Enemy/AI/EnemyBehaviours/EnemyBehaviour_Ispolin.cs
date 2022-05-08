using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

using CommonsHelper;
using CommonsPattern;
using UnityEngine.Serialization;

/// Behaviour for Enemy: Ispolin
/// System for MeleeAttackIntention and ThrowIntention: handles control
/// It manages MeleeAttackIntention and ThrowIntention is therefore responsible for their Setup.
/// SEO: before MeleeAttack and Throw
[AddComponentMenu("Game/Enemy Behaviour: Ispolin")]
public class EnemyBehaviour_Ispolin : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Detection Throw AI Parameters Data")]
    [InspectInline(canEditRemoteTarget = true)]
    public DetectionThrowAIParameters detectionThrowAiParameters;

    [Header("Child references")]

    [Tooltip("Position from which we cast the detection upper cone to find Throw target. Should be at eye level.")]
    public Transform throwDetectionOrigin;


    /* Sibling components */

    private MeleeAttack m_MeleeAttack;
    private MeleeAttackIntention m_MeleeAttackIntention;

    private Throw m_Throw;
    private ThrowIntention m_ThrowIntention;


    #if UNITY_EDITOR

    /* Debug */

    /// Last AI behaviour chosen by this controller
    private string m_DebugLastAIBehaviourResult = "None";
    public string DebugLastAIBehaviourResult => m_DebugLastAIBehaviourResult;

    #endif


    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(detectionThrowAiParameters, this, "[BodyAttack] Throw AI Parameters not set on Enemy Behaviour: Ispolin component {0}", this);
        #endif

        m_MeleeAttack = this.GetComponentOrFail<MeleeAttack>();
        m_MeleeAttackIntention = this.GetComponentOrFail<MeleeAttackIntention>();
        m_Throw = this.GetComponentOrFail<Throw>();
        m_ThrowIntention = this.GetComponentOrFail<ThrowIntention>();
    }

    public override void Setup()
    {
        m_MeleeAttackIntention.startAttack = false;

        m_ThrowIntention.startThrow = false;
        m_ThrowIntention.throwDirection = Vector2.zero;
        m_ThrowIntention.throwSpeed = 0f;
    }

    private void FixedUpdate()
    {
        // Only attack if can start new action (not already attacking, or attacking but can cancel)
        // In practice, enemies don't really have a Cancel phase (to force them to do heavy attacks and let player
        // estimate when they'll recover), but we check availability using the general API to be sure.
        if (m_MeleeAttack.CanStartNewAction() && !m_Throw.IsThrowing)
        {
            CheckNextAction();
        }
    }

    private void CheckNextAction()
    {
        // In priority order:
        // 1. check Melee Attack first (if detection area overlaps with Throw, Melee Attack will make more sense)
        // 2. check Throw
        if (InGameManager.Instance.PlayerCharacterMaster != null)
        {
            Vector2 targetPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            if (m_MeleeAttack.IsCloseEnoughToMeleeAttack(targetPosition))
            {
                OrderMeleeAttack();

                #if UNITY_EDITOR
                m_DebugLastAIBehaviourResult = "Melee Attack";
                #endif
            }
            else if (IsCloseEnoughToThrowRock(targetPosition))
            {
                // Note: target position is taken from present, but Ispolin Throw animation takes some time before
                // actually spanw the projectile. So, since Ispolin detected the target, it may have moved a bit.
                // If you want to make Ispolin more laggy, store the target position at the time it starts the Throw
                // animation. This will make it easier for player to dodge it.
                // Because we get the latest position of target, it may already be behind Ispolin or in some other
                // area normally out of detection zone. This may lead to unnatural throwing!
                // Reduce detection area and spawn projectile lag, or store target position at detection time to fix it!
                Vector2 throwDirection = CalculateThrowDirectionToHit(targetPosition);
                OrderThrow(throwDirection, detectionThrowAiParameters.throwSpeed);

                #if UNITY_EDITOR
                m_DebugLastAIBehaviourResult = "Throw Rock";
                #endif
            }
            #if UNITY_EDITOR
            else
            {
                m_DebugLastAIBehaviourResult = "Not close enough";
            }
            #endif
        }
        #if UNITY_EDITOR
        else
        {
            m_DebugLastAIBehaviourResult = "No Player Character";
        }
        #endif
    }

    /// Return true if targetPosition is close enough, and at the right angle, for Ispolin to throw a rock
    private bool IsCloseEnoughToThrowRock(Vector2 targetPosition)
    {
        bool canDetectTargetY = false;
        Vector2 toTarget = targetPosition - (Vector2) throwDetectionOrigin.position;

        // we are only checking for target on the left, far enough, so we expect negative distance on X
        if (toTarget.x > - detectionThrowAiParameters.minDetectionDistanceX)
        {
            // target is behind, or on front but too close on X
            return false;
        }

        if (toTarget.y <= 0f)
        {
            // target is below eye level, Y is OK
            canDetectTargetY = true;
        }
        else
        {
            // target is above eye level, detect target up to 45 degrees upward
            // we assume Ispolin is always facing left
            float toTargetAngle = Vector2.Angle(Vector2.left, targetPosition);
            if (toTargetAngle <= detectionThrowAiParameters.maxDetectionUpwardAngle)
            {
                canDetectTargetY = true;
            }
        }

        // if Y can be detected, also check (sqr) distance to target
        return canDetectTargetY && toTarget.sqrMagnitude <= detectionThrowAiParameters.maxDetectionDistance * detectionThrowAiParameters.maxDetectionDistance;
    }

    /// Ballistic method that calculates the direction required to hit a target at a given position,
    /// given the current throw speed parameter
    private Vector2 CalculateThrowDirectionToHit(Vector2 targetPosition)
    {
        // Unfortunately we cannot easily access the gravity scale of the Rock projectile because we defined it by name on Throw (projectileName)
        // To access the Rigidbody2D component of the Rock, we'd either need to define a reference to prefab (from which we can still get the name for pooling),
        // then get the component directly from the prefab; or get a pooled instance of the Rock. For now, gravity scale of the rock is 1 so we don't need to get that
        // and simply use the physics gravity. Note that we pass the gravity vector directly as our formula takes general gravity, although we don't use gravity on X.

        // The method CalculateFiringSolution is basically working, but we decided it was too risky for now,
        // so throw in straight line + small upward offset until we can fix all edge cases and assertions
        return new Vector2(-1f, 1f);

        /*
        Vector2 toTarget = targetPosition - (Vector2) m_Throw.throwAnchor.position;
        if (toTarget.y <= 0f)
        {
            return toTarget;
        }
        else
        {
            return toTarget + Vector2.up * 2f;
        }

        bool result = PhysicsPrediction.CalculateFiringSolution(m_Throw.throwAnchor.position, targetPosition, detectionThrowAiParameters.projectileSpeed, Physics2D.gravity, out Vector2 aimDirection);
        if (result)
        {
            return aimDirection;
        }
        else
        {
            // this method must return something, so return a classic 45-degree direction to the left-up, but warn
            Debug.LogWarningFormat(this, "PhysicsPrediction.CalculateFiringSolution failed to find a firing solution for {0}. " +
                                         "Is target too far for projectile speed?", this);
            return new Vector2(-1f, 1f);
        }
        */
    }

    private void OrderMeleeAttack()
    {
        m_MeleeAttackIntention.startAttack = true;
    }

    private void OrderThrow(Vector2 aimDirection, float throwSpeed)
    {
        m_ThrowIntention.startThrow = true;
        m_ThrowIntention.throwDirection = aimDirection;
        m_ThrowIntention.throwSpeed = throwSpeed;
    }
}
