using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Component data for enemy wave
/// Combine with EventTrigger_SpatialProgress and EventEffect_StartEnemyWave to trigger a timely wave
public class EnemyWave : MonoBehaviour
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Array of enemy spawn data. All enemies will be spawned when this wave is triggered.")]
    private EnemySpawnData[] enemySpawnDataArray;
    
    /// Array of enemy spawn data. All enemies will be spawned when this wave is triggered.
    public EnemySpawnData[] EnemySpawnDataArray => enemySpawnDataArray;

    
    /* Dynamic external references */

    /// Optional wave cleared event effect, triggered when all enemies spawned from this wave have died or exited
    private IEventEffect m_OnWaveClearedEventEffect;
    
    
    /* State */
    
    /// Tracked count of enemies spawned during current wave
    /// When this count is decremented back to 0, the associated Event Effect is triggered.
    private int m_TrackedEnemiesCount;

    
    public void Setup()
    {
        m_TrackedEnemiesCount = 0;
    }
    
    public void Clear()
    {
        // Important to avoid keeping a delayed spawn around that would happen out of nowhere after level restart
        StopAllCoroutines();
    }
    
    public void StartWave()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_TrackedEnemiesCount == 0, this,
            "[EnemyWave] StartWave is called but m_TrackedEnemiesCount is {0}, expected 0. " +
            "Make sure to Setup every wave on Level Restart, and not Starting the same wave again before it is cleared.",
            m_TrackedEnemiesCount);
        #endif
        
        foreach (var enemySpawnData in EnemySpawnDataArray)
        {
            if (enemySpawnData.enemyData)
            {
                // Track enemy whether immediate or delayed spawn because if we only increment count on actual spawn,
                // a quick player may destroy all immediate enemies before delayed enemies even arrive and incorrectly
                // trigger the On Wave Cleared Event Effect. A priori all enemies will eventually spawn, so just
                // increment. If some spawning fails below, then we will decrement back.
                ++m_TrackedEnemiesCount;
                
                if (enemySpawnData.delay <= 0f)
                {
                    EnemyCharacterMaster enemyCharacterMaster = EnemyPoolManager.Instance.SpawnCharacter(enemySpawnData.enemyData.enemyName, enemySpawnData.spawnPosition, this);
                    if (enemyCharacterMaster == null)
                    {
                        // Spawning failed, immediately stop tracking
                        --m_TrackedEnemiesCount;
                    }
                }
                else
                {
                    StartCoroutine(SpawnEnemyWithDelayAsync(enemySpawnData));
                }
                

            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
            {
                Debug.LogErrorFormat(this, "Missing EnemyData on {0}", this);
            }
            #endif
        }
    }
    
    public void RegisterOnWaveClearedEventEffect(IEventEffect eventEffect)
    {
        m_OnWaveClearedEventEffect = eventEffect;
    }

    public void DecrementTrackedEnemiesCount()
    {
        if (m_TrackedEnemiesCount > 0)
        {
            --m_TrackedEnemiesCount;
            
            if (m_TrackedEnemiesCount == 0)
            {
                // Last enemy cleared, trigger wave cleared event effect, if any
                m_OnWaveClearedEventEffect?.Trigger();
            }
        }
        else
        {
            Debug.LogErrorFormat(this, "[EnemyWave] m_TrackedEnemiesCount ({0}) is not positive on {1}, cannot decrement",
                m_TrackedEnemiesCount, this);
        }
    }

    private IEnumerator SpawnEnemyWithDelayAsync(EnemySpawnData enemySpawnData)
    {
        yield return new WaitForSeconds(enemySpawnData.delay);
        
        EnemyCharacterMaster enemyCharacterMaster = EnemyPoolManager.Instance.SpawnCharacter(enemySpawnData.enemyData.enemyName, enemySpawnData.spawnPosition, this);
        if (enemyCharacterMaster == null)
        {
            // Spawning failed, immediately stop tracking
            --m_TrackedEnemiesCount;
        }
    }
}
