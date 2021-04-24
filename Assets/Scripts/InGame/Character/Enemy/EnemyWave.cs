using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Component data for enemy wave
public class EnemyWave : MonoBehaviour
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Time of the first spawn in this wave")]
    private float startTime = 0f;
    
    /// Time of the first spawn in this wave
    public float StartTime => startTime;

    [SerializeField, Tooltip("Array of enemy spawn data. All enemies will be spawned when this wave is triggered.")]
    private EnemySpawnData[] enemySpawnDataArray;
    
    /// Array of enemy spawn data. All enemies will be spawned when this wave is triggered.
    public EnemySpawnData[] EnemySpawnDataArray => enemySpawnDataArray;
}
