using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using UnityConstants;
using CommonsHelper;

public class LevelPreview : VisualElement
{
    public new class UxmlFactory : UxmlFactory<LevelPreview> {}
    
    
    /* Constants */

    private const float SCROLL_ZOOM_FACTOR = 1f; 
    
    
    /* Cached scene references */
    
    /// Cached camera start position reference
    private Transform m_CameraStartTransform;
    
    /// Cached level data, retrieved from the level identifier of the current scene
    private LevelData m_LevelData;

    /// Cached spatial events parent
    private GameObject m_SpatialEventsParent;

    
    /* Queried elements */
    
    /// Area showing the level preview
    private VisualElement m_LevelPreviewArea;
    
    /// Rectangle representing the camera preview
    /// Can be dragged to move the scene view across the level quickly
    private VisualElement m_LevelPreviewRectangle;

    /// Scroller used to scroll the level preview area across the whole the whole level
    private Scroller m_Scroller;
    
    /// Area showing enemy waves in the current level preview rectangle
    private VisualElement m_EnemyWavePreviewArea;
    
    
    /* State */

    /// Level spatial progress corresponding to the back (generally left) edge
    /// of the preview rectangle
    private float m_PreviewRectangleCenter;
    private float m_PreviewProgress;
    private float m_PreviewSpanLeftProgress;
    private float m_PreviewSpanRightProgress;
    private Vector2 m_PreviewCameraPosition;

