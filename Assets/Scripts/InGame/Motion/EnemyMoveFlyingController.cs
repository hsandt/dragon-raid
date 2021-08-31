using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;
using CommonsPattern;
using UnityEngine.Serialization;

/// System for MoveFlyingIntention on Enemy: handles control
/// SEO: before MoveFlying
public class EnemyMoveFlyingController : BaseMoveFlyingController
{
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

    /// Used for Wave only: Time since motion start, modulo motion period
    private float m_CurrentTimeModulo;
    
    
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

        m_CurrentTimeModulo = 0f;
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
        }
    }
}
