using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using CommonsHelper;

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


    /* Cached objects */

    /// Dictionary of cached action editors, with action script instance ID as key
    private readonly Dictionary<int, BehaviourActionEditor> m_CachedEditors = new Dictionary<int, BehaviourActionEditor>();


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
                BehaviourActionEditor actionEditor;

                // Check for any cached editor first
                if (m_CachedEditors.TryGetValue(behaviourAction.GetInstanceID(), out BehaviourActionEditor cachedEditor))
                {
                    // This script had one cached, reuse it to avoid high CPU usage and lag while panning in the
                    // Scene View due to CreateEditor heavy cost (found via Profiler in Edit Mode with Deep Profile).
                    actionEditor = cachedEditor;
                }
                else
                {
                    // No cached editor (not even a null cached editor indicating the absence of custom editor),
                    // so get Custom Editor for this action (if any).
                    // If no Custom Editor has been defined, GenericInspector is returned, which will cast to null.
                    // If so, we must *still* add null to the cached dictionary so we know that there is no custom
                    // editor and we don't try to indefinitely get one further times.
                    // Note that the only reason we must CreateEditor is that we decided to put action-specific
                    // methods to override DrawHandles and ComputeEndPosition on the action Editor class for better
                    // code separation. In fact, we could totally move those methods to the runtime class,
                    // between #if UNITY_EDITOR preprocessing (because HandlesUtil is in CommonsHelper and can be
                    // referenced by runtime scripts; it was moved there precisely for this reason). This would also
                    // remove the issue of high CPU usage by CreateEditor.
                    // But since caching is enough to solve the CPU issue, we decided to keep the handles code in the
                    // Editor classes for clarity.
                    actionEditor = CreateEditor(behaviourAction) as BehaviourActionEditor;
                    m_CachedEditors.Add(behaviourAction.GetInstanceID(), actionEditor);
                }

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

    private void OnDisable()
    {
        // Destroy all cached editors explicitly, as recommended by the doc of CreateEditor
        // This doesn't seem to fix the memory increase caused by repeatedly selecting and deselecting
        // a RunActionSequence. But in normal usage, this should be OK as editor caching is enough to
        // fix the CPU lag due to CreateEditor while avoiding to create many of them.
        foreach (var cachedEditor in m_CachedEditors.Values)
        {
            DestroyImmediate(cachedEditor);
        }
    }
}
