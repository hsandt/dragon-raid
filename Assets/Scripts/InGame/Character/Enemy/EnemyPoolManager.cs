using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Pool manager for all enemy characters
public class EnemyPoolManager : MultiPoolManager<EnemyCharacterMaster, EnemyPoolManager>
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
    public EnemyCharacterMaster SpawnCharacter(string enemyName, Vector2 position, EnemyWave enemyWave)
    {
        EnemyCharacterMaster enemyCharacter = GetObject(enemyName);
        
        if (enemyCharacter != null)
        {
            enemyCharacter.Spawn(position);
            enemyCharacter.SetEnemyWave(enemyWave);
            return enemyCharacter;
        }
        
        // pool starvation (error is already logged inside GetObject)
        return null;
    }
}
