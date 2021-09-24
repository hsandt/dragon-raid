using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using UnityEngine.Serialization;

/// System for MoveFlyingIntention on Enemy: handles control
/// SEO: before MoveFlying
public class EnemyMoveFlyingController : BaseMoveFlyingController
{
    /// Maximum distance along dive direction normal allowed behind this enemy to detect target to start Dive 
    private const float DIVE_DETECTION_BEHIND_THRESHOLD = 1f;
    
    [Header("Parameters data")]

    [Tooltip("Enemy Move Parameters Data")]
    public EnemyMoveFlyingParameters enemyMoveFlyingParameters;

    
    [Header("Individual parameters")]
    
    [Header("Linear motion")]
    
    [SerializeField, Tooltip("Fixed direction in which the enemy moves (all enemies of the same type will move the same) " +
                             "Will be normalized.")]
    [FormerlySerializedAs("moveDirection")]
    private Vector2 linearMoveDirection = Vector2.left;

#if UNITY_EDITOR
    public Vector2 LinearMoveDirection { get => linearMoveDirection; set => linearMoveDirection = value; }
#endif
    
    
    /* State */

    /// Used for Move Paths with linear timers: Time since current phase start
    private float m_CurrentTime;
    
    /// Used for Move Paths with cyclic timers: Time since current phase start, modulo motion period
    private float m_CurrentTimeModulo;
    
    /// Used for Move Paths with several phases: DiveLinear. Use int instead of enum so it can be used across different
    /// Move Path Types, as more generic.
    private int m_CurrentPhaseIndex;
    
    
    protected override void Init()
    {
        base.Init();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(enemyMoveFlyingParameters != null, this, "[EnemyMoveFlyingController] Init: Enemy Move Flying Parameters not set on {0}", this);
        #endif
    }

    public override void Setup()
    {
        base.Setup();

        m_CurrentTime = 0f;
        m_CurrentTimeModulo = 0f;
        m_CurrentPhaseIndex = 0;
    }

