using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Component data for enemy wave
/// Combine with EventTrigger_SpatialProgress to trigger a timely wave
public class EnemyWave : MonoBehaviour, IEventEffect
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Array of enemy spawn data. All enemies will be spawned when this wave is triggered.")]
    private EnemySpawnData[] enemySpawnDataArray;
    
    /// Array of enemy spawn data. All enemies will be spawned when this wave is triggered.
    public EnemySpawnData[] EnemySpawnDataArray => enemySpawnDataArray;

    
    /* Dynamic external references */

    /// Optional death event effect plugged to every enemy spawned from this wave
    private IEventEffect m_OnDeathEventEffect;
    
    
    public void Trigger()
    {
        foreach (var enemySpawnData in EnemySpawnDataArray)
        {
            if (enemySpawnData.enemyData)
            {
                CharacterMaster characterMaster = EnemyPoolManager.Instance.SpawnCharacter(enemySpawnData.enemyData.enemyName, enemySpawnData.spawnPosition);
                var healthSystem = characterMaster.GetComponent<HealthSystem>();
                if (healthSystem != null)
                {
                    healthSystem.RegisterOnDeathEventEffect(m_OnDeathEventEffect);
                }
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                else
                {
                    Debug.LogErrorFormat(this, "Enemy character of type '{0}' spawned from {1} has no Health System",
                        enemySpawnData.enemyData.enemyName, this);
                }
                #endif
            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
            {
                Debug.LogErrorFormat(this, "Missing EnemyData on {0}", this);
            }
            #endif
        }
    }
    
    public void RegisterOnDeathEventEffect(IEventEffect eventEffect)
    {
        m_OnDeathEventEffect = eventEffect;
    }
}