    /// Current world-to-window pixel resolution
    private float m_WorldToWindowPixelResolution = 10f; 
    
    
    /// Parameterless constructor with minimal static asset loading to allow UxmlFactory preview in UI Toolkit Builder
    public LevelPreview()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/LevelEditor/LevelPreview.uxml");
        visualTree.CloneTree(this);
    }

    /// Initialisation method to call from LevelEditor window only to avoid errors in UI Toolkit Builder
    public void Init()
    {
        // Query existing elements
        m_LevelPreviewArea = this.Q<VisualElement>("LevelPreviewArea");
        Debug.AssertFormat(m_LevelPreviewArea != null, "[LevelEditor] No VisualElement 'LevelPreviewArea' found on Level Editor UXML");

        m_LevelPreviewRectangle = this.Q<VisualElement>("LevelPreviewRectangle");
        Debug.AssertFormat(m_LevelPreviewRectangle != null, "[LevelEditor] No VisualElement 'LevelPreviewRectangle' found on Level Editor UXML");

        m_Scroller = this.Q<Scroller>("Scroller");
        Debug.AssertFormat(m_Scroller != null, "[LevelEditor] No VisualElement 'Scroller' found on Level Editor UXML");

        m_EnemyWavePreviewArea = this.Q<VisualElement>("EnemyWavePreviewArea");
        Debug.AssertFormat(m_EnemyWavePreviewArea != null, "[LevelEditor] No VisualElement 'EnemyWavePreviewArea' found on Level Editor UXML");

        CacheSceneReferences();
        GenerateWaveButtons();

        RegisterExternalCallbacks();
        RegisterInternalCallbacks();
        
        Setup();
    }
    
    public void OnDestroy()
    {
        UnregisterExternalCallbacks();
    }

    private void CacheSceneReferences()
    {
        // We cache scene references on window open, but also on scene change.
        // In addition, if a tagged object found previously has been destroyed, we will search for a tagged object
        // one last time and return early if we still find nothing. So we are safe against missing object null references.
        // However, if you retag objects after opening this window, this will not be detected, and our references may
        // be outdated (e.g. using the wrong transform). This should be a rare case, so just reopen the window,
        // or make sure that the previously tagged object was destroyed, to force cache reference refresh.
        
        m_CameraStartTransform = GameObject.FindWithTag(Tags.CameraStartPosition)?.transform;
        if (m_CameraStartTransform == null)
        {
            Debug.LogError("[LevelEditor] Could not find Game Object tagged CameraStartPosition");
        }
        
        m_LevelData = GameObject.FindWithTag(Tags.LevelIdentifier)?.GetComponent<LevelIdentifier>()?.levelData;
        if (m_LevelData == null)
        {
            Debug.LogError("[LevelEditor] Could not find Game Object tagged LevelIdentifier, " +
                           "or LevelIdentifier component with Level Data is missing on it");
        }

        m_SpatialEventsParent = GameObject.FindWithTag(Tags.SpatialEvents);
        if (m_SpatialEventsParent == null)
        {
            Debug.LogError("[LevelEditor] Could not find Game Object tagged SpatialEvents");
        }
    }
    private void RegisterExternalCallbacks()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        SceneView.duringSceneGui += OnDuringSceneGui;
    }
    private void UnregisterExternalCallbacks()
    {
        EditorSceneManager.sceneOpened -= OnSceneOpened;
        SceneView.duringSceneGui -= OnDuringSceneGui;
    }

    private void RegisterInternalCallbacks()
    {
        // Visual Elements are not ready on construction and still have NaN coordinates, so we must setup
        // things based on such coordinates after document geometry was constructed.
        // https://forum.unity.com/threads/visualelement-layout-content-rects-contain-nan-values-initially-transform-scale-breaks-children.677314/
        // In addition, we must update the preview elements after window resize.
        // So register a callback for geometry change.
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        
        // Callback system and implementation based on UI Toolkit Samples: PointerEventsWindow.cs
        m_LevelPreviewArea.RegisterCallback<PointerDownEvent>(OnPreviewAreaPointerDown);
        m_LevelPreviewArea.RegisterCallback<PointerUpEvent>(OnPreviewAreaPointerUp);
        m_LevelPreviewArea.RegisterCallback<PointerMoveEvent>(OnPreviewAreaPointerMove);
        m_LevelPreviewArea.RegisterCallback<WheelEvent>(OnPreviewAreaWheelEvent);
        
        m_Scroller.valueChanged += OnScrollerValueChanged;
    }

    private void Setup()
    {
        m_PreviewRectangleCenter = 0f;
    }
    
    private void GenerateWaveButtons()
    {
        // Seems too early, there are no children at this point, but *afterward* the sample button appears at the bottom
        m_EnemyWavePreviewArea.Clear();
        
        if (m_SpatialEventsParent == null )
        {
            CacheSceneReferences();
            
            if (m_SpatialEventsParent == null )
            {
                return;
            }
        }

        EnemyWave[] allEnemyWaves = m_SpatialEventsParent.GetComponentsInChildren<EnemyWave>();
        var allSpatialEventTriggers = m_SpatialEventsParent.GetComponentsInChildren<EventTrigger_SpatialProgress>();

        for (int i = 0; i < allSpatialEventTriggers.Length; i++)
        {
            EventTrigger_SpatialProgress spatialEventTrigger = allSpatialEventTriggers[i];
            
            // For now, only care about Enemy Wave
            var enemyWave = spatialEventTrigger.GetComponent<EnemyWave>();
            if (enemyWave != null)
            {
                AddEnemyWaveButton(i, spatialEventTrigger, enemyWave);
            }
        }
    }

    private void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        CacheSceneReferences();
    }

    private void OnDuringSceneGui(SceneView sceneView)
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            // Get camera view dimensions
            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = camera.aspect * cameraHalfHeight;
            Vector2 cameraHalfExtent = new Vector2(cameraHalfWidth, cameraHalfHeight);
            
            // Draw rectangle representing camera preview
            Rect rect = new Rect(m_PreviewCameraPosition - cameraHalfExtent, 2f * cameraHalfExtent);
            HandlesUtil.DrawRect(rect, Color.cyan);
            
            // Draw "Preview" in the top-left corner
            Vector2 offset = new Vector2(0.1f, -0.1f);
            HandlesUtil.Label2D(new Vector2(rect.xMin, rect.yMax) + offset, "Preview",
                2f,  true, Color.white);
        }
    }
    
    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        RefreshScroller();
        RefreshPreviewRectangle();
    }

    private void RefreshScroller()
    {
        Camera camera = Camera.main;
        if (camera != null)
        {
            // Get camera view dimensions
            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = camera.aspect * cameraHalfHeight;
            float cameraWidth = 2f * cameraHalfWidth;
            Debug.LogFormat("low {0}, high {1}", m_Scroller.lowValue, m_Scroller.highValue);
            m_Scroller.lowValue = 0f;
            // We can scroll along the whole camera + level distance, minus one area width 
            float visibleWidth = m_LevelPreviewArea.contentRect.width;
            float fullWidth = m_WorldToWindowPixelResolution * (cameraWidth + m_LevelData.maxScrollingProgress);
            m_Scroller.highValue = Mathf.Max(0f, fullWidth - visibleWidth);
            m_Scroller.Adjust(visibleWidth / fullWidth);
            
            // Due to bug, Slider inverted set to true on document save, as if Vertical,
            // so we must reset it to false manually (as scroller is Horizontal)
            m_Scroller.slider.inverted = false;
        }
    }
    
    private void RefreshPreviewRectangle()
    {
        // Adjust preview rectangle width to match camera view
        Camera camera = Camera.main;
        if (camera != null)
        {
            // Get camera view dimensions
            float cameraHalfHeight = camera.orthographicSize;
            float cameraHalfWidth = camera.aspect * cameraHalfHeight;
            float cameraWidth = 2f * cameraHalfWidth;

            // Compute real world unit to window pixels scaling factor
            // Take camera width itself into account, on top of scrolling
            // It's equivalent to taking previewAreaWidth - preview rect width (but not known at this point)
            // and just dividing by m_LevelData.maxScrollingProgress
            // float previewAreaWidth = m_LevelPreviewArea.contentRect.width;
            // float spatialProgressToWindowWidth = previewAreaWidth / (cameraWidth + m_LevelData.maxScrollingProgress);
            float spatialProgressToWindowWidth = m_WorldToWindowPixelResolution;

            float previewRectangleWidth = cameraWidth * spatialProgressToWindowWidth;
            m_LevelPreviewRectangle.style.width = previewRectangleWidth;

            // A kind of reverse of RefreshPreviewRectanglePosition:
            // we stabilize camera position and cached progress members, but we recompute preview rectangle position
            // from those members instead
            float previewRectangleLeft = m_PreviewProgress * spatialProgressToWindowWidth;
            // not really used until next change, but cleaner to set now
            m_PreviewRectangleCenter = previewRectangleLeft + previewRectangleWidth / 2f;
            MovePreviewRectangle(previewRectangleLeft);

            // Also adjust button positions
            RefreshAllEnemyWaveButtonPositions();
        }
    }

    private void OnPreviewAreaPointerDown(PointerDownEvent evt)
    {
        // Capture pointer (in preview area, not preview rectangle, to allow
        // clicking anywhere to warp the preview rectangle, and to avoid motion jitter)
        m_LevelPreviewArea.CapturePointer(evt.pointerId);
        
        // Highlight preview rectangle
        m_LevelPreviewRectangle.AddToClassList("preview-rectangle--dragged");

        // Warp preview rectangle to pointer
        SetPreviewRectanglePosition(evt.localPosition);
    }
    
    private void OnPreviewAreaPointerUp(PointerUpEvent evt)
    {
        // Release pointer
        m_LevelPreviewArea.ReleasePointer(evt.pointerId);
        
        // Stop highlighting preview rectangle
        m_LevelPreviewRectangle.RemoveFromClassList("preview-rectangle--dragged");
        
        // Warp preview rectangle to pointer one last time
        SetPreviewRectanglePosition(evt.localPosition);
    }
    
    private void OnPreviewAreaPointerMove(PointerMoveEvent evt)
    {
        // Check that we have started the drag from inside the preview area
        if (m_LevelPreviewArea.panel.GetCapturingElement(evt.pointerId) == evt.target)
        {
            // Move preview rectangle along with pointer
            SetPreviewRectanglePosition(evt.localPosition);
        }
    }
    
    private void OnPreviewAreaWheelEvent(WheelEvent evt)
    {
        // Wheel up to zoom in i.e. decrease world to pixel factor
        m_WorldToWindowPixelResolution = Mathf.Clamp(m_WorldToWindowPixelResolution - SCROLL_ZOOM_FACTOR * evt.delta.y, 1f, 100f);
        Debug.LogFormat("wheel: {0}", m_WorldToWindowPixelResolution);
        RefreshScroller();
        RefreshPreviewRectangle();
    }

    private void OnScrollerValueChanged(float value)
    {
        Debug.LogFormat("scroll {0}", value);
        RefreshPreviewRectangle();
    }

    private void SetPreviewRectanglePosition(Vector2 localPosition)
    {
        // Add scroller value to take current scroll into account
        m_PreviewRectangleCenter = localPosition.x + m_Scroller.value;
        RefreshPreviewRectanglePosition();
    }

    private void RefreshPreviewRectanglePosition()
    {
        // Center preview rectangle around pointer by subtracting half-width
        // Clamp to limits of containing area (PreviewArea)
        // Note that we use contentRect for the container, but resolvedStyle for the containee which includes the
        // border for full width (e.g. 100 instead of 98, so preview rectangle is perfectly contained within its parent) 
        float previewRectangleLeft = m_PreviewRectangleCenter - m_LevelPreviewRectangle.resolvedStyle.width / 2;
        float maxPreviewRectangleX = m_LevelPreviewArea.contentRect.width - m_LevelPreviewRectangle.resolvedStyle.width;
        float previewRectangleLeftClamped = Mathf.Clamp(previewRectangleLeft, 0, maxPreviewRectangleX);
        MovePreviewRectangle(previewRectangleLeftClamped);

        // Compute the preview progress ratio (it's close to the level progress ratio, except since preview
        // occupies a window, it reaches 100% one content rect width before the end, see maxPreviewRectangleX)
        float previewProgressRatio = previewRectangleLeftClamped / maxPreviewRectangleX;
        MoveSceneViewToPreviewProgressRatio(previewProgressRatio);

        m_PreviewSpanLeftProgress = previewRectangleLeftClamped / m_WorldToWindowPixelResolution;
        float previewRectangleRight = previewRectangleLeftClamped + m_LevelPreviewRectangle.resolvedStyle.width;
        m_PreviewSpanRightProgress = previewRectangleRight / m_WorldToWindowPixelResolution;

        RefreshAllEnemyWaveButtonPositions();
    }

    private void MovePreviewRectangle(float previewRectangleX)
    {
        // Scroller was set to use value scaled with window pixels, ranging from 0 to max right, so just subtract it
        Debug.LogFormat("scroll: {0}", m_Scroller.value);
        m_LevelPreviewRectangle.style.left = previewRectangleX - m_Scroller.value;
    }

    private void MoveSceneViewToPreviewProgressRatio(float previewProgressRatio)
    {
        if (m_CameraStartTransform == null || m_LevelData == null)
        {
            CacheSceneReferences();
            
            if (m_CameraStartTransform == null || m_LevelData == null)
            {
                return;
            }
        }

        // Compute preview progress from ratio
        m_PreviewProgress = previewProgressRatio * m_LevelData.maxScrollingProgress;
        
        // Only support scrolling to the right for now
        m_PreviewCameraPosition = (Vector2) m_CameraStartTransform.position + m_PreviewProgress * Vector2.right;
        SceneView.lastActiveSceneView.pivot = m_PreviewCameraPosition;
    }
    
    
    /* Enemy Wave Editor */
    
    private EnemyWaveButton AddEnemyWaveButton(int index, EventTrigger_SpatialProgress spatialEventTrigger, EnemyWave enemyWave)
    {
        // Create button with class
        EnemyWaveButton enemyWaveButton = new EnemyWaveButton();
        enemyWaveButton.Init(spatialEventTrigger, enemyWave);
        m_EnemyWavePreviewArea.Add(enemyWaveButton);
        
        return enemyWaveButton;
    }

    private void RefreshEnemyWaveButtonPosition(EnemyWaveButton enemyWaveButton)
    {
        // Custom styling
        // Map RequiredSpatialProgress to style x
        // m_PreviewSpanLeftProgress => 0
        // m_PreviewSpanRightProgress => m_EnemyWavePreviewArea.contentRect.width
        float ratio = (enemyWaveButton.SpatialEventRequiredSpatialProgress - m_PreviewSpanLeftProgress) /
                      (m_PreviewSpanRightProgress - m_PreviewSpanLeftProgress);
        float x = Mathf.LerpUnclamped(0f, m_EnemyWavePreviewArea.contentRect.width, ratio);
        enemyWaveButton.style.left = x;
    }
    
    private void RefreshAllEnemyWaveButtonPositions()
    {
        foreach (VisualElement child in m_EnemyWavePreviewArea.Children())
        {
            if (child is EnemyWaveButton enemyWaveButton)
            {
                RefreshEnemyWaveButtonPosition(enemyWaveButton);
            }
        }
    }
}