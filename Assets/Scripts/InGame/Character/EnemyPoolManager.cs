using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Pool manager for all enemy characters
public class EnemyPoolManager : MultiPoolManager<CharacterMaster, EnemyPoolManager>
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
    public CharacterMaster SpawnCharacter(string enemyName, Vector2 position)
    {
        CharacterMaster character = GetObject(enemyName);
        
        if (character != null)
        {
            character.Spawn(position);
            return character;
        }
        
        // pool starvation (error is already logged inside GetObject)
        return null;
    }
}
