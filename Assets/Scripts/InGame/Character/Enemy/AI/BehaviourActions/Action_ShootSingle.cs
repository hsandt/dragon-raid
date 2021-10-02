using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to shoot a single bullet in a given direction
[AddComponentMenu("Game/Action: Shoot Single")]
public class Action_ShootSingle : BehaviourAction
{
    [Header("Parent references")]
    
    [Tooltip("Shoot component of the character (Shoot Intention is accessed via it)")]
    public Shoot shoot;

    
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Which angle reference to use for the shot. " +
        "Shoot Anchor Forward: use anchor's Right direction. " +
        "Target Player Character: use direction from shoot anchor to Player Character.")]
    private EnemyShootDirectionMode shootDirectionMode = EnemyShootDirectionMode.ShootAnchorForward;

    [SerializeField, Tooltip("Angle to shoot at, from the reference angle defined by Shoot Direction Mode " +
        "(degrees, 0 for forward, positive CCW)")]
    [Range(-180f, 180f)]
    private float angle;

    #if UNITY_EDITOR
    public float Angle { get => angle; set => angle = value; }
    #endif
    
    
    /* State */

    /// True when character has shot the single bullet
    private bool hasOrderedShot;
    
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    void Awake()
    {
        Debug.AssertFormat(shoot != null, this, "[Action_ShootSingle] No Shoot component reference set on {0}.", this);
    }
    #endif
    
    public override void OnStart()
    {
        hasOrderedShot = false;
    }

    public override void RunUpdate()
    {
        // We're counting on IsOver to prevent double shooting, so no need to check hasOrderedShot here 
        shoot.ShootIntention.fireOnce = true;

        Vector2 referenceDirection;
        if (shootDirectionMode == EnemyShootDirectionMode.ShootAnchorForward)
        {
            // Note that it's generally Left on enemies
            referenceDirection = shoot.shootAnchor.right;
        }
        else  // EnemyShootDirectionMode.TargetPlayerCharacter
        {
            Vector3 playerCharacterPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            // normalized is optional since fire direction will be normalized, but clearer to handle unit vectors
            referenceDirection = ((Vector2) (playerCharacterPosition - shoot.shootAnchor.position)).normalized;
        }
        shoot.ShootIntention.fireDirection = VectorUtil.Rotate(referenceDirection, angle);
        
        hasOrderedShot = true;
    }
    
    public override bool IsOver()
    {
        return hasOrderedShot;
    }

    public override void OnEnd()
    {
        // Optional cleanup
        // No need to clear fireOnce, as it is consumed
        shoot.ShootIntention.fireDirection = Vector2.zero;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
