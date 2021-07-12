using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(EnemyBehaviour_Ispolin))]
public class EnemyBehaviour_IspolinEditor : Editor
{
    static Editor previousEditor;
    
    public void OnSceneGUI()
    {
        var script = (EnemyBehaviour_Ispolin) target;
        
        if (script.throwAIParameters)
        {
            // Draw vision "cone"
            // Enemy is facing left, so angle upward should go CW from the Left vector, which is negative in Unity convention
            // Also, we work with 3D vectors, so make sure to pass Vector3.left so VectorUtil.Rotate picks the correct overload
            Vector3 detectionOrigin = script.throwDetectionOrigin.position;
            Vector3 upwardDelta = 5f * VectorUtil.Rotate(Vector3.left, - script.throwAIParameters.maxDetectionUpwardAngle);
            Vector3 detectionUpwardAngleHandlePos = detectionOrigin + upwardDelta;
            HandlesUtil.DrawLine(detectionOrigin, detectionUpwardAngleHandlePos, ColorUtil.orange);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                HandlesUtil.DrawFreeMoveHandle(ref detectionUpwardAngleHandlePos, ColorUtil.orange);
                
                if (check.changed)
                {
                    // Scriptable Object is on a different object, so we need to mark it as dirty manually
                    EditorUtility.SetDirty(script.throwAIParameters);
                    Undo.RecordObject(script.throwAIParameters, "Changed Throw Parameters (maxDetectionUpwardAngle)");
                    script.throwAIParameters.maxDetectionUpwardAngle = Vector3.Angle(Vector3.left, detectionUpwardAngleHandlePos - detectionOrigin);
                    
                    // EditScriptableAttribute does not provide auto-refresh on inlined scriptable object,
                    // so manually refresh this custom editor on Handles move
                    Repaint();
                }
            }
        }
        
        if (Application.isPlaying)
        {
            HandlesUtil.Label2D(script.transform.position + 1f * Vector3.up, script.DebugLastAIBehaviourResult, color: Color.black);
        }
    }
}
