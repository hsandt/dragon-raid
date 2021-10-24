using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(Action_MoveFlyingToward))]
public class Action_MoveFlyingTowardEditor : Editor
{
    // Factor applied to avoid placing the speed handle too far from the center to reach high speeds
    private const float SPEED_HANDLE_DISTANCE_FACTOR = 0.5f;

    private void OnSceneGUI()
    {
        var script = (Action_MoveFlyingToward) target;

        Vector2 startPosition = (Vector2) script.transform.position;
        Vector2 direction = VectorUtil.Rotate(Vector2.left, script.Angle);

        // Apply factor to draw speed handle closer to center
        float speedHandleDistance = SPEED_HANDLE_DISTANCE_FACTOR * script.Speed;
        Vector2 speedHandlePosition = startPosition + speedHandleDistance * direction;

        // Velocity arrow
        HandlesUtil.DrawArrow2D(startPosition, speedHandlePosition, ColorUtil.gold);

        // Circle that contains the speed handle for easier visualization of magnitude
        Handles.DrawWireDisc(startPosition, Vector3.forward, speedHandleDistance);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Angle handle
            float angle = script.Angle;
            HandlesUtil.DrawAngleHandle(startPosition, 1f, Vector2.left, ref angle,
                ColorUtil.orange, ColorUtil.gold, HandlesUtil.CrossedCircleHandleCap, 2f);

            if (check.changed)
            {
                Undo.RecordObject(script, "Edit Angle");
                script.Angle = angle;
            }
        }

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Speed handle
            // Snap is less relevant when moving a point along a radial direction
            HandlesUtil.DrawFreeMoveHandle(ref speedHandlePosition, Color.magenta, null,
                HandlesUtil.CrossedCircleHandleCap, 1.5f);

            if (check.changed)
            {
                Undo.RecordObject(script, "Edit Speed");
                Vector2 newVelocity = speedHandlePosition - startPosition;

                // Apply factor inverse to convert distance to speed handle back to actual speed
                script.Speed = newVelocity.magnitude / SPEED_HANDLE_DISTANCE_FACTOR;
            }
        }
    }
}