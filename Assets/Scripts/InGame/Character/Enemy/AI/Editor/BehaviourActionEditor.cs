using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// Base class for all behaviour action editors
/// Make sure to define a child class editor for each action whose GetNodeName returns a dynamic node name based on
/// action properties, even if the class content is empty (no custom handles, etc.), just so OnInspectorGUI can refresh
/// the node name on property change.
[CustomEditor(typeof(BehaviourAction), editorForChildClasses: true)]
[CanEditMultipleObjects]
public class BehaviourActionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Detect any property change
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if (check.changed)
            {
                // Some property changes, so refresh node name for this action
                // (this is only useful if node name is dynamic and uses the changed property, but to simplify,
                // just refresh anyway)
                BehaviourTreeEditor.RefreshNodeNamesInWindowIfAny((BehaviourAction) target);
            }
        }
    }

    /// Helper method to call in OnSceneGUI of every child class
    /// This only exists because even if we define OnSceneGUI on this base class, it won't be called
    /// on the child classes like Awake or Start
    public void DrawLocalHandles()
    {
        // We know that target should be a BehaviourAction, although in this case we only need it as a Component
        var script = (Component) target;
        DrawHandles((Vector2) script.transform.position);
    }

    /// Draw editor handles for this action when starting from [startPosition]
    /// Must be called in custom Editor OnSceneGUI.
    public virtual void DrawHandles(Vector2 startPosition) {}

    /// Return the end position of the action when starting from [startPosition]
    /// Used to chain action handles at the correct positions.
    public virtual Vector2 ComputeEndPosition(Vector2 startPosition)
    {
        // Many actions don't move the character, so return startPosition in default implementation
        // so only actions that move character need to override this method
        return startPosition;
    }
}