﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Pool manager for Player Character (Dragon)
/// The only reason to use a pool despite having a single character of this type,
/// is to have an API ready to spawn and despawn without ever destroying the object
/// SEO: after LocatorManager
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
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat("[PlayerCharacterPoolManager] SpawnCharacter: pool starvation! Cannot spawn player character. " +
            "Consider setting instantiateNewObjectOnStarvation: true on PlayerCharacterPoolManager, or increasing its pool size.");
        #endif
        
        return null;
    }
}
