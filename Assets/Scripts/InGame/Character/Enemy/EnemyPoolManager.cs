using System.Collections;
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
    public EnemyCharacterMaster SpawnCharacter(string enemyName, Vector2 position, EnemyWave enemyWave, BehaviourAction overrideRootAction)
    {
        EnemyCharacterMaster enemyCharacter = GetObject(enemyName);
        
        if (enemyCharacter != null)
        {
            enemyCharacter.Spawn(position);
            enemyCharacter.SetEnemyWave(enemyWave);

            var behaviourTreePlayer = enemyCharacter.GetComponent<BehaviourTreePlayer>();
            if (behaviourTreePlayer != null)
            {
                behaviourTreePlayer.StartBehaviourTree(overrideRootAction);
            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else if (overrideRootAction != null)
            {
                Debug.LogErrorFormat(enemyWave, "[EnemyPoolManager] SpawnCharacter: Override Root Action {0} was passed " +
                    "to spawn enemy {1} for wave {2}, but this enemy has no BehaviourTreePlayer component.",
                    overrideRootAction, enemyName, enemyWave);
            }
            #endif
            
            return enemyCharacter;
        }
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat(this, "[EnemyPoolManager] SpawnCharacter: Cannot spawn enemy '{0}' for wave {1} due to either " +
            "missing prefab or pool starvation. In case of pool starvation, consider setting " +
            "Consider setting instantiateNewObjectOnStarvation: true on EnemyPoolManager, or increasing its pool size.",
            enemyName, enemyWave);
        #endif
        
        return null;
    }
    
    public void PauseAllEnemies()
    {
        foreach (EnemyCharacterMaster activeEnemy in GetObjectsInUseInAllPools())
        {
            activeEnemy.Pause();
        }
    }
    
    public void ResumeAllEnemies()
    {
        foreach (EnemyCharacterMaster activeEnemy in GetObjectsInUseInAllPools())
        {
            activeEnemy.Resume();
        }
    }
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void KillAllEnemies()
    {
        foreach (EnemyCharacterMaster activeEnemy in GetObjectsInUseInAllPools())
        {
            var healthSystem = activeEnemy.GetComponentOrFail<HealthSystem>();
            healthSystem.TryBeKilled();
        }
    }
    #endif
}
