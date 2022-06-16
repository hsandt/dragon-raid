using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using CommonsHelper;

/// Base class for all behaviour action editors
/// Make sure to define a child class editor for each action whose GetNodeName returns a dynamic node name based on
/// action properties, even if the class content is empty (no custom handles, etc.), just so OnInspectorGUI can refresh
/// the node name on property change.
[CustomEditor(typeof(BehaviourAction), editorForChildClasses: true)]
[CanEditMultipleObjects]
public class BehaviourActionEditor : Editor
{
    /* Constants */

    /// Offset added on X to every label to approximately center the label below the action start position
    protected const float LABEL_OFFSET_X = 100f;

    /// Offset added on Y to every label to put it below the action start position and avoid hiding handles there
    protected const float LABEL_OFFSET_Y = 20f;

    /// Offset added on Y for every new label placed on an action at the same start position,
    /// to keep the labels distinct and readable
    protected const float LABEL_STACK_OFFSET_Y = 25f;


    // Note: this will only apply to BehaviourAction that have no custom child editors
    // Each custom child editor (inheriting from this class) must reimplement OnSceneGUI
    // (just copy-paste the block below)
    private void OnSceneGUI()
    {
        DrawLocalHandlesWithLabel();
    }

    public override void OnInspectorGUI()
    {
        // Detect any property change
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();

            if (check.changed)
            {
                // Some property changes, so refresh node name for this action
                // (this is only useful if node name is dynamic and uses the changed property, but to simplify,
                // just refresh anyway)
                BehaviourTreeEditor.RefreshNodeNamesInWindowIfAny((BehaviourAction) target);
            }
        }
    }

    /// Helper method to call in OnSceneGUI of every child class
    /// This only exists because even if we define OnSceneGUI on this base class, it won't be called
    /// on the child classes like Awake or Start
    public void DrawLocalHandlesWithLabel()
    {
        // We know that target should be a BehaviourAction, although in this case we only need it as a Component
        var script = (Component) target;
        Vector2 startPosition = (Vector2) script.transform.position;

        // From here, we try to draw handles and label similarly to RunActionSequenceEditor.DrawHandles,
        // except we don't care about next position and label stacking

        float pixelSize = HandlesUtil.Get2DPixelSize();

        Vector2 labelRectPosition = startPosition + new Vector2(- LABEL_OFFSET_X * pixelSize,
            - LABEL_OFFSET_Y * pixelSize );

        string labelText;
        Color textColor;

        // Check if we can draw handles
        string handlesError = CheckHandlesError();
        if (string.IsNullOrEmpty(handlesError))
        {
            // Handles specific to this action
            DrawHandles(startPosition);

            // Valid configuration, show normal action label
            labelText = script.ToString();
            textColor = Color.white;
        }
        else
        {
            // Handles cannot be drawn (invalid configuration)
            // Show error
            labelText = $"{script.name} - {handlesError}";
            textColor = Color.yellow;
        }

        // Whether valid or invalid, print the label
        HandlesUtil.DrawLabelWithBackground(labelRectPosition, labelText, 1f, true, textColor);
    }

    /// Verifies that behaviour action component configuration is valid, so DrawHandles and ComputeEndPosition
    /// can be called safely
    /// - return null if there is no error
    /// - return an error message else
    /// Most actions don't need a specific configuration, so return null in default implementation
    /// In fact, this method has been added specifically for actions that need parameters as references like Path actions
    public virtual string CheckHandlesError() { return null; }

    /// Draw editor handles for this action when starting from [startPosition]
    /// Must be called in custom Editor OnSceneGUI.
    /// UB unless CheckHandlesError returns null
    public virtual void DrawHandles(Vector2 startPosition) {}

    /// Return the end position of the action when starting from [startPosition]
    /// Used to chain action handles at the correct positions.
    /// UB unless CheckHandlesError returns null
    public virtual Vector2 ComputeEndPosition(Vector2 startPosition)
    {
        // Many actions don't move the character, so return startPosition in default implementation
        // so only actions that move character need to override this method
        return startPosition;
    }
}