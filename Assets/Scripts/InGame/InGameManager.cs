using System.Collections;
using System.Collections.Generic;
using CommonsHelper;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

public class InGameManager : SingletonManager<InGameManager>
{
    protected override void Init()
    {
        base.Init();


    }

    private void Start()
    {
        // Find player character spawn position
        Transform playerSpawnTr = Locator.FindWithTag(Tags.PlayerSpawnPosition)?.transform;
        if (playerSpawnTr != null)
        {
            // Spawn character as a pooled object (in a pool of 1 object)
            CharacterMaster characterMaster = DragonPoolManager.Instance.SpawnCharacter(playerSpawnTr.position);
            
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
        Debug.Log("restart!");
    }
}
