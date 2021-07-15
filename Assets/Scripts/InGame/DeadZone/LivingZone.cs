using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Instead of defining a Dead Zone, we prefer defining a Living Zone is an approximately complementary area.
/// This ensures that big objects are only cleaned up (despawned) after leaving the Living Zone completely,
/// while a classic Dead Zone may remove big objects too early, as soon as they touch it.
/// Note that this relies on the main collider, not object visuals. Make sure to expand the Living Zone
/// slightly beyond the screen surface to allow visuals bigger than the main collider to exit the screen
/// completely before the object is cleaned up.
/// Physics 2D: LivingZone layer must collider with any layer containing entities that can leave the screen,
/// like enemy characters and projectiles. In addition, those entities should have a LivingZoneTracker component
/// on them.
public class LivingZone : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D other)
    {
        // Fail if a game object set to detect Living Zone in Physics 2D collision matrix
        // has no LivingZoneTracker component, to immediately catch objects that may not clean up.
        // Note that `other` may be a trigger (e.g. Projectile hitbox) or not (e.g. enemy movebox)
        
        // ! If the MoveBox collider at the root is not big enough to guarantee the entity will despawn when its
        // ! visual is completely out of screen, then the Living Zone tracker must be placed on some child with a bigger collider
        // ! covering the whole entity's visual. If you do so, split the MoveBox and LivingZoneTracker layers to avoid asserting
        // ! on GetComponentOrFail, by disabling collision between MoveBox and LivingZone entirely (all entities must then define
        // ! a child on LivingZoneTracker)
        var tracker = other.GetComponentOrFail<LivingZoneTracker>();
        
        // Process exiting live zone, but only if the entity is still alive (not in the process of being
        // released/deactivated as during Restart). Make sure to add a MasterBehaviour script to entities with
        // a LivingZoneTracker so it can Setup/Clear them appropriately so IsAlive is up-to-date. 
        if (tracker.IsAlive)
        {
            tracker.OnExitLivingZone();
        }
    }
}
