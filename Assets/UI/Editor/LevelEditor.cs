using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

using UnityConstants;

public class LevelEditor : EditorWindow
{
    /* Cached scene references */
    
    /// Cached camera start position reference
    private Transform m_CameraStartTransform;

    
    /* Queried elements */
    
    /// Area showing the level preview
    private VisualElement m_PreviewArea;
    
    /// Rectangle representing the camera preview
    /// Can be dragged to move the scene view across the level quickly
    private VisualElement m_PreviewRectangle;
    
    
    /* State */

    /// Level spatial progress corresponding to the back (generally left) edge
    /// of the preview rectangle
    private float m_PreviewProgress;
    
    
    [MenuItem("Window/Game/Level Editor")]
    public static void OpenWindow()
    {
        LevelEditor wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }

    private void CreateGUI()
    {
        CacheSceneReferences();

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

        Setup();
    }

    private void CacheSceneReferences()
    {
        // We cache scene references on window open, but also on scene change.
        // In addition, if a tagged object found previously has been destroyed, we will search for a tagged object
        // one last time and return early if we still find nothing. So we are safe against missing object null references.
        // However, if you retag objects after opening this window, this will not be detected, and our references may
        // be outdated (e.g. using the wrong transform). This should be a rare case, so just reopen the window,
        // or make sure that the previously tagged object was destroyed, to force cache reference refresh.
        
        m_CameraStartTransform = GameObject.FindWithTag(Tags.CameraStartPosition).transform;
        if (m_CameraStartTransform == null)
        {
            Debug.LogError("[LevelEditor] Could not find Game Object tagged CameraStartPosition");
        }
    }

    private void Setup()
    {
        m_PreviewProgress = 0f;
    }

    private void RegisterCallbacks()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        
        // Callback system and implementation based on UI Toolkit Samples: PointerEventsWindow.cs
        m_PreviewArea.RegisterCallback<PointerDownEvent>(OnPointerDown);
        m_PreviewArea.RegisterCallback<PointerUpEvent>(OnPointerUp);
        m_PreviewArea.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }
    
    private void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        CacheSceneReferences();
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
        float maxPreviewRectangleX = m_PreviewArea.contentRect.width - m_PreviewRectangle.contentRect.width;
        float previewRectangleX = Mathf.Clamp(localPosition.x - m_PreviewRectangle.contentRect.width / 2,
            0, maxPreviewRectangleX);
        MovePreviewRectangle(previewRectangleX);
        
        // Compute the preview progress ratio (it's close to the level progress ratio, except since preview
        // occupies a window, it reaches 100% one content rect width before the end, see maxPreviewRectangleX)
        float previewProgressRatio = previewRectangleX / maxPreviewRectangleX;
        MoveSceneViewToProgressRatio(previewProgressRatio);
    }

    private void MovePreviewRectangle(float previewRectangleX)
    {
        m_PreviewRectangle.style.left = previewRectangleX;
    }

    private void MoveSceneViewToProgressRatio(float previewProgressRatio)
    {
        if (m_CameraStartTransform == null)
        {
            CacheSceneReferences();
            
            if (m_CameraStartTransform == null)
            {
                return;
            }
        }
        
        // Estimated level end
        m_PreviewProgress = previewProgressRatio * 100f;
        
        Vector3 newSceneViewPivot = m_CameraStartTransform.position + m_PreviewProgress * Vector3.right;
        SceneView.lastActiveSceneView.pivot = newSceneViewPivot;
    }
}