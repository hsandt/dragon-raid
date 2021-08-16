using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnData
{
    [Tooltip("Reference to Enemy Data")]
    public EnemyData enemyData;

    [Tooltip("Position of spawn, in fixed level coordinates. Often to the right of the screen " +
             "(X > 10 and -5.625 <= Y <= 5.625) but can also appear from bottom, top, etc.")]
    public Vector2 spawnPosition = new Vector2(11f, 0f);
    
    [Tooltip("Spawn delay. Useful to add a small delay on a particular enemy spawn instead of having to add " +
             "another wave just for that. Unlike Wave, it really uses Time, not Spatial Progress. " +
             "Place multiple enemy spawn points at the same position, with various delays, to create a Chain Squad.")]
    public float delay = 0f;
}
