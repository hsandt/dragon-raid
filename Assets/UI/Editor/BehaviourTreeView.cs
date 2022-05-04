using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView : VisualElement
{
    private const float INDENT_FACTOR = 40;

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView> {}

    public void PopulateView(BehaviourTreeRoot behaviourTreeRoot)
    {
        Clear();

        BehaviourAction rootAction = behaviourTreeRoot.GetRootAction();
        if (rootAction != null)
        {
            AddBehaviourActionButton(0, rootAction);
        }
        else
        {
            Debug.LogErrorFormat(behaviourTreeRoot, "[BehaviourTreeView] PopulateView: No Root Action found on {0}. " +
                "Expected 1 child with a BehaviourAction component. Further execution will cause null reference exceptions.",
                behaviourTreeRoot);
        }
    }

    /// If targetAction is null, refresh all node names (done for Undo/Redo which doesn't know what changed)
    /// Else, refresh only the node name associated to target action
    public void RefreshNodeNames(BehaviourAction targetAction = null)
    {
        foreach (VisualElement child in Children())
        {
            if (child is Button { userData: BehaviourAction action } button)
            {
                if (targetAction == null || targetAction == action)
                {
                    Debug.LogFormat("Rename {0} to {1}", button.text, action.GetNodeName());
                    button.text = action.GetNodeName();
                }
            }
        }
    }

    private void AddBehaviourActionButton(int indentLevel, BehaviourAction behaviourAction)
    {
        string text = behaviourAction.GetNodeName();
        AddButton(indentLevel, text, behaviourAction);

        foreach (Transform child in behaviourAction.transform)
        {
            var childBehaviourAction = child.GetComponent<BehaviourAction>();
            if (childBehaviourAction != null)
            {
                AddBehaviourActionButton(indentLevel + 1, childBehaviourAction);
            }
            else
            {
                // Missing BehaviourAction component on this game object, but still show button to indicate it to user
                AddInvalidActionButton(indentLevel + 1, child.gameObject);
            }
        }
    }

    private void AddInvalidActionButton(int indentLevel, GameObject child)
    {
        string text = $"{child.name} (Invalid)";
        Button invalidActionButton = AddButton(indentLevel, text, child);
        invalidActionButton.AddToClassList("invalid");
    }

    private Button AddButton(int indentLevel, string text, Object target)
    {
        // Create button with class
        Button actionButton = new Button();
        actionButton.AddToClassList("behaviour-action-button");

        // Custom styling

        // Ex: "MoveLeft (Action_MoveFlyingBy)"
        actionButton.text = text;

        // Compute margin left from indent factor and indent level
        Length marginLeft = new Length(INDENT_FACTOR * indentLevel, LengthUnit.Pixel);
        actionButton.style.marginLeft = marginLeft;

        // Bind target as user data and define behaviour to select game object on button click
        // Note that target may be either a BehaviourAction (for valid button) or GameObject (for invalid button)
        actionButton.userData = target;
        actionButton.clickable.clicked += () => { Selection.activeObject = target; };

        Add(actionButton);

        return actionButton;
    }
}
