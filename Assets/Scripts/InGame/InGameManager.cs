using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

public class InGameManager : SingletonManager<InGameManager>
{
    /// Cached player spawn transform
    private Transform playerSpawnTransform;
    
    private void Start()
    {
        // Find player character spawn position
        playerSpawnTransform = Locator.FindWithTag(Tags.PlayerSpawnPosition)?.transform;
        if (playerSpawnTransform != null)
        {
            // Spawn character as a pooled object (in a pool of 1 object)
            CharacterMaster characterMaster = DragonPoolManager.Instance.SpawnCharacter(playerSpawnTransform.position);
            
            // Assign HUD's player health gauge to player health system
            var healthSystem = characterMaster.GetComponentOrFail<HealthSystem>();
            HUD.Instance.AssignGaugeHealthPlayerTo(healthSystem);
        }
#if UNITY_EDITOR
        else
        {
            Debug.LogError("[InGameManager] No active object with tag PlayerSpawnPosition found in scene");
        }
#endif
    }

    public void RestartLevel()
    {
        // Despawn and respawn the player character
        // In theory, we could respawn a different one, although set up exactly the same way as the original
        // In practice, the Dragon Pool has only 1 object, so we know we're gonna get the same back
        DragonPoolManager.Instance.ReleaseCharacter();
        if (playerSpawnTransform != null)
        {
            // Respawn character
            // It will also Setup the character and refresh the HUD
            DragonPoolManager.Instance.SpawnCharacter(playerSpawnTransform.position);
        }
    }
}
