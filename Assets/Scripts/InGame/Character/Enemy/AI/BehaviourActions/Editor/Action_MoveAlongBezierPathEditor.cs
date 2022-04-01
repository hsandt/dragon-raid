using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(Action_MoveAlongBezierPath))]
public class Action_MoveAlongBezierPathEditor : BehaviourActionEditor
{
    private void OnSceneGUI()
    {
        DrawLocalHandles();
    }

    public override void DrawHandles(Vector2 startPosition)
    {
        var script = (Action_MoveAlongBezierPath) target;
    }

    public override void OnInspectorGUI()
    {
        var script = (Action_MoveAlongBezierPath) target;

        base.OnInspectorGUI();

        float estimatedArcLength = script.bezierPath2DComponent.Path.EvaluateLength(100);
        EditorGUILayout.LabelField("Arc length", estimatedArcLength.ToString("0.00"));
        float estimatedTravelDuration = estimatedArcLength / script.Speed;
        EditorGUILayout.LabelField("Approximate duration", estimatedTravelDuration.ToString("0.00"));
    }
}