using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelEditor : EditorWindow
{
    /* Queried elements */

    /// Level preview (main content)
    private LevelPreview m_LevelPreview;


    [MenuItem("Window/Game/Level Editor")]
    public static void OpenWindow()
    {
        LevelEditor wnd = GetWindow<LevelEditor>();
        wnd.titleContent = new GUIContent("Level Editor");
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Import UXML (it's mostly empty)
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Shmup Framework/Scripts/Editor/Editor Windows/LevelEditor/LevelEditor.uxml");
        visualTree.CloneTree(root);

        RegisterExternalCallbacks();

        TryCreateLevelPreview();
    }

    private void OnDestroy()
    {
        m_LevelPreview?.OnDestroy();

        UnregisterExternalCallbacks();
    }

    private void RegisterExternalCallbacks()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private void UnregisterExternalCallbacks()
    {
        EditorSceneManager.sceneOpened -= OnSceneOpened;
    }

    private void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        TryCreateLevelPreview();
    }

    private void TryCreateLevelPreview()
    {
        rootVisualElement.Clear();

        // Only create LevelPreview main widget if a Level scene is open
        if (GameObject.FindWithTag("LevelIdentifier") != null)
        {
            m_LevelPreview = new LevelPreview();
            m_LevelPreview.Init();

            rootVisualElement.Add(m_LevelPreview);
        }
        else
        {
            Label errorLabel = new Label("No Level scene opened");
            rootVisualElement.Add(errorLabel);

            Button retryButton = new Button(TryCreateLevelPreview);
            retryButton.text = "Retry Create Level Preview";
            rootVisualElement.Add(retryButton);
        }
    }
}