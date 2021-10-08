using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Action to shoot a single bullet in a given direction
[AddComponentMenu("Game/Action: Shoot Single")]
public class Action_ShootSingle : BehaviourAction
{
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
    
    
    /* Owner sibling components */
    
    private Shoot m_Shoot;
    private ShootIntention m_ShootIntention;

    
    /* State */

    /// True when character has shot the single bullet
    private bool hasOrderedShot;
    
    
    public override void OnInit()
    {
        m_Shoot = m_EnemyCharacterMaster.GetComponentOrFail<Shoot>();
        m_ShootIntention = m_Shoot.ShootIntention;
    }
    
    public override void OnStart()
    {
        hasOrderedShot = false;
    }

    public override void RunUpdate()
    {
        // We're counting on IsOver to prevent double shooting, so no need to check hasOrderedShot here 
        m_ShootIntention.fireOnce = true;

        Vector2 referenceDirection;
        if (shootDirectionMode == EnemyShootDirectionMode.ShootAnchorForward)
        {
            // Note that it's generally Left on enemies
            referenceDirection = m_Shoot.shootAnchor.right;
        }
        else  // EnemyShootDirectionMode.TargetPlayerCharacter
        {
            Vector3 playerCharacterPosition = InGameManager.Instance.PlayerCharacterMaster.transform.position;
            // normalized is optional since fire direction will be normalized, but clearer to handle unit vectors
            referenceDirection = ((Vector2) (playerCharacterPosition - m_Shoot.shootAnchor.position)).normalized;
        }
        m_ShootIntention.fireDirection = VectorUtil.Rotate(referenceDirection, angle);
        
        hasOrderedShot = true;
    }
    
    protected override bool IsOver()
    {
        return hasOrderedShot;
    }

    public override void OnEnd()
    {
        // Optional cleanup
        // No need to clear fireOnce, as it is consumed
        m_ShootIntention.fireDirection = Vector2.zero;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
