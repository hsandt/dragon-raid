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
    
    /// List of enemy waves found in the scene and not triggered yet
    private List<EnemyWave> m_EnemyWaves;

    
    /* State */

    /// Time elapsed since level start
    private float m_TimeSinceLevelStart;
    
    
    protected override void Init()
    {
        GameObject enemyWavesParent = LocatorManager.Instance.FindWithTag(Tags.EnemyWaves);
        m_EnemyWaves = enemyWavesParent.GetComponentsInChildren<EnemyWave>().ToList();
    }

    private void FixedUpdate()
    {
        m_TimeSinceLevelStart += Time.deltaTime;
        
        // do reverse iteration so we can remove waves by index safely
        for (int i = m_EnemyWaves.Count - 1; i >= 0; i--)
        {
            EnemyWave enemyWave = m_EnemyWaves[i];
            
            if (m_TimeSinceLevelStart >= enemyWave.StartTime)
            {
                TriggerEnemyWave(enemyWave);
                m_EnemyWaves.RemoveAt(i);
            }
        }
    }

    private void TriggerEnemyWave(EnemyWave enemyWave)
    {
        foreach (var enemySpawnData in enemyWave.EnemySpawnDataArray)
        {
            EnemyPoolManager.Instance.SpawnCharacter(enemySpawnData.enemyName, enemySpawnData.spawnPosition);
        }
    }
}
