using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Event effect: Finish Level
[AddComponentMenu("Game/Event Effect: Finish Level")]
public class EventEffect_FinishLevel : MonoBehaviour, IEventEffect
{
    public void Trigger()
    {
        // Start Finish Level sequence
        // This will disable the SpatialEventManager so you don't have to worry
        // about this being entered many times
        // Use the Try version, just in case level continued scrolling or player character's projectile
        // hit and killed boss after player character lost their last life, to avoid an assert
        // as we cannot finish level in this case, to prevent conflict with game over.
        // In the case of boss death, we could also prevent this earlier by making all enemies invincible
        // (probably a tangible invincibility that blocks projectiles) during game over -> restart phase.
        InGameManager.Instance.TryFinishLevel();
    }
}
