using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

using CommonsHelper;

/// Action to wait for some time
[AddComponentMenu("Game/Action: Wait")]
public class Action_Wait : BehaviourAction
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Wait duration (s)")]
    [Min(0f)]
    private float duration = 1f;

    
    /* State */

    /// Time since action start
    private float m_Time;
    
    
    public override void OnStart()
    {
        m_Time = 0f;
    }

    public override void RunUpdate()
    {
        m_Time += Time.deltaTime;
    }
    
    protected override bool IsOver()
    {
        return m_Time >= duration;
    }

    public override float GetEstimatedDuration()
    {
        throw new System.NotImplementedException();
    }
}
