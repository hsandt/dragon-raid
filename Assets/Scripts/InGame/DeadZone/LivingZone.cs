using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Instead of defining a Dead Zone, we prefer defining a Living Zone is an approximately complementary area.
/// This ensures that big objects are only cleaned up after leaving the Living Zone completely,
/// while a classic Dead Zone may remove big objects too early, as soon as they touch it.
/// Note that this relies on the main collider, not object visuals. Make sure to expand the Living Zone
/// slightly beyond the screen surface to allow visuals bigger than the main collider to exit the screen
/// completely before the object is cleaned up.
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
