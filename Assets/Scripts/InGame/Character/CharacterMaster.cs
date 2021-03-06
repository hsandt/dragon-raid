﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for a character
public class CharacterMaster : MasterBehaviour, IPooledObject
{
    /* Sibling components */
    
    private Rigidbody2D m_Rigidbody2D;

    
    protected override void Init()
    {
        base.Init();
        
        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
    }

    // Do not define a Start method to call Setup, as Setup is managed
    // Instead, each Character PoolManager will Spawn an instance of Character,
    // and Spawn will call Setup.
    
    
    /* Own methods */

    public void Spawn(Vector2 position)
    {
        gameObject.SetActive(true);
        Setup();

        m_Rigidbody2D.position = position;
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
}
