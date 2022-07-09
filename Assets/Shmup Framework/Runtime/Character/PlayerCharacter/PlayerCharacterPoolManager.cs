using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Pool manager for Player Character
/// The only reason to use a pool despite having a single character of this type,
/// is to have an API ready to spawn and despawn without ever destroying the object
/// SEO: after LocatorManager
public class PlayerCharacterPoolManager : PoolManager<PlayerCharacterMaster, PlayerCharacterPoolManager>
{
    protected override void Init()
    {
        if (poolTransform == null)
        {
            poolTransform = LocatorManager.Instance.FindWithTag(ConstantsManager.Tags.PlayerCharacterPool)?.transform;
        }

        base.Init();
    }


    /// Spawn character at position
    public PlayerCharacterMaster SpawnCharacter(Vector2 position)
    {
        PlayerCharacterMaster playerCharacter = AcquireFreeObject();

        if (playerCharacter != null)
        {
            playerCharacter.WarpAndSetup(position);
            return playerCharacter;
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.LogErrorFormat("[PlayerCharacterPoolManager] SpawnCharacter: Cannot spawn player character due to either " +
            "missing prefab or pool starvation. In case of pool starvation, consider setting " +
            "Consider setting instantiateNewObjectOnStarvation: true on PlayerCharacterPoolManager, or increasing its pool size.");
        #endif

        return null;
    }

    public void PauseCharacter()
    {
        foreach (PlayerCharacterMaster playerCharacter in GetObjectsInUse())
        {
            playerCharacter.Pause();
        }
    }

    public void ResumeCharacter()
    {
        foreach (PlayerCharacterMaster playerCharacter in GetObjectsInUse())
        {
            playerCharacter.Resume();
        }
    }

    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void Cheat_KillPlayerCharacter()
    {
        // Pool is generic, so loop on all player characters although we know there is only one
        foreach (PlayerCharacterMaster activePlayerCharacter in GetObjectsInUse())
        {
            var healthSystem = activePlayerCharacter.GetComponentOrFail<HealthSystem>();
            healthSystem.Cheat_TryBeKilled();
        }
    }
    #endif
}
