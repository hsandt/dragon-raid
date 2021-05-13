using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(EnemyMoveController))]
public class EnemyMoveControllerEditor : Editor
{
    public void OnSceneGUI()
    {
        var script = (EnemyMoveController) target;
            
        Vector2 startPosition = (Vector2)script.transform.position;
        Vector2 endPosition = startPosition + script.MoveDirection;
        HandlesUtil.DrawArrow2D(startPosition, endPosition, Color.white);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            HandlesUtil.DrawFreeMoveHandle(ref endPosition, Color.blue, capFunction: HandlesUtil.CrossedCircleHandleCap, screenSizeScale: 2f);  // moving direction handle
            
            if (check.changed)
            {
                // Don't normalize direction to make it easier to edit using snapping
                // with values like (-2, 1)
                // Move direction will be normalized before moving
                script.MoveDirection = endPosition - startPosition;
            }
        }
    }
}
