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
    private const float autoSnapValue = 1f / 16f;

    /// Position snapping distance when holding Ctrl. Bigger than auto-snap, used for broad placement.
    private const float manualSnapValue = 1f;

    /// Color used for spawn point debug and enemy label
    private readonly Color spawnPointColor = new Color(0.78f, 0.21f, 0.42f);
    

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
                HandlesUtil.DrawFreeMoveHandle(ref enemySpawnData.spawnPosition, spawnPointColor, manualSnapValue * Vector2.one, HandlesUtil.CrossedCircleHandleCap, 2f);

                if (check.changed)
                {
                    Vector3 roundedPosition = enemySpawnData.spawnPosition;
                    roundedPosition.x = Round(roundedPosition.x);
                    roundedPosition.y = Round(roundedPosition.y);
                    roundedPosition.z = Round(roundedPosition.z);
                    enemySpawnData.spawnPosition = roundedPosition;
                }
            }
            
            // Scale offset with handle size so it remains constant on screen
            float handleSize = HandlesUtil.Get2DPixelSize();
            
            string enemyName = enemySpawnData.enemyData ? enemySpawnData.enemyData.enemyName : "NONE";
            HandlesUtil.Label2D(new Vector3(enemySpawnData.spawnPosition.x - 24f * handleSize, enemySpawnData.spawnPosition.y - 32f * handleSize, 0f), enemyName, 2f, true, spawnPointColor);
                
            Texture previewTexture = enemySpawnData.enemyData.editorSpawnPreviewTexture;
            if (previewTexture != null)
            {
                Vector3 position = new Vector3(enemySpawnData.spawnPosition.x - (previewTexture.width / 2f) * handleSize, enemySpawnData.spawnPosition.y - (15f + previewTexture.height / 2f) * handleSize, 0f);
                Handles.Label(position, previewTexture);
            }
        }
    }
    
    // Borrowed from AutoSnap.cs
    private float Round(float input)
    {
        return autoSnapValue * Mathf.Round(input / autoSnapValue);
    }
}
