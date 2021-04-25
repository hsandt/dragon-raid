using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using CommonsHelper;

[CustomEditor(typeof(EnemyWave))]
public class EnemyWaveEditor : Editor
{
    /* Parameters */
    
    /// Position snapping distance. Set to 1px for pixel perfect placement.
    private float autoSnapValue = 1f / 16f;
    
    /// Position snapping distance when holding Ctrl. Bigger than auto-snap, used for broad placement.
    private float manualSnapValue = 1f;
    
    /// Color used for spawn point debug and enemy label
    private readonly Color spawnPointColor = new Color(0.78f, 0.02f, 0.24f);
    

    /* State */
    
    /// Root element
    private VisualElement m_RootElement;

    private void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        m_RootElement = new VisualElement();
        
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/InGame/Character/Enemy/Editor/EnemyWaveEditor.uss");
        m_RootElement.styleSheets.Add(styleSheet);
    }

    public override VisualElement CreateInspectorGUI()
    {
        Editor editor = CreateEditor(target);
        IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        m_RootElement.Add(inspectorIMGUI);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/InGame/Character/Enemy/Editor/EnemyWaveEditor.uxml");
        visualTree.CloneTree(m_RootElement);

        return m_RootElement;
    }
    
    // Adapted from Handles.CircleHandleCap
    // Draws a circle with a cross inside so we can see the target position precisely
    public static void CrossedCircleHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType)
    {
        switch (eventType)
        {
            case EventType.MouseMove:
            case EventType.Layout:
                HandleUtility.AddControl(controlID, HandleUtility.DistanceToRectangle(position, rotation, size));
                break;
            case EventType.Repaint:
                // Reflection code for Handles.StartCapDraw(position, rotation, size);
                var handlesEntries = Type.GetType("UnityEditor.Handles,UnityEditor.dll");
                if (handlesEntries != null)
                {
                    var startCapDrawMethod = handlesEntries.GetMethod("StartCapDraw", BindingFlags.Static | BindingFlags.NonPublic);
                    if (startCapDrawMethod != null)
                    {
                        startCapDrawMethod.Invoke(null, new object[]{position, rotation, size});
                        
                        // End of reflection, do the rest of what Handles.CircleHandleCap does normally
                        Vector3 normal = rotation * new Vector3(0.0f, 0.0f, 1f);
                        Handles.DrawWireDisc(position, normal, size);
                        
                        // Add custom code here to draw the cross inside the circle
                        Handles.DrawLine(position + size * Vector3.left, position + size * Vector3.right);
                        Handles.DrawLine(position + size * Vector3.up, position + size * Vector3.down);
                    }
                }
                break;
        }
    }
    
    private void OnSceneGUI()
    {
        var script = (EnemyWave) target;

        // we're modifying indirect (deep) members of the script, and besides applying custom rounding,
        // so changes are not trivial and need Undo Record Object
        Undo.RecordObject(script, "Changed Enemy Wave Data");

        foreach (EnemySpawnData enemySpawnData in script.EnemySpawnDataArray)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                HandlesUtil.DrawFreeMoveHandle(ref enemySpawnData.spawnPosition, spawnPointColor, manualSnapValue * Vector2.one, CrossedCircleHandleCap, 2f);

                if (check.changed)
                {
                    Vector3 roundedPosition = enemySpawnData.spawnPosition;
                    roundedPosition.x = Round(roundedPosition.x);
                    roundedPosition.y = Round(roundedPosition.y);
                    roundedPosition.z = Round(roundedPosition.z);
                    enemySpawnData.spawnPosition = roundedPosition;
                }
                
                // Scale offset with handle size so it remains constant on screen
                float handleSize = HandleUtility.GetHandleSize(enemySpawnData.spawnPosition);
                HandlesUtil.Label2D(new Vector3(enemySpawnData.spawnPosition.x - 0.3225f * handleSize, enemySpawnData.spawnPosition.y - 0.25f * handleSize, 0f), enemySpawnData.enemyName, 2f, true, spawnPointColor);
            }
        }
    }
    
    // Borrowed from AutoSnap.cs
    private float Round(float input)
    {
        return autoSnapValue * Mathf.Round(input / autoSnapValue);
    }
}
