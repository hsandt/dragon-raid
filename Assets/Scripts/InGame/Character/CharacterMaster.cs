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
        // slave behaviours common to all characters
        slaveBehaviours.Add(this.GetComponentOrFail<HealthSystem>());
        slaveBehaviours.Add(this.GetComponentOrFail<Move>());
        slaveBehaviours.Add(this.GetComponentOrFail<Shoot>());
        slaveBehaviours.Add(this.GetComponentOrFail<BaseShootController>());
        slaveBehaviours.Add(this.GetComponentOrFail<Brighten>());
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
