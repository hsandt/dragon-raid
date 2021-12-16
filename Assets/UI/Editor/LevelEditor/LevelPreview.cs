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
    
    
    /* Cached scene references */
    
    /// Cached camera start position reference
    private Transform m_CameraStartTransform;
    
    /// Cached level data, retrieved from the level identifier of the current scene
    private LevelData m_LevelData;

    /// Cached enemy wave parent
    private GameObject m_EnemyWavesParent;

    
    /* Queried elements */
    
    /// Area showing the level preview
    private VisualElement m_LevelPreviewArea;
    
    /// Rectangle representing the camera preview
    /// Can be dragged to move the scene view across the level quickly
    private VisualElement m_LevelPreviewRectangle;
    
    /// Area showing enemy waves in the current level preview rectangle
    private VisualElement m_EnemyWavePreviewArea;
    
    
    /* State */

    /// Level spatial progress corresponding to the back (generally left) edge
    /// of the preview rectangle
    private float m_PreviewProgress;
    private Vector2 m_PreviewCameraPosition;
    
    
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

        m_EnemyWavesParent = GameObject.FindWithTag(Tags.EnemyWaves);
        if (m_EnemyWavesParent == null)
        {
            Debug.LogError("[LevelEditor] Could not find Game Object tagged EnemyWaves");
        }
    }
    public void RegisterExternalCallbacks()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
        SceneView.duringSceneGui += OnDuringSceneGui;
    }
    public void UnregisterExternalCallbacks()
    {
        EditorSceneManager.sceneOpened -= OnSceneOpened;
        SceneView.duringSceneGui -= OnDuringSceneGui;
    }

    private void RegisterInternalCallbacks()
    {
        // Callback system and implementation based on UI Toolkit Samples: PointerEventsWindow.cs
        m_LevelPreviewArea.RegisterCallback<PointerDownEvent>(OnPreviewAreaPointerDown);
        m_LevelPreviewArea.RegisterCallback<PointerUpEvent>(OnPreviewAreaPointerUp);
        m_LevelPreviewArea.RegisterCallback<PointerMoveEvent>(OnPreviewAreaPointerMove);
    }
    
    private void Setup()
    {
        m_PreviewProgress = 0f;
    }
    
    private void GenerateWaveButtons()
    {
        m_EnemyWavePreviewArea.Clear();
        
        if (m_EnemyWavesParent == null )
        {
            CacheSceneReferences();
            
            if (m_EnemyWavesParent == null )
            {
                return;
            }
        }

        EnemyWave[] allEnemyWaves = m_EnemyWavesParent.GetComponentsInChildren<EnemyWave>();

        for (int i = 0; i < allEnemyWaves.Length; i++)
        {
            EnemyWave enemyWave = allEnemyWaves[i];
            AddEnemyWaveButton(i, enemyWave.name);
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
    
    private void OnPreviewAreaPointerDown(PointerDownEvent evt)
    {
        // Capture pointer (in preview area, not preview rectangle, to allow
        // clicking anywhere to warp the preview rectangle, and to avoid motion jitter)
        m_LevelPreviewArea.CapturePointer(evt.pointerId);
        
        // Highlight preview rectangle
        m_LevelPreviewRectangle.AddToClassList("preview-rectangle--dragged");

        // Warp preview rectangle to pointer
        UpdatePreviewRectanglePosition(evt.localPosition);
    }
    
    private void OnPreviewAreaPointerUp(PointerUpEvent evt)
    {
        // Release pointer
        m_LevelPreviewArea.ReleasePointer(evt.pointerId);
        
        // Stop highlighting preview rectangle
        m_LevelPreviewRectangle.RemoveFromClassList("preview-rectangle--dragged");
        
        // Warp preview rectangle to pointer one last time
        UpdatePreviewRectanglePosition(evt.localPosition);
    }
    
    private void OnPreviewAreaPointerMove(PointerMoveEvent evt)
    {
        // Check that we have started the drag from inside the preview area
        if (m_LevelPreviewArea.panel.GetCapturingElement(evt.pointerId) == evt.target)
        {
            // Move preview rectangle along with pointer
            UpdatePreviewRectanglePosition(evt.localPosition);
        }
    }

    private void UpdatePreviewRectanglePosition(Vector2 localPosition)
    {
        // Center preview rectangle around pointer by subtracting half-width
        // Clamp to limits of containing area (PreviewArea)
        float maxPreviewRectangleX = m_LevelPreviewArea.contentRect.width - m_LevelPreviewRectangle.contentRect.width;
        float previewRectangleX = Mathf.Clamp(localPosition.x - m_LevelPreviewRectangle.contentRect.width / 2,
            0, maxPreviewRectangleX);
        MovePreviewRectangle(previewRectangleX);
        
        // Compute the preview progress ratio (it's close to the level progress ratio, except since preview
        // occupies a window, it reaches 100% one content rect width before the end, see maxPreviewRectangleX)
        float previewProgressRatio = previewRectangleX / maxPreviewRectangleX;
        MoveSceneViewToPreviewProgressRatio(previewProgressRatio);
    }

    private void MovePreviewRectangle(float previewRectangleX)
    {
        m_LevelPreviewRectangle.style.left = previewRectangleX;
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
    
    private EnemyWaveButton AddEnemyWaveButton(float x, string waveName)
    {
        // Create button with class
        EnemyWaveButton enemyWaveButton = new EnemyWaveButton(waveName);

        // Custom styling

        enemyWaveButton.style.left = x;

        // Bind behaviour to select game object on button click
        // actionButton.clickable.clicked += () => { Selection.activeObject = target; };

        m_EnemyWavePreviewArea.Add(enemyWaveButton);
        
        return enemyWaveButton;
    }
}