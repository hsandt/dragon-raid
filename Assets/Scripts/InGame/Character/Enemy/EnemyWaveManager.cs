using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CommonsPattern;
using UnityConstants;

/// Enemy Wave Manager
/// System for Enemy Wave
public class EnemyWaveManager : SingletonManager<EnemyWaveManager>
{
    /* Cached scene references */
    
    /// Array of enemy waves found in the scene
    private EnemyWave[] m_AllEnemyWaves;

    
    /* State */

    /// List of enemy waves found in the scene and not triggered yet
    private List<EnemyWave> m_RemainingEnemyWaves;
    
    /// Time elapsed since level start
    private float m_TimeSinceLevelStart;
    
    
    protected override void Init()
    {
        GameObject enemyWavesParent = LocatorManager.Instance.FindWithTag(Tags.EnemyWaves);
        m_AllEnemyWaves = enemyWavesParent.GetComponentsInChildren<EnemyWave>();

        Setup();
    }

    public void Setup()
    {
        m_RemainingEnemyWaves = m_AllEnemyWaves.ToList();
        m_TimeSinceLevelStart = 0f;
    }

    private void FixedUpdate()
    {
        m_TimeSinceLevelStart += Time.deltaTime;
        
        // do reverse iteration so we can remove waves by index safely
        for (int i = m_RemainingEnemyWaves.Count - 1; i >= 0; i--)
        {
            EnemyWave enemyWave = m_RemainingEnemyWaves[i];
            
            if (m_TimeSinceLevelStart >= enemyWave.StartTime)
            {
                TriggerEnemyWave(enemyWave);
                m_RemainingEnemyWaves.RemoveAt(i);
            }
        }
    }

    private void TriggerEnemyWave(EnemyWave enemyWave)
    {
        foreach (var enemySpawnData in enemyWave.EnemySpawnDataArray)
        {
            if (enemySpawnData.enemyData)
            {
                EnemyPoolManager.Instance.SpawnCharacter(enemySpawnData.enemyData.enemyName, enemySpawnData.spawnPosition);
            }
            else
            {
                Debug.LogErrorFormat(enemyWave, "Missing EnemyData on {0}", enemyWave);
            }
        }
    }
}
