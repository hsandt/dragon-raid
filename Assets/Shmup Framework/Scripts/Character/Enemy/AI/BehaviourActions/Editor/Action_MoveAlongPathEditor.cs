using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;
using CommonsHelper.Editor;

[CustomEditor(typeof(Action_MoveAlongPath))]
public class Action_MoveAlongPathEditor : BehaviourActionEditor
{
    /* Cached objects */

    /// Cached path component, used to verify if it changed and we should still use associated cached editor
    private Path2DComponent m_CachedPath2DComponent;

    /// Cached editor for action, with action script instance ID as key
    private Path2DComponentEditor m_CachedPath2DComponentEditor;


    private void OnSceneGUI()
    {
        DrawLocalHandlesWithLabel();
    }

    public override void OnInspectorGUI()
    {
        var script = (Action_MoveAlongPath) target;

        base.OnInspectorGUI();

        if (script.path2DComponent != null)
        {
            float estimatedArcLength = script.path2DComponent.Path.EvaluateLength(100);
            EditorGUILayout.LabelField("Arc length", estimatedArcLength.ToString("0.00"));
            float estimatedTravelDuration = estimatedArcLength / script.AnchorSpeed;
            EditorGUILayout.LabelField("Approximate duration", estimatedTravelDuration.ToString("0.00"));
        }
    }

    public override string CheckHandlesError()
    {
        var script = (Action_MoveAlongPath) target;
        return script.path2DComponent == null ? "Missing path 2D component" : null;
    }

    public override void DrawHandles(Vector2 startPosition)
    {
        var script = (Action_MoveAlongPath) target;

        Path2DComponent path2DComponent = script.path2DComponent;

        // When only selecting game object with path component, we automatically show editable path as part of
        // that component's custom editor. So to avoid duplicate drawing, do not draw again.
        // (and do not even create/cache editor to reduce cost until we need it)
        // Generally, path component is on same object as action component, but it may be somewhere else, so make sure
        // to check the game object having the path component.
        if (Selection.activeGameObject == path2DComponent.gameObject)
        {
            return;
        }

        if (path2DComponent != m_CachedPath2DComponent)
        {
            // Cache was invalidated: update both cached component and editor
            // If there is no component at all, just clear cached editor
            m_CachedPath2DComponent = path2DComponent;
            m_CachedPath2DComponentEditor = path2DComponent != null ? CreateEditor(path2DComponent) as Path2DComponentEditor : null;
        }

        if (m_CachedPath2DComponentEditor != null)
        {
            // Delegate Handles drawing to root action
            // Assume CheckHandlesError was checked by caller, so we can safely use script.path2DComponent
            m_CachedPath2DComponentEditor.DrawEditablePath(path2DComponent);
        }
    }
}