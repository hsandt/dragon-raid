using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for a character
public class CharacterMaster : MasterBehaviour, IPooledObject
{
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;
    
    private void Awake()
    {
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();

        AddSiblingSlaveBehaviours();
    }

    protected virtual void AddSiblingSlaveBehaviours()
    {
        // All the slave behaviours are ClearableBehaviour, so for easy extensibility
        // just register them all. Still keep this method virtual in case Player/Enemy character
        // must also restart specific, non-clearable behaviours (i.e. Unity native behaviour components)
        var clearableBehaviours = GetComponents<ClearableBehaviour>();
        foreach (var clearableBehaviour in clearableBehaviours)
        {
            // to avoid infinite recursion on Setup/Clear, do not register te Master script itself as its own Slave!
            if (clearableBehaviour != this)
            {
                slaveBehaviours.Add(clearableBehaviour);
            }
        }
    }
    
    private void Start()
    {
        Setup();
    }
    
    
    /* IPooledObject interface */
    
    public void InitPooled()
    {
    }

    public bool IsInUse()
    {
        return gameObject.activeSelf;
    }

    public void Release()
    {
        Clear();
        gameObject.SetActive(false);
    }
    
    
    /* Own methods */

    public void Spawn(Vector2 position)
    {
        gameObject.SetActive(true);
        Setup();

        m_Rigidbody2D.position = position;
    }
}
