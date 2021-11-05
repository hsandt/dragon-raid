using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyChainSpawnData : EnemyBaseSpawnData
{
    [Tooltip("Number of enemies to spawn")]
    public int spawnCount = 0;
    
    [Tooltip("Time interval between two successive enemy spawns (s)")]
    public float timeInterval = 0f;
}
