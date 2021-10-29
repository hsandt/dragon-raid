using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CommonsHelper;

public class ActionSequence : MonoBehaviour, IEnumerable<BehaviourAction>
{
    /* Cached child references */
    
    /// List of behaviour actions on children
    private List<BehaviourAction> m_BehaviourActions;

    /// Convenience accessor for actions count
    public int Count => m_BehaviourActions.Count;

    /// Convenience action accessor
    public BehaviourAction this[int index] => m_BehaviourActions[index];
    

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

    private void Awake()
    {
        // Linq statement to iterate on all children, get BehaviourAction component and generate a list
        m_BehaviourActions = transform.Cast<Transform>().Select(tr => tr.GetComponentOrFail<BehaviourAction>()).ToList();
    }

    /* IEnumerable<BehaviourAction> interface */
    
    public IEnumerator<BehaviourAction> GetEnumerator()
    {
        return m_BehaviourActions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
