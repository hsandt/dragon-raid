using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shoot parameters specific to Enemies
[CreateAssetMenu(fileName = "EnemyShootParameters", menuName = "Data/Enemy Shoot Parameters")]
public class EnemyShootParameters : ScriptableObject
{
    [Tooltip("How the enemy should aim fire. Shoot Anchor Forward: shoot along shoot anchor's Right direction. " +
             "Target Player Character: aim at Player Character. ")]
    public EnemyShootDirectionMode shootDirectionMode = EnemyShootDirectionMode.ShootAnchorForward;
}
