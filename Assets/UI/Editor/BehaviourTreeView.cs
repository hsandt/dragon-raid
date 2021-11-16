using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView : VisualElement
{
    private float INDENT_FACTOR = 40;

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView> {}

    public void PopulateView(BehaviourAction rootAction)
    {
        Clear();
        AddBehaviourActionButton(rootAction, 0);
    }

    private void AddBehaviourActionButton(BehaviourAction behaviourAction, int indentLevel)
    {
        // Create button representing the action
        Button actionButton = new Button();
        actionButton.AddToClassList("behaviour-action-button");
        
        // Styling
        
        // Ex: "MoveLeft (Action_MoveFlyingBy)"
        actionButton.text = $"{behaviourAction.name} ({behaviourAction.GetType()})";
        
        // Compute margin left from indent factor and indent level
        Length marginLeft = new Length(INDENT_FACTOR * indentLevel, LengthUnit.Pixel);
        actionButton.style.marginLeft = marginLeft;
        
        // Bind behaviour to select game object on button click
        actionButton.clickable.clicked += () => { Selection.activeObject = behaviourAction; };

        Add(actionButton);
        
        foreach (Transform child in behaviourAction.transform)
        {
            var childBehaviourAction = child.GetComponent<BehaviourAction>();
            if (childBehaviourAction != null)
            {
                AddBehaviourActionButton(childBehaviourAction, indentLevel + 1);
            }
            else
            {
                Debug.LogWarningFormat(child, "child {0} has no BehaviourAction component", child);
            }
        }
    }
}
