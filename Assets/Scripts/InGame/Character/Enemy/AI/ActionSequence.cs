using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSequence : MonoBehaviour
{
    public BehaviourAction[] behaviourActions;

    public int Length => behaviourActions.Length;

    public BehaviourAction this[int index] => behaviourActions[index];

#if UNITY_EDITOR
    /// Initial position on start. Recorded only to draw the action handles correctly
    /// (e.g. AI trajectory as initially planned)
    private Vector2 debugInitialPosition;
    public Vector2 DebugInitialPosition => debugInitialPosition;

    void Start()
    {
        debugInitialPosition = (Vector2)transform.position;
    }
#endif
}
