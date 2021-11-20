using System.Collections;
using System.Collections.Generic;
using CommonsDebug;
using CommonsHelper;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RunActionSequence))]
public class RunActionSequenceEditor : BehaviourActionEditor
{
    /* Constants */

    /// Offset added on X to evert label to approximately center the label below the action start position
    private static float LABEL_OFFSET_X = 100f;
    
    /// Offset added on Y to evert label to put it below the action start position and avoid hiding handles there
    private static float LABEL_OFFSET_Y = 20f;
    
    /// Offset added on Y for every new label placed on an action at the same start position,
    /// to keep the labels distinct and readable
    private static float LABEL_STACK_OFFSET_Y = 25f;
    
    
    private void OnSceneGUI()
    {
        DrawLocalHandles();
    }

    public override void DrawHandles(Vector2 startPosition)
    {
        var script = (RunActionSequence) target;
        Vector2 currentPosition = startPosition;

        float pixelSize = HandlesUtil.Get2DPixelSize();

        // Track the number of actions that don't move the character since the last move,
        // so we know we must show the labels with some offset to make them readable
        int labelStackCount = 0;
        
        foreach (Transform child in script.transform)
        {
            // Deactivated action objects are ignored at runtime, so ignore them in the editor too
            if (!child.gameObject.activeSelf)
            {
                continue;
            }
            
            float labelStackOffset = labelStackCount * LABEL_STACK_OFFSET_Y * pixelSize;
            
            Vector2 labelRectPosition = currentPosition + new Vector2(- LABEL_OFFSET_X * pixelSize,
                - LABEL_OFFSET_Y * pixelSize - labelStackOffset );
            
            string labelText;
            Color textColor;
            
            var behaviourAction = child.GetComponent<BehaviourAction>();
            if (behaviourAction != null)
            {
                // Get Custom Editor for this action if any.
                // If no Custom Editor has been defined, GenericInspector is returned, which will cast to null.
                var actionEditor = CreateEditor(behaviourAction) as BehaviourActionEditor;
                if (actionEditor != null)
                {
                    labelText = behaviourAction.ToString();
                    textColor = Color.white;

                    // Handles specific to this action
                    actionEditor.DrawHandles(currentPosition);
                    Vector2 nextPosition = actionEditor.ComputeEndPosition(currentPosition);
                
                    if (nextPosition == currentPosition)
                    {
                        // Next action will be at same position, remember stack count to show next label more below
                        labelStackCount++;
                    }
                    else
                    {
                        // Reset count, as after one move, we can start showing label at the base position again
                        labelStackCount = 0;
                    }

                    currentPosition = nextPosition;
                }
                else
                {
                    // Missing custom editor
                    labelText = $"{behaviourAction} - No Custom Editor";
                    textColor = ColorUtil.orange;
                    
                    // Next action will be at same position, remember stack count to show next label more below
                    labelStackCount++;
                }
            }
            else
            {
                // Missing behaviour action component
                labelText = $"{child.name} - No Behaviour Action";
                textColor = Color.red;

                // Next action will be at same position, remember stack count to show next label more below
                labelStackCount++;
            }
            
            // Whether valid or invalid, print the label
            HandlesUtil.DrawLabelWithBackground(labelRectPosition, labelText, 1f, true, textColor);
        }
    }
}
