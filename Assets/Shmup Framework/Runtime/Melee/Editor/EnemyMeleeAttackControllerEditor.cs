using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(EnemyMeleeAttackController))]
public class EnemyMeleeAttackControllerEditor : Editor
{
    private void OnSceneGUI()
    {
        if (Application.isPlaying)
        {
            var script = (EnemyMeleeAttackController) target;
            HandlesUtil.Label2D(script.transform.position + 1f * Vector3.up, script.DebugLastAIBehaviourResult, color: Color.black);
        }
    }
}
