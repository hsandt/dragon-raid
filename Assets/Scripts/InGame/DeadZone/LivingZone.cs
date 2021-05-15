using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

public class LivingZone : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        // Fail if a game object set to detect Living Zone in Physics 2D collision matrix
        // has no LivingZoneTracker component, to immediately catch objects that may not clean up.
        // Note that `other` may be a trigger (e.g. Projectile hitbox) or not (e.g. enemy movebox)
        var tracker = other.GetComponentOrFail<LivingZoneTracker>();
        tracker.OnExitLivingZone();
    }
}