    private void FixedUpdate()
    {
        switch (enemyMoveFlyingParameters.movePathType)
        {
            case MovePathType.Linear:
                // Linear motion
                m_MoveFlyingIntention.moveVelocity = enemyMoveFlyingParameters.linearMaxSpeed * linearMoveDirection.normalized;
                break;
            case MovePathType.Wave:
                // Applying modulo is optional since Cos() is periodic, and since enemies leave screen or die pretty quickly,
                // I wouldn't say it avoids overflow either, but it can help keeping small numbers for debugging.
                m_CurrentTimeModulo = (m_CurrentTimeModulo + Time.deltaTime) % enemyMoveFlyingParameters.wavePeriod;
                
                // Wave motion
                // Sinusoidal pattern, always go left at given horizontal speed,
                // and oscillate vertically with given half-height and period
                m_MoveFlyingIntention.moveVelocity = new Vector2(
                    - enemyMoveFlyingParameters.waveHorizontalSpeed,
                    // Y(t) = A * sin(2*pi*t/T) => Y'(t) = A * 2*pi/T * cos(2*pi*t/T)
                    // (sin is fine too for velocity, but cos makes sure character stays vertically centered around start position)
                    enemyMoveFlyingParameters.waveHalfHeight * 2 * Mathf.PI / enemyMoveFlyingParameters.wavePeriod * Mathf.Cos(2 * Mathf.PI * m_CurrentTimeModulo / enemyMoveFlyingParameters.wavePeriod)
                );
                break;
            case MovePathType.LinearDive:
                switch (m_CurrentPhaseIndex)
                {
                    case 0:
                    {
                        // Phase 0: Linear motion like Linear
                        m_MoveFlyingIntention.moveVelocity = enemyMoveFlyingParameters.linearMaxSpeed * linearMoveDirection.normalized;

                        // Check if enemy is in Dive area
                        if (IsPlayerCharacterInsideDiveDetectionArea())
                        {
                            // Enter phase 1: Dive (don't mind the 1-frame lag due to still applying Linear motion this frame)
                            m_CurrentPhaseIndex = 1;
                            m_CurrentTime = 0f;
                        }

                        break;
                    }
                    case 1:
                    {
                        // Phase 1: Dive on target
                        m_CurrentTime = m_CurrentTime + Time.deltaTime;
                        
                        // Angle is signed, positive CCW from world left
                        Vector2 diveDirection = VectorUtil.Rotate(Vector2.left, enemyMoveFlyingParameters.diveAngle);
                        m_MoveFlyingIntention.moveVelocity = enemyMoveFlyingParameters.diveSpeed * diveDirection;
                        
                        // After some time, recover
                        if (m_CurrentTime > enemyMoveFlyingParameters.diveDuration)
                        {
                            // Enter phase 2: Recover
                            m_CurrentPhaseIndex = 2;
                            m_CurrentTime = 0f;
                        }
                        
                        break;
                    }
                    case 2:
                    {
                        // Phase 2: Recover position to continue on initial trajectory
                        m_CurrentTime = m_CurrentTime + Time.deltaTime;

                        // To do this, move with the dive vector reflected by the initial direction vector,
                        // also at dive speed
                        Vector2 diveDirection = VectorUtil.Rotate(Vector2.left, enemyMoveFlyingParameters.diveAngle);
                        Vector2 recoverDirection = VectorUtil.Mirror(diveDirection, linearMoveDirection.normalized);
                        m_MoveFlyingIntention.moveVelocity = enemyMoveFlyingParameters.diveSpeed * recoverDirection;
                        
                        // After same duration as dive, resume initial trajectory
                        // Unless they were blocked by the environment, this should place them exactly on the initial trajectory
                        if (m_CurrentTime > enemyMoveFlyingParameters.diveDuration)
                        {
                            // Enter phase 3: Resume initial trajectory
                            m_CurrentPhaseIndex = 3;
                            m_CurrentTime = 0f;
                        }
                        
                        break;
                    }
                    case 3:
                    {
                        // Phase 3: Resume initial trajectory (same as Phase 0)
                        m_MoveFlyingIntention.moveVelocity = enemyMoveFlyingParameters.linearMaxSpeed * linearMoveDirection.normalized;
                        break;
                    }
                    default:
                        #if UNITY_EDITOR || DEVELOPMENT_BUILD
                        Debug.LogErrorFormat(this, "[EnemyMoveFlyingController] {0} has Move Path Type {1} but Current Phase Index {2} is invalid",
                            this, enemyMoveFlyingParameters.movePathType, m_CurrentPhaseIndex);
                        #endif
                        break;
                }
                break;
            default:
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogErrorFormat(enemyMoveFlyingParameters, "[EnemyMoveFlyingController] {0} has unhandled Move Path Type {1}",
                    enemyMoveFlyingParameters, enemyMoveFlyingParameters.movePathType);
                #endif
                break;
        }
    }
    
    /// Return true iff player character is inside Dive detection area
    private bool IsPlayerCharacterInsideDiveDetectionArea()
    {
        if (enemyMoveFlyingParameters.diveAngle == 0f)
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.LogError("[EnemyMoveFlyingController] Cannot decide if target is inside dive detection area when Dive Angle is 0");
            #endif
            return false;
        }
        
        Vector2 targetPosition = (Vector2)InGameManager.Instance.PlayerCharacterMaster.transform.position;
        Vector2 toTarget = targetPosition - (Vector2)transform.position;

        // the target must be below this enemy, i.e. on the side of the dive direction (relative to initial move direction)
        // when diving down, angle is positive and normal is a rotation CCW / up, negative, CW resp.
        Vector2 initialDirectionNormalDiveSide = VectorUtil.Rotate(Vector2.left, enemyMoveFlyingParameters.diveAngle > 0f ? 90f : -90f);
        float diveDotProduct = Vector2.Dot( initialDirectionNormalDiveSide, toTarget);

        // dot product should be positive when target is in dive side
        if (diveDotProduct < 0)
        {
            return false;
        }
        
        // to get the inside normal of the dive detection area's outer edge,
        // rotate the dive direction by 90 degrees CCW, which is equivalent to rotating world down
        // instead of world left, by dive angle
        Vector2 normal = VectorUtil.Rotate(Vector2.down, enemyMoveFlyingParameters.diveAngle);

        // compute dot product with inside normal to know on which side the target is
        float normalDotProduct = Vector2.Dot(normal, toTarget);

        // dot product is positive when target is inside area
        // in addition, we don't want to detect a target too far behind, so we check that dot product
        // has a value that is not too low
        return 0 < normalDotProduct && normalDotProduct < DIVE_DETECTION_BEHIND_THRESHOLD;
    }
}
