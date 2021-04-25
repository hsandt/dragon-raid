using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyShootDirectionMode
{
    /// The projectile is sent along the shoot anchor Right direction, set in the editor.
    /// Useful for characters always in a single direction, and for turrets rotating their sprites together with
    /// their shoot anchor.
    FollowShootAnchor,
    
    /// The projectile is aimed at the player character's current position (no predictive aiming)
    TargetPlayerCharacter
}
