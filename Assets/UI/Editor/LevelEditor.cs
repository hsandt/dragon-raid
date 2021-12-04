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
        // Callback system and implementation based on UI Toolkit Samples: PointerEventsWindow.cs
        m_PreviewArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
        m_PreviewArea.RegisterCallback<PointerUpEvent>(OnPointerUp);
        m_PreviewArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }
    
    private void OnPointerDown(PointerDownEvent evt)
    {
        // Capture pointer (in preview area, not preview rectangle, to allow
        // clicking anywhere to warp the preview rectangle, and to avoid motion jitter)
        m_PreviewArea.CapturePointer(evt.pointerId);
        
        // Highlight preview rectangle
        m_PreviewRectangle.AddToClassList("preview-rectangle--dragged");

        // Warp preview rectangle to pointer
        UpdatePreviewRectanglePosition(evt.localPosition);
    }
    
    private void OnPointerUp(PointerUpEvent evt)
    {
        // Release pointer
        m_PreviewArea.ReleasePointer(evt.pointerId);
        
        // Stop highlighting preview rectangle
        m_PreviewRectangle.RemoveFromClassList("preview-rectangle--dragged");
        
        // Warp preview rectangle to pointer one last time
        UpdatePreviewRectanglePosition(evt.localPosition);
    }
    
    private void OnPointerMove(PointerMoveEvent evt)
    {
        // Check that we have started the drag from inside the preview area
        if (m_PreviewArea.panel.GetCapturingElement(evt.pointerId) == evt.target)
        {
            // Move preview rectangle along with pointer
            UpdatePreviewRectanglePosition(evt.localPosition);
        }
    }
    
    private void UpdatePreviewRectanglePosition(Vector2 localPosition)
    {
        // Center preview rectangle around pointer by subtracting half-width
        // Clamp to limits of containing area (PreviewArea)
        m_PreviewRectangle.style.left = Mathf.Clamp(localPosition.x - m_PreviewRectangle.contentRect.width / 2,
            0, m_PreviewArea.contentRect.width - m_PreviewRectangle.contentRect.width);
    }
}