using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(Action_MoveAlongPath))]
public class Action_MoveAlongPathEditor : BehaviourActionEditor
{
    private void OnSceneGUI()
    {
        DrawLocalHandles();
    }

    public override void DrawHandles(Vector2 startPosition)
    {
        var script = (Action_MoveAlongPath) target;

        // TODO: use existing path editor to draw handles
    }

    public override void OnInspectorGUI()
    {
        var script = (Action_MoveAlongPath) target;

        base.OnInspectorGUI();

        float estimatedArcLength = script.path2DComponent.Path.EvaluateLength(100);
        EditorGUILayout.LabelField("Arc length", estimatedArcLength.ToString("0.00"));
        float estimatedTravelDuration = estimatedArcLength / script.Speed;
        EditorGUILayout.LabelField("Approximate duration", estimatedTravelDuration.ToString("0.00"));
    }
}