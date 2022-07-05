using UnityEngine;

public class EnemyBaseSpawnData
{
    [Tooltip("Reference to Enemy Data")]
    public EnemyData enemyData;

    [Tooltip("Position of spawn, in fixed level coordinates. Often to the right of the screen " +
             "(X > 10 and -5.625 <= Y <= 5.625) but can also appear from bottom, top, etc.")]
    public Vector2 spawnPosition = new(11f, 0f);
}