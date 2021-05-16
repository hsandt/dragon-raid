using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;
using UnityConstants;

/// Spatial Event Manager
/// System for Spatial Event Trigger x Enemy Wave/Event Finish Level
public class SpatialEventManager : SingletonManager<SpatialEventManager>
{
    /* Cached scene references */
    
    /// List of pairs (spatial event trigger, enemy wave effect) found in the scene
    private readonly List<Pair<SpatialEventTrigger, EnemyWave>> m_AllEnemyWaveEventPairs = new List<Pair<SpatialEventTrigger, EnemyWave>>();

    /// Event trigger for finish level effect found in the scene
    /// FinishLevel component is empty, so we don't even need to store it
    private SpatialEventTrigger m_FinishLevelEventTrigger;
    
    
    /* State */

    /// List of pairs (spatial event trigger, enemy wave effect) found in the scene
    /// and not triggered yet
    private List<Pair<SpatialEventTrigger, EnemyWave>> m_RemainingEnemyWaveEventPairs;

    
    protected override void Init()
    {
        // Find all Spatial Event Triggers in the scene, then find any associated effect
        // Usually we would define events as child class components of some base event component,
        // which contains both the trigger condition and the effect, as a virtual OnTrigger
        // method. However, this is the polymorphic approach, and in this project we mostly use
        // ECS-style components with composition over inheritance. There, we prefer splitting
        // event trigger condition and effect, and besides instead of defining effect as
        // a virtual method, we let the System (this script) check every effect component
        // type and handle them manually.
        GameObject spatialEventsParent = LocatorManager.Instance.FindWithTag(Tags.SpatialEvents);
        var allSpatialEventTriggers = spatialEventsParent.GetComponentsInChildren<SpatialEventTrigger>();
        
        foreach (var spatialEventTrigger in allSpatialEventTriggers)
        {
            // Check which effect is associated to the trigger
            // Rather than having each event effect derive from 
            var enemyWave = spatialEventTrigger.GetComponent<EnemyWave>();
            if (enemyWave != null)
            {
                // Store pair (event trigger, effect) to quickly access both later
                // so we can simulate a 2-component iteration, ECS-style
                m_AllEnemyWaveEventPairs.Add(Pair.Create(spatialEventTrigger, enemyWave));
                continue;
            }
            
            var finishLevel = spatialEventTrigger.GetComponent<EventFinishLevel>();
            if (finishLevel != null)
            {
                // Just store event trigger, FinishLevel component is empty anyway
                m_FinishLevelEventTrigger = spatialEventTrigger;
                continue;
            }
        }
    }

    /// Setup is managed by InGameManager, so not called on Start
    public void Setup()
    {
        // Reset all enemy waves
        m_RemainingEnemyWaveEventPairs = m_AllEnemyWaveEventPairs.ToList();
    }

    private void FixedUpdate()
    {
        // do reverse iteration so we can remove waves by index safely
        
        // OPTIMIZATION: when we have many waves, this will start getting slow
        // in this case, it's better to enforce convention on level design that all wave objects
        // are ordered chronologically, so we can only check the next one i.e. the first in the
        // remaining list
        for (int i = m_RemainingEnemyWaveEventPairs.Count - 1; i >= 0; i--)
        {
            var enemyWaveEventPair = m_RemainingEnemyWaveEventPairs[i];
            SpatialEventTrigger eventTrigger = enemyWaveEventPair.First;
            EnemyWave enemyWave = enemyWaveEventPair.Second;
            
            if (ScrollingManager.Instance.SpatialProgress >= eventTrigger.RequiredSpatialProgress)
            {
                TriggerEnemyWave(enemyWave);
                m_RemainingEnemyWaveEventPairs.RemoveAt(i);
            }
        }

        if (m_FinishLevelEventTrigger != null)
        {
            if (ScrollingManager.Instance.SpatialProgress >= m_FinishLevelEventTrigger.RequiredSpatialProgress)
            {
                // Start Finish Level sequence
                // This will disable the SpatialEventManager so you don't have to worry
                // about this being entered many times
                InGameManager.Instance.FinishLevel();
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
