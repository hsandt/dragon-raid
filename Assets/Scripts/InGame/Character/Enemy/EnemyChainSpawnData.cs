using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyChainSpawnData
{
    [Tooltip("Reference to Enemy Data")]
    public EnemyData enemyData;

    [Tooltip("Position of spawn, in fixed level coordinates. Often to the right of the screen " +
             "(X > 10 and -5.625 <= Y <= 5.625) but can also appear from bottom, top, etc.")]
    public Vector2 spawnPosition = new Vector2(11f, 0f);

    [Tooltip("Number of enemies to spawn")]
    public int spawnCount = 0;
    
    [Tooltip("Time interval between two successive enemy spawns (s)")]
    public float timeInterval = 0f;
}
