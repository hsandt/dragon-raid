using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Enemy Wave Manager
/// System for Enemy Wave
/// The Spatial Event Manager already handles the whole Start Wave logic, so this manager only cares about
/// calling Setup on Level Restart.
/// The Spatial Event Manager could also retrieve enemy waves as indirect children of the parent tagged 'SpatialEvents',
/// but in case we add non-spatial waves later (e.g. a boss that spawns sub-waves dynamically), we prefer tracking them
/// with a different manager.
/// SEO: after LocatorManager
public class EnemyWaveManager : SingletonManager<EnemyWaveManager>
{
    /* Cached scene references */
    
    /// List of pairs (spatial event trigger, event effect) found in the scene
    /// This includes all enemy wave events.
    private EnemyWave[] m_AllEnemyWaves;

    
    protected override void Init()
    {
        // Find all Enemy Wave Triggers in the scene, then find any associated effect
        // We dropped the ECS-tag-component approach and prefer a classic interface approach with IEventEffect,
        // so we can move handling code to each of the event effect classes
        GameObject enemyWavesParent = LocatorManager.Instance.FindWithTag(Tags.EnemyWaves);
        if (enemyWavesParent != null)
        {
            m_AllEnemyWaves = enemyWavesParent.GetComponentsInChildren<EnemyWave>();
        }
    }

    /// Setup is managed by InGameManager, so not called on Start
    public void Setup()
    {
        foreach (EnemyWave enemyWave in m_AllEnemyWaves)
        {
            enemyWave.Setup();
        }
    }
}
