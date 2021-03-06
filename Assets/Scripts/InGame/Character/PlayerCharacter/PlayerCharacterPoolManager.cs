﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Pool manager for Player Character (Dragon)
/// The only reason to use a pool despite having a single character of this type,
/// is to have an API ready to spawn and despawn without ever destroying the object
public class PlayerCharacterPoolManager : PoolManager<PlayerCharacterMaster, PlayerCharacterPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = LocatorManager.Instance.FindWithTag(Tags.CharacterPool)?.transform;
        }

        base.Init();
    }
    

    /// Spawn character at position
    public PlayerCharacterMaster SpawnCharacter(Vector2 position)
    {
        PlayerCharacterMaster playerCharacter = GetObject();
        
        if (playerCharacter != null)
        {
            playerCharacter.Spawn(position);
            return playerCharacter;
        }
        
        // pool starvation (error is already logged inside GetObject)
        return null;
    }
}
