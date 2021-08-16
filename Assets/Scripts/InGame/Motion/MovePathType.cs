using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovePathType
{
    // Enum values are serialized in EnemyMoveFlyingParameters,
    // so don't reorder these values, only add new ones at the end
    Linear, // Straight line
    Wave,   // Sinusoidal wave
}
