using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using CommonsHelper;

[CustomEditor(typeof(Action_MoveFlyingBy))]
public class Action_MoveFlyingByEditor : BehaviourActionEditor
{
    // Factor applied to avoid placing the speed handle too far from the center to reach high speeds
    private const float SPEED_HANDLE_DISTANCE_FACTOR = 0.5f;


    private void OnSceneGUI()
    {
        DrawLocalHandles();
    }

    public override void DrawHandles(Vector2 startPosition)
    {
        var script = (Action_MoveFlyingBy) target;
        Vector2 moveVector = script.MoveVector;

        // Move arrow
        HandlesUtil.DrawArrow2D(startPosition, startPosition + moveVector, ColorUtil.gold);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Move vector handle
            Vector2 relativeTargetPosition = startPosition + moveVector;

            HandlesUtil.DrawSlider2D(ref relativeTargetPosition, ColorUtil.gold,
                capFunction: HandlesUtil.CrossedCircleHandleCap, screenSizeScale: 2f);

            if (check.changed)
            {
                Undo.RecordObject(script, "Edit Move Vector");
                script.MoveVector = relativeTargetPosition - startPosition;
            }
        }

        Vector2 moveDirection = moveVector.normalized;

        // Only draw speed arrow and handle if move vector is not (almost) zero
        if (moveDirection != Vector2.zero)
        {
            // Apply factor to draw speed handle closer to center
            float speedHandleDistance = SPEED_HANDLE_DISTANCE_FACTOR * script.Speed;
            Vector2 speedHandlePosition = startPosition + speedHandleDistance * moveDirection;

            // Velocity arrow
            HandlesUtil.DrawArrow2D(startPosition, speedHandlePosition, Color.magenta);

            // Circle that contains the speed handle for easier visualization of magnitude
            HandlesUtil.DrawCircle2D(startPosition, speedHandleDistance, Color.magenta);

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                // Speed handle
                // Snap is less relevant when moving a point along a radial direction
                HandlesUtil.DrawSlider2D(ref speedHandlePosition, Color.magenta,
                    capFunction: HandlesUtil.CrossedCircleHandleCap, screenSizeScale: 1.5f);

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

    public override Vector2 ComputeEndPosition(Vector2 startPosition)
    {
        var script = (Action_MoveFlyingBy) target;
        Vector2 moveVector = script.MoveVector;
        return startPosition + moveVector;
    }
}