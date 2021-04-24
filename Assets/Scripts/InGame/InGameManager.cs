using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

public class InGameManager : SingletonManager<InGameManager>
{
    /* Cached scene references */
    
    /// Cached player spawn transform
    private Transform playerSpawnTransform;
    
    
    private void Start()
    {
        // Find player character spawn position
        playerSpawnTransform = LocatorManager.Instance.FindWithTag(Tags.PlayerSpawnPosition)?.transform;

        // Setup level
        SetupLevel();
    }

    private void SetupLevel()
    {
        SpawnPlayerCharacter();
    }
    
    private void SpawnPlayerCharacter()
    {
        // Spawn Player Character
        if (playerSpawnTransform != null)
        {
            // Spawn character as a pooled object (in a pool of 1 object)
            CharacterMaster characterMaster = DragonPoolManager.Instance.SpawnCharacter(playerSpawnTransform.position);
            
            // Assign HUD's player health gauge to player health system (on Restart, it only refreshes the gauge)
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

    private void ClearLevel()
    {
        // Despawn the player character
        // We use the generic Pool API to release all objects, but it really only releases 1 here
        DragonPoolManager.Instance.ReleaseAllObjects();

        // Despawn all enemies
        EnemyPoolManager.Instance.ReleaseAllObjects();
        
        // Clean up all projectiles
        ProjectilePoolManager.Instance.ReleaseAllObjects();
    }

    public void RestartLevel()
    {
        ClearLevel();
        SetupLevel();
    }
}
