using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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
}
