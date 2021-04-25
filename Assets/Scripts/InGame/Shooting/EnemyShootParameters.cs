using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shoot parameters
[CreateAssetMenu(fileName = "EnemyShootParameters", menuName = "Data/EnemyShootParameters")]
public class EnemyShootParameters : ScriptableObject
{
    [Tooltip("How the enemy should aim fire. Follow Shoot Anchor: follow shoot anchor's Right direction. " +
             "Target Player Character: aim at Player Character. ")]
    public EnemyShootDirectionMode shootDirectionMode = EnemyShootDirectionMode.FollowShootAnchor;
}
