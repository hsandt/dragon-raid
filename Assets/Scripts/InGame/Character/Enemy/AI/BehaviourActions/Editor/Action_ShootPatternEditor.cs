using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(Action_ShootPattern))]
public class Action_ShootPatternEditor : BehaviourActionEditor
{
    private void OnSceneGUI()
    {
        DrawLocalHandlesWithLabel();
    }

    public override void DrawHandles(Vector2 startPosition)
    {
        var script = (Action_ShootPattern) target;

        ShootPattern shootPattern = script.shootPattern;
        if (shootPattern == null)
        {
            return;
        }

        // We don't know where the owner is located, so search Shoot component on parents
        // as Actions are often part of a Behaviour Tree under the character root.
        // TODO: support Override BT under Enemy Spawn Wave too, but retrieving an Enemy Prefab may be costly,
        // so consider defaulting shootAnchor to current transform position if no Shoot component is found at all
        var shoot = script.GetComponentInParent<Shoot>();
        if (shoot == null)
        {
            return;
        }

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            // Scriptable Object is on a different object, so we need to mark it as dirty manually (still needs Undo)
            EditorUtility.SetDirty(shootPattern);
            Undo.RecordObject(shootPattern, "Edit Shoot Pattern Angle");

            // Angle start handle
            // We cannot get Player Character in Edit mode, so we ignore script.ShootDirectionMode and just use
            // shootAnchor.right (often Vector2.left) even if shoot direction mode is EnemyShootDirectionMode.TargetPlayerCharacter,
            // instead of using Shoot.GetBaseFireDirection.
            Vector2 baseFireDirection = (Vector2) shoot.shootAnchor.right;
            HandlesUtil.DrawAngleRangeHandle(startPosition, 1f, baseFireDirection, ref shootPattern.angleStart, ref shootPattern.angleEnd,
                ColorUtil.orange, ColorUtil.brown, ColorUtil.gold, ColorUtil.pink, ColorUtil.quarterInvisibleWhite,
                HandlesUtil.CrossedCircleHandleCap, 2f);

            // Read-only: show each bullet, based on bullet count, with a line indicating the fire angle
            foreach (float fireAngle in script.ComputeFireAngles(0, shootPattern.bulletCount))
            {
                HandlesUtil.DrawLine2D(startPosition, startPosition + VectorUtil.Rotate(baseFireDirection, fireAngle), Color.yellow);
            }

            if (check.changed)
            {
                // InspectInline attribute does not provide auto-refresh on inlined scriptable object,
                // so manually refresh this custom editor on Handles move
                Repaint();
            }
        }
    }
}