using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class LevelEditor : EditorWindow
{
    /* Queried elements */
    
    /// Area showing the level preview
    private VisualElement m_PreviewArea;
    
    /// Rectangle representing the camera preview
    /// Can be dragged to move the scene view across the level quickly
    private VisualElement m_PreviewRectangle;
    
    
    [MenuItem("Window/Game/Level Editor")]
    public static void OpenWindow()
    {
        LevelEditor wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }

    private void CreateGUI()
    {
        Debug.Log("CreateGUI");
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/LevelEditor.uxml");
        visualTree.CloneTree(root);

        // Query existing elements
        m_PreviewArea = rootVisualElement.Q<VisualElement>("PreviewArea");
        Debug.AssertFormat(m_PreviewArea != null, "[LevelEditor] No VisualElement 'PreviewArea' found on Level Editor UXML");
        m_PreviewRectangle = rootVisualElement.Q<VisualElement>("PreviewRectangle");
        Debug.AssertFormat(m_PreviewRectangle != null, "[LevelEditor] No VisualElement 'PreviewRectangle' found on Level Editor UXML");
        
        RegisterCallbacks();
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void RegisterCallbacks()
    {
        m_PreviewRectangle.RegisterCallback<PointerDownEvent>(OnPointerDown);
        m_PreviewRectangle.RegisterCallback<PointerUpEvent>(OnPointerUp);
        // m_PreviewRectangle.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }
    
    private void OnPointerDown(PointerDownEvent evt)
    {
        Debug.Log("pointer down");
        Debug.LogFormat("pos: {0}", evt.localPosition);
        m_PreviewRectangle.AddToClassList("preview-rectangle--dragged");
        m_PreviewRectangle.CapturePointer(evt.pointerId);
    }
    
    private void OnPointerUp(PointerUpEvent evt)
    {
        Debug.Log("pointer up");
        Debug.LogFormat("pos: {0}", evt.localPosition);
        m_PreviewRectangle.ReleasePointer(evt.pointerId);
        m_PreviewRectangle.RemoveFromClassList("preview-rectangle--dragged");
    }
}