using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Component data for enemy wave
/// Combine with SpatialEventTrigger to trigger a timely wave
public class EnemyWave : MonoBehaviour
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Array of enemy spawn data. All enemies will be spawned when this wave is triggered.")]
    private EnemySpawnData[] enemySpawnDataArray;
    
    /// Array of enemy spawn data. All enemies will be spawned when this wave is triggered.
    public EnemySpawnData[] EnemySpawnDataArray => enemySpawnDataArray;
}
