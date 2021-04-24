using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using CommonsHelper;

[CustomEditor(typeof(EnemyWave))]
public class EnemyWaveEditor : Editor
{
    private VisualElement m_RootElement;
    
    public override VisualElement CreateInspectorGUI()
    {
        // Each editor window contains a root VisualElement object
        m_RootElement = new VisualElement();
        
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/InGame/Character/Enemy/Editor/EnemyWaveEditor.uss");
        m_RootElement.styleSheets.Add(styleSheet);
        
        Editor editor = CreateEditor(target);
        IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
        m_RootElement.Add(inspectorIMGUI);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/InGame/Character/Enemy/Editor/EnemyWaveEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        m_RootElement.Add(labelFromUXML);

        return m_RootElement;
    }
    
    private void OnSceneGUI()
    {
        var script = (EnemyWave) target;

        // we're modifying indirect members of the script, so changes are not trivial and need Undo Record
        Undo.RecordObject(script, "Changed Enemy Wave Data");

        foreach (EnemySpawnData enemySpawnData in script.EnemySpawnDataArray)
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Handles.Label(enemySpawnData.spawnPosition.ToVector3(0f), enemySpawnData.enemyName);
                HandlesUtil.DrawFreeMoveHandle(ref enemySpawnData.spawnPosition, Color.magenta, Vector2.one / 16f, Handles.CircleHandleCap);

                if (check.changed)
                {
                    // add any post-change processing here
                }
            }
        }
    }
}
