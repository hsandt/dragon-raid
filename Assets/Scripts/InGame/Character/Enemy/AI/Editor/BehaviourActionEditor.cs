using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BehaviourActionEditor : Editor
{
    /// Helper method to call in OnSceneGUI of every child class
    /// This only exists because even if we define OnSceneGUI on this base class, it won't be called
    /// on the child classes like Awake or Start
    protected void DrawLocalHandles()
    {
        // We know that target should be a BehaviourAction, although in this case we only need it as a Component
        var script = (Component) target;
        DrawHandles((Vector2) script.transform.position);
    }
    
    /// Draw editor handles for this action when starting from [startPosition]
    /// Must be called in custom Editor OnSceneGUI.
    public abstract void DrawHandles(Vector2 startPosition);

    /// Return the end position of the action when starting from [startPosition]
    /// Used to chain action handles at the correct positions.
    public virtual Vector2 ComputeEndPosition(Vector2 startPosition)
    {
        // Many actions don't move the character, so return startPosition in default implementation 
        // so only actions that move character need to override this method
        return startPosition;
    }
}