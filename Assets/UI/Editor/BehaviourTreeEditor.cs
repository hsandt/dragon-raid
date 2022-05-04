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

    private void OnEnable()
    {
        Undo.undoRedoPerformed += UndoRedoPerformed;
    }

    private void OnDisable()
    {
        Undo.undoRedoPerformed -= UndoRedoPerformed;
    }

    private static void UndoRedoPerformed()
    {
        RefreshNodeNamesInWindowIfAny();
    }

    public static void RefreshNodeNamesInWindowIfAny(BehaviourAction targetAction = null)
    {
        if (HasOpenInstances<BehaviourTreeEditor>())
        {
            BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
            wnd.RefreshNodeNames(targetAction);
        }
    }

    private void RefreshNodeNames(BehaviourAction targetAction = null)
    {
        m_BehaviourTreeView?.RefreshNodeNames(targetAction);
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
            BehaviourTreeRoot behaviourTreeRoot = null;
            BehaviourAction previousAction = null;
            Transform currentTransform = selectedTransform;

            // Search component of base type BehaviourAction iteratively among parents
            // and return the one we found at the highest level (without interruption, i.e.
            // each intermediate parent must have a BehaviourAction), known as Root Action
            // Make sure to stop if we reached the scene top by checking for null parent transform
            while (currentTransform != null)
            {
                behaviourTreeRoot = currentTransform.GetComponent<BehaviourTreeRoot>();
                if (behaviourTreeRoot != null)
                {
                    // We've found the root, DONE
                    break;
                }

                // No root yet, check that at least we are continuously navigating actions upward
                var currentAction = currentTransform.GetComponent<BehaviourAction>();
                if (currentAction == null)
                {
                    // No BehaviourAction on this game object, stop searching

                    if (previousAction != null)
                    {
                        // We found some action(s) previously so it's weird that we got no action nor root
                        Debug.LogWarningFormat(currentTransform, "[BehaviourTreeEditor] OnSelectionChange: action {0} parent {1} has " +
                            "no BehaviourAction nor BehaviourTreeRoot component, which means that there is a " +
                            "\"hole\" in the action tree.",
                            previousAction, currentTransform);
                    }

                    // Otherwise, we are on first iteration and found no action, which means we're simply outside a BT
                    // (or we are just in the middle of a "hole" as described above, but it's cumbersome to check that)
                    break;
                }

                previousAction = currentAction;

                // Iterate to next parent (or null if reached scene top)
                currentTransform = currentTransform.transform.parent;
            }

            if (behaviourTreeRoot != null)
            {
                // We found a root action, display it on the BehaviourTreeView
                m_BehaviourTreeName.text = behaviourTreeRoot.gameObject.name;
                m_BehaviourTreeView?.PopulateView(behaviourTreeRoot);
            }
            else
            {
                m_BehaviourTreeName.text = "No Behaviour Action selected";
                m_BehaviourTreeView?.Clear();
            }
        }
    }
}