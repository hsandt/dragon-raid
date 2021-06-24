using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Throw))]
public class ThrowEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var script = (Throw) target;
        
        ThrowParameters throwParameters = script.throwParameters;
        if (throwParameters != null)
        {
            EditorGUILayout.LabelField("Parameters quick view", EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.FloatField("Max Detection Upward Angle", throwParameters.maxDetectionUpwardAngle);
            }
        }
    }
}