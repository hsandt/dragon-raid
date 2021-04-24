using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnData
{
    [Tooltip("Name of the enemy to spawn")]
    public string enemyName;

    [Tooltip("Position of spawn, in fixed level coordinates. Often to the right of the screen " +
             "(X > 10 and -5.625 <= Y <= 5.625) but can also appear from bottom, top, etc.")]
    public Vector2 spawnPosition;
}
