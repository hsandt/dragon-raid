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
    private Transform m_PlayerSpawnTransform;

    /// Player Character Master component, reference stored after spawn
    private CharacterMaster m_playerCharacterMaster;
    
    /// Player Character Master component, reference stored after spawn (getter)
    public CharacterMaster PlayerCharacterMaster => m_playerCharacterMaster;
    
    
    private void Start()
    {
        // Find player character spawn position
        m_PlayerSpawnTransform = LocatorManager.Instance.FindWithTag(Tags.PlayerSpawnPosition)?.transform;

        // Setup level
        SetupLevel();
    }

    private void SetupLevel()
    {
        SpawnPlayerCharacter();
        EnemyWaveManager.Instance.Setup();
    }
    
    private void SpawnPlayerCharacter()
    {
        // Spawn Player Character
        if (m_PlayerSpawnTransform != null)
        {
            // Spawn character as a pooled object (in a pool of 1 object)
            m_playerCharacterMaster = DragonPoolManager.Instance.SpawnCharacter(m_PlayerSpawnTransform.position);
            
            // Assign HUD's player health gauge to player health system (on Restart, it only refreshes the gauge)
            var healthSystem = m_playerCharacterMaster.GetComponentOrFail<HealthSystem>();
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
        
        // First clean up reference to avoid relying on Unity's "destroyed null"
        m_playerCharacterMaster = null;
        
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
