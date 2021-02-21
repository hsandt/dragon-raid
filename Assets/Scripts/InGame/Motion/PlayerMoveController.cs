using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void FixedUpdate()
    {
        // for testing: just go right at max speed
        m_MoveIntention.moveVelocity = moveParameters.maxSpeed * Vector2.right;
    }
}
