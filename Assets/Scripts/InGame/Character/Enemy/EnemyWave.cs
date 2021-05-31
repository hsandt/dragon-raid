using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Component data for enemy wave
/// Combine with SpatialEventTrigger to trigger a timely wave
public class EnemyWave : MonoBehaviour, IEventEffect
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Array of enemy spawn data. All enemies will be spawned when this wave is triggered.")]
    private EnemySpawnData[] enemySpawnDataArray;
    
    /// Array of enemy spawn data. All enemies will be spawned when this wave is triggered.
    public EnemySpawnData[] EnemySpawnDataArray => enemySpawnDataArray;

    public void Trigger()
    {
        foreach (var enemySpawnData in EnemySpawnDataArray)
        {
            if (enemySpawnData.enemyData)
            {
                EnemyPoolManager.Instance.SpawnCharacter(enemySpawnData.enemyData.enemyName, enemySpawnData.spawnPosition);
            }
            else
            {
                Debug.LogErrorFormat(this, "Missing EnemyData on {0}", this);
            }
        }
    }
}
