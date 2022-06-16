using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySquadSpawnData : EnemyBaseSpawnData
{
    [Tooltip("Array of spatial offset of each squad unit spawn point relative to spawn anchor. " +
        "Its length is also the squad size.")]
    public Vector2[] formationOffsets;

    [Tooltip("Moving anchor prefab: The anchor is a reference point that moves and guides squad units. " +
        "It should be a prefab with a MoveFlying component and MoveFlyingIntention component. Apart from that, " +
        "the only characteristic that matters is whether MoveFlying data is set to move relatively to screen or world.")]
    public GameObject movingAnchorPrefab;

    [Tooltip("Behaviour tree root (in scene or as asset prefab, if reused) that should control anchor motion.")]
    public BehaviourTreeRoot anchorMoveBehaviourTreeRoot;

    /// Number of enemies spawned. Derived from formation offsets
    public int SpawnCount => formationOffsets.Length;
}
