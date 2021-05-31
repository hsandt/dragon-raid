using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(EnemyMoveGroundedController))]
public class EnemyMoveGroundedControllerEditor : Editor
{
    public void OnSceneGUI()
    {
        if (Application.isPlaying)
        {
            var script = (EnemyMoveGroundedController) target;
            HandlesUtil.Label2D(script.transform.position + 1f * Vector3.up, script.DebugLastAIBehaviourResult, color: Color.black);
        }
    }
}
