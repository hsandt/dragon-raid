﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsHelper;
using CommonsPattern;

/// Pool manager for all enemy characters
/// SEO: after LocatorManager
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
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat(this, "[EnemyPoolManager] SpawnCharacter: pool starvation! Cannot spawn enemy '{0}'. " +
            "Consider setting instantiateNewObjectOnStarvation: true on EnemyPoolManager, or increasing its pool size.",
            enemyName);
        #endif
        
        return null;
    }
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void KillAllEnemies()
    {
        foreach (Pool<EnemyCharacterMaster> pool in m_MultiPool.Values)
        {
            foreach (EnemyCharacterMaster activeEnemy in pool.GetObjectsInUse())
            {
                var healthSystem = activeEnemy.GetComponentOrFail<HealthSystem>();
                healthSystem.Kill();
            }
        }
    }
    #endif
}
