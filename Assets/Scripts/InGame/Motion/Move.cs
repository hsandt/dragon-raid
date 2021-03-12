using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// System for Rigidbody2D and MoveIntention: handles move
public class Move : MonoBehaviour
{
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    private MoveIntention m_MoveIntention;
    
    private void Awake()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
        m_MoveIntention = this.GetComponentOrFail<MoveIntention>();
    }

    private void FixedUpdate()
    {
        m_Rigidbody2D.velocity = m_MoveIntention.moveVelocity;
    }
}
