using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BehaviourTreeRoot))]
public class BehaviourTreeRootEditor : Editor
{
    /* Cached objects */

    /// Cached root action, used to verify if it changed and we should still use associated cached editor
    private BehaviourAction m_CachedRootAction;

    /// Cached editor for root action, with action script instance ID as key
    private BehaviourActionEditor m_CachedRootActionEditor;


    private void OnSceneGUI()
    {
        var script = (BehaviourTreeRoot)target;

        BehaviourAction rootAction = script.GetRootAction();

        if (rootAction != m_CachedRootAction)
        {
            // Cache was invalidated: update both cached action and editor
            // If there is no action at all, just clear cached editor
            m_CachedRootAction = rootAction;
            m_CachedRootActionEditor = rootAction != null ? CreateEditor(rootAction) as BehaviourActionEditor : null;
        }

        if (m_CachedRootActionEditor != null)
        {
            // Delegate Handles drawing to root action
            m_CachedRootActionEditor.DrawLocalHandles();
        }
    }
}
