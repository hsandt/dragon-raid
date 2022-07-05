using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(Action_ShootSingle))]
public class Action_ShootSingleEditor : BehaviourActionEditor
{
    private void OnSceneGUI()
    {
        DrawLocalHandlesWithLabel();
    }

    public override void DrawHandles(Vector2 startPosition)
    {
        var script = (Action_ShootSingle) target;
        
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
            // Angle handle
            float angle = script.Angle;
            
            // We cannot get Player Character in Edit mode, so we ignore script.ShootDirectionMode and just use
            // shootAnchor.right (often Vector2.left) even if shoot direction mode is EnemyShootDirectionMode.TargetPlayerCharacter,
            // instead of using Shoot.GetBaseFireDirection.
            HandlesUtil.DrawAngleHandle(startPosition, 1f, shoot.shootAnchor.right, ref angle,
                ColorUtil.orange, ColorUtil.gold, ColorUtil.quarterInvisibleWhite, HandlesUtil.CrossedCircleHandleCap, 2f);

            if (check.changed)
            {
                Undo.RecordObject(script, "Edit Shoot Single Angle");
                script.Angle = angle;
            }
        }
    }
}