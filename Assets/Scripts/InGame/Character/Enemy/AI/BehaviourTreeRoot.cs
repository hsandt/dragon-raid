using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsDebug;
using CommonsHelper;

/// Root of the behaviour tree, it only acts as a proxy for the actual root action
/// But it is useful to have this proxy to have some stable component reference in scene,
/// as the root action itself may be swapped with another type of action component while designing.
public class BehaviourTreeRoot : MonoBehaviour
{
    /* Cached child references */

    /// Root action: behaviour action on unique child
    private BehaviourAction m_RootAction;


    private void Awake()
    {
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            m_RootAction = transform.GetChild(0).GetComponentOrFail<BehaviourAction>();

            DebugUtil.AssertFormat(childCount == 1, this,
                "[BehaviourTreeRoot] There are {0} children, expected 1. The first child will be considered " +
                "as root action, other children will be ignored.", childCount);
        }
        else
        {
            DebugUtil.LogError("[BehaviourTreeRoot] There are no children, expected 1. " +
                "Cannot register root action, further execution will cause null reference exceptions.", this);
        }
    }

    /// Delegate initialize with owner to root action
    public void Init(EnemyCharacterMaster enemyCharacterMaster)
    {
        m_RootAction.Init(enemyCharacterMaster);
    }

    /// Delegate Start to root action
    public void OnStart()
    {
        m_RootAction.OnStart();
    }

    /// Delegate Run Update to root action
    public void RunUpdate()
    {
        m_RootAction.RunUpdate();
    }

    /// Delegate Is Over / Deactivated check to root action
    public bool IsOverOrDeactivated()
    {
        return m_RootAction.IsOverOrDeactivated();
    }

    /// Delegate End to root action
    public void OnEnd()
    {
        m_RootAction.OnEnd();
    }
}
