using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class BehaviourTreeEditor : EditorWindow
{
    /* Queried elements */
    
    /// Label showing current behaviour tree name
    private Label m_BehaviourTreeName;
    
    /// Behaviour Tree View for the current selection
    private BehaviourTreeView m_BehaviourTreeView;
    
    
    [MenuItem("Window/Game/Behaviour Tree Editor")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("Behaviour Tree Editor");
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);
        
        // Query existing elements
        m_BehaviourTreeName = root.Q<Label>();
        Debug.AssertFormat(m_BehaviourTreeName != null, "[BehaviourTreeEditor] No Label (BehaviourTreeName) found on Behaviour Tree Editor UXML");
        m_BehaviourTreeView = root.Q<BehaviourTreeView>();
        Debug.AssertFormat(m_BehaviourTreeView != null, "[BehaviourTreeEditor] No BehaviourTreeView found on Behaviour Tree Editor UXML");
    }

    private void OnSelectionChange()
    {
        var selectedTransform = Selection.activeTransform;
        if (selectedTransform != null)
        {
            Transform currentTransform = selectedTransform;
            BehaviourAction rootAction = null;

            // Search component of base type BehaviourAction iteratively among parents
            // and return the one we found at the highest level (without interruption, i.e.
            // each intermediate parent must have a BehaviourAction), known as Root Action
            while (true)
            {
                var currentAction = currentTransform.GetComponent<BehaviourAction>();
                if (currentAction == null)
                {
                    // No BehaviourAction on this game object, stop searching
                    // (with or without having found at least one behaviour action)
                    break;
                }

                // Iterate to next parent
                currentTransform = currentTransform.transform.parent;
                rootAction = currentAction;
            }

            if (rootAction != null)
            {
                // We found a root action, display it on the BehaviourTreeView
                m_BehaviourTreeName.text = rootAction.gameObject.name;
                m_BehaviourTreeView?.PopulateView(rootAction);
            }
            else
            {
                m_BehaviourTreeName.text = "No Behaviour Action selected";
                m_BehaviourTreeView?.Clear();
            }
        }
    }
}