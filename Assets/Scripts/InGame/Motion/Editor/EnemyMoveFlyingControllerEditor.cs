using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(EnemyMoveFlyingController))]
public class EnemyMoveFlyingControllerEditor : Editor
{
    public void OnSceneGUI()
    {
        var script = (EnemyMoveFlyingController) target;
            
        Vector2 startPosition = (Vector2)script.transform.position;
        Vector2 endPosition = startPosition + script.LinearMoveDirection;
        HandlesUtil.DrawArrow2D(startPosition, endPosition, Color.white);

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            HandlesUtil.DrawFreeMoveHandle(ref endPosition, Color.blue, capFunction: HandlesUtil.CrossedCircleHandleCap, screenSizeScale: 2f);  // moving direction handle
            
            if (check.changed)
            {
                // Don't normalize direction to make it easier to edit using snapping
                // with values like (-2, 1)
                // Move direction will be normalized before moving
                script.LinearMoveDirection = endPosition - startPosition;
            }
        }
    }
}
