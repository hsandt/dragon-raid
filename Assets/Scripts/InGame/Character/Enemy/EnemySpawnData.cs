using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnData : EnemyBaseSpawnData
{
    [Tooltip("Spawn delay. Useful to add a small delay on a particular enemy spawn instead of having to add " +
             "another wave just for that. Unlike Wave, it really uses Time, not Spatial Progress. " +
             "If chaining multiple enemies at the same position, prefer Enemy Chain Spawn Data.")]
    public float delay = 0f;

    [Tooltip("Optional root used to override the default one on this specific enemy. " +
             "It actually represents the whole Behaviour Tree made by this root and all its children ." +
             "Useful to customize enemy behavior, by either only changing parameters or the full behaviour tree.")]
    public BehaviourTreeRoot overrideRoot = null;
}
