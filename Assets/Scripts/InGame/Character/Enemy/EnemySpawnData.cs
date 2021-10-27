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
             "If chaining multiple enemies at the same position, prefer Enemy Chain Spawn Data.")]
    public float delay = 0f;

    [Tooltip("Optional action sequence used to override the default one on this specific enemy. " +
             "Useful to customize enemy behavior, by either only changing parameters or the full action sequence.")]
    public ActionSequence overrideActionSequence = null;
}
