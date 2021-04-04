using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

public class DragonPoolManager : PoolManager<CharacterMaster, DragonPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = Locator.FindWithTag(Tags.CharacterPool)?.transform;
        }

        base.Init();
    }

    /// Spawn character at position
    public CharacterMaster SpawnCharacter(Vector2 position)
    {
        CharacterMaster character = GetObject();
        
        if (character != null)
        {
            character.Spawn(position);
            return character;
        }
        
        // pool starvation (error is already logged inside GetObject)
        return null;
    }
}
