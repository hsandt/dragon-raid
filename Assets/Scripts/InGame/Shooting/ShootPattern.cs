using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shoot Pattern
[CreateAssetMenu(fileName = "ShootPattern", menuName = "Data/Shoot Pattern")]
public class ShootPattern : ScriptableObject
{
    [Tooltip("Which angle reference to use for the shot. " +
             "Shoot Anchor Forward: use anchor's Right direction. " +
             "Target Player Character: use direction from shoot anchor to Player Character.")]
    public EnemyShootDirectionMode shootDirectionMode = EnemyShootDirectionMode.ShootAnchorForward;

    [Tooltip("First angle to shoot at, from the reference angle defined by Shoot Direction Mode " +
             "(degrees, 0 for forward, positive CCW)")]
    [Range(-180f, 180f)]
    public float angleStart = 0f;

    [Tooltip("Last angle to shoot at (same convention as Angle Start). No wrapping around -180/180.")]
    [Range(-180f, 180f)]
    public float angleEnd = 0f;

    [Tooltip("Number of bullets to shoot. First one is shot at Angle Start, last one at Angle End, " +
             "and those in the middle are spread at regular angle intervals between Start and End.")]
    [Min(1)]
    public int bulletCount = 1;

    [Tooltip("Bullet speed (m/s)")]
    [Min(0f)]
    public float bulletSpeed = 6f;

    [Tooltip("Duration of the pattern, time between first and last bullet. " +
             "If 0, all bullets are fired simultaneously.")]
    [Min(0f)]
    public float duration = 0f;
}
