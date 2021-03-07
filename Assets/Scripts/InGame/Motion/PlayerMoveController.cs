using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CommonsHelper;

/// Controller for Move on Player character
public class PlayerMoveController : MonoBehaviour
{
    /* Parameters data */
    
    public MoveParameters moveParameters;

    
    /* Sibling components */
    
    private MoveIntention m_MoveIntention;
    
    
    private void Awake()
    {
        m_MoveIntention = this.GetComponentOrFail<MoveIntention>();
    }
    
    /// PlayerInput action message callback for Move
    private void OnMove(InputValue value)
    {
        m_MoveIntention.moveVelocity = moveParameters.maxSpeed * value.Get<Vector2>();
    }
}
