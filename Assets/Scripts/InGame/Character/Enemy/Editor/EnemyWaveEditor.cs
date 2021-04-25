using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using CommonsHelper;

[CustomEditor(typeof(EnemyWave))]
public class EnemyWaveEditor : Editor
{
    /// Color used for spawn point debug and enemy label
    private readonly Color spawnPointColor = new Color(0.78f, 0.02f, 0.24f);
    
    /// Style used for all Handles labels
    private static readonly GUIStyle labelStyle = new GUIStyle();

    /// Root element
    private VisualElement m_RootElement;

    private void OnEnable()
    {
        labelStyle.fontSize = 20;
        labelStyle.normal.textColor = spawnPointColor;
        
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
                StartCapDraw(position, rotation, size);
                Vector3 normal = rotation * new Vector3(0.0f, 0.0f, 1f);
                Handles.DrawWireDisc(position, normal, size);
                Handles.DrawLine(position + size * Vector3.left, position + size * Vector3.right);
                Handles.DrawLine(position + size * Vector3.up, position + size * Vector3.down);
                break;
        }
    }
    
    // hack to access Unity internals by copy-pasting them... will not update with Unity updates, so maybe prefer Reflection
    internal static Matrix4x4 StartCapDraw(Vector3 position, Quaternion rotation, float size)
    {
        Shader.SetGlobalColor("_HandleColor", realHandleColor);
        Shader.SetGlobalFloat("_HandleSize", size);
        Matrix4x4 matrix4x4 = Handles.matrix * Matrix4x4.TRS(position, rotation, Vector3.one);
        Shader.SetGlobalMatrix("_ObjectToWorld", matrix4x4);
        HandleUtility.handleMaterial.SetFloat("_HandleZTest", (float) Handles.zTest);
        HandleUtility.handleMaterial.SetPass(0);
        return matrix4x4;
    }
    
    internal static Color realHandleColor
    {
        get
        {
            return Handles.color * new Color(1f, 1f, 1f, 0.5f) + (Handles.lighting ? new Color(0.0f, 0.0f, 0.0f, 0.5f) : new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }
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
                Handles.Label(new Vector3(enemySpawnData.spawnPosition.x - 0.32f, enemySpawnData.spawnPosition.y - 0.26f, 0f), enemySpawnData.enemyName, labelStyle);
                HandlesUtil.DrawFreeMoveHandle(ref enemySpawnData.spawnPosition, spawnPointColor, Vector2.one / 16f, CrossedCircleHandleCap, 2f);

                if (check.changed)
                {
                    // add any post-change processing here
                }
            }
        }
    }
}
