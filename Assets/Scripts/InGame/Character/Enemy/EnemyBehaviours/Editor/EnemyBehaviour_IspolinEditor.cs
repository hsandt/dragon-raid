using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

[CustomEditor(typeof(EnemyBehaviour_Ispolin))]
public class EnemyBehaviour_IspolinEditor : Editor
{
    public void OnSceneGUI()
    {
        var script = (EnemyBehaviour_Ispolin) target;
        
        var throwComponent = script.GetComponent<Throw>();
        if (throwComponent)
        {
            ThrowParameters throwParameters = throwComponent.throwParameters;
            if (throwParameters)
            {
                // Draw vision "cone"
                // Enemy is facing left, so angle upward should go CW from the Left vector, which is negative in Unity convention
                // Also, we work with 3D vectors, so make sure to pass Vector3.left so VectorUtil.Rotate picks the correct overload
                Vector3 detectionOrigin = script.throwDetectionOrigin.position;
                Vector3 upwardDelta = 5f * VectorUtil.Rotate(Vector3.left, - throwParameters.maxDetectionUpwardAngle);
                Vector3 detectionUpwardAngleHandlePos = detectionOrigin + upwardDelta;
                HandlesUtil.DrawLine(detectionOrigin, detectionUpwardAngleHandlePos, ColorUtil.orange);
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    HandlesUtil.DrawFreeMoveHandle(ref detectionUpwardAngleHandlePos, ColorUtil.orange);
                    
                    if (check.changed)
                    {
                        // Known issue: Undo works, but file is not saved, even if you Ctrl+S
                        // It probably needs a AssetDatabase.SaveAssets() but at the same time, you don't really want to force
                        // saving until you're done, let alone every frame while dragging the Handle. I need to find a way
                        // to just mark the asset as dirty.
                        Undo.RecordObject(throwParameters, "Changed Throw Parameters (maxDetectionUpwardAngle)");
                        throwParameters.maxDetectionUpwardAngle = Vector3.Angle(Vector3.left, detectionUpwardAngleHandlePos - detectionOrigin);
                        
                        // HACK: create Throw custom inspector (we'd rather get the existing one, but not sure how to) and force Repaint so
                        // the Parameters quick view is refreshed
                        // Consider showing this on EnemyBehaviour script instead
                        Editor editor = CreateEditor(throwComponent);
                        editor.Repaint();
                    }
                }
            }
        }
        
        if (Application.isPlaying)
        {
            HandlesUtil.Label2D(script.transform.position + 1f * Vector3.up, script.DebugLastAIBehaviourResult, color: Color.black);
        }
    }
}
