using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Component data for enemy wave
/// Combine with EventTrigger_SpatialProgress and EventEffect_StartEnemyWave to trigger a timely wave
public class EnemyWave : MonoBehaviour
{
    private class DelayedEnemySpawnInfo
    {
        private readonly EnemyData m_EnemyData;
        public EnemyData EnemyData => m_EnemyData;
        
        private Vector2 m_SpawnPosition;
        public Vector2 SpawnPosition => m_SpawnPosition;
    
        private ActionSequence m_OverrideActionSequence;
        public ActionSequence OverrideActionSequence => m_OverrideActionSequence;

        
        private float m_TimeLeft;

        public DelayedEnemySpawnInfo(EnemyData enemyData, Vector2 spawnPosition, ActionSequence overrideActionSequence,
            float delay)
        {
            m_EnemyData = enemyData;
            m_SpawnPosition = spawnPosition;
            m_OverrideActionSequence = overrideActionSequence;
            m_TimeLeft = delay;
        }
        
        /// Countdown the time by deltaTime, and return true if timer reached 0
        /// Must be called in FixedUpdate
        public bool CountDown(float deltaTime)
        {
            // Implementation is much simpler than CommonsHelper.Timer.CountDown, since we're never supposed to
            // keep a finished timer around, so don't do the m_TimeLeft > 0 check and don't
            // clamp the time to 0.
            m_TimeLeft -= deltaTime;
            return m_TimeLeft <= 0;
        }
    }
    
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Array of enemy spawn data. All enemies will be spawned when this wave is triggered.")]
    private EnemySpawnData[] enemySpawnDataArray;
    
    /// Array of enemy spawn data. All enemies will be spawned when this wave is triggered.
    public EnemySpawnData[] EnemySpawnDataArray => enemySpawnDataArray;

    [SerializeField, Tooltip("Array of enemy chain spawn data. Allows chaining enemies of the same type.")]
    private EnemyChainSpawnData[] enemyChainSpawnDataArray;
    
    /// Array of enemy spawn data. All enemies will be spawned when this wave is triggered.
    public EnemyChainSpawnData[] EnemyChainSpawnDataArray => enemyChainSpawnDataArray;

    
    /* Dynamic external references */

    /// Optional wave cleared event effect, triggered when all enemies spawned from this wave have died or exited
    private IEventEffect m_OnWaveClearedEventEffect;
    
    
    /* State */
    
    /// Tracked count of enemies spawned during current wave
    /// When this count is decremented back to 0, the associated Event Effect is triggered.
    private int m_TrackedEnemiesCount;
    
    /// List of delayed enemy spawn info
    /// Allows to manually update delay spawn timers and pause them on game pause
    /// (we cannot just use coroutines because they are not paused when script is disabled)
    private readonly List<DelayedEnemySpawnInfo> m_DelayedEnemySpawnInfoList = new List<DelayedEnemySpawnInfo>();

    
    public void Setup()
    {
        m_TrackedEnemiesCount = 0;
    }
    
    public void Clear()
    {
    }
    
    private void SpawnEnemy(EnemyData enemyData, Vector2 spawnPosition, ActionSequence overrideActionSequence = null)
    {
        EnemyCharacterMaster enemyCharacterMaster = EnemyPoolManager.Instance.SpawnCharacter(enemyData.enemyName, spawnPosition, this, overrideActionSequence);
        if (enemyCharacterMaster == null)
        {
            // Spawning failed, immediately stop tracking
            --m_TrackedEnemiesCount;
        }
    }

    public void StartWave()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_TrackedEnemiesCount == 0, this,
            "[EnemyWave] StartWave is called but m_TrackedEnemiesCount is {0}, expected 0. " +
            "Make sure to Setup every wave on Level Restart, and not Starting the same wave again before it is cleared.",
            m_TrackedEnemiesCount);
        #endif
        
        foreach (var enemySpawnData in enemySpawnDataArray)
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
                    SpawnEnemy(enemySpawnData.enemyData, enemySpawnData.spawnPosition, enemySpawnData.overrideActionSequence);
                }
                else
                {
                    m_DelayedEnemySpawnInfoList.Add(new DelayedEnemySpawnInfo(
                        enemySpawnData.enemyData,
                        enemySpawnData.spawnPosition,
                        enemySpawnData.overrideActionSequence,
                        enemySpawnData.delay));
                }
            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
            {
                Debug.LogErrorFormat(this, "Missing EnemyData on {0}", this);
            }
            #endif
        }
        
        foreach (var enemyChainSpawnData in enemyChainSpawnDataArray)
        {
            if (enemyChainSpawnData.enemyData)
            {
                // Track all chained enemies now for the same reason as above
                m_TrackedEnemiesCount += enemyChainSpawnData.spawnCount;
                
                // Spawn first enemy of the chain now
                SpawnEnemy(enemyChainSpawnData.enemyData, enemyChainSpawnData.spawnPosition);

                // Prepare spawning all the other enemies after a delay by regular intervals
                // Make sure to start index at 1
                for (int i = 1; i < enemyChainSpawnData.spawnCount; i++)
                {
                    float delay = i * enemyChainSpawnData.timeInterval;
                    m_DelayedEnemySpawnInfoList.Add(new DelayedEnemySpawnInfo(
                        enemyChainSpawnData.enemyData,
                        enemyChainSpawnData.spawnPosition,
                        null,
                        delay));
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

    private void FixedUpdate()
    {
        // Reverse loop for stable removal by index during iteration on list
        for (int i = m_DelayedEnemySpawnInfoList.Count - 1; i >= 0; i--)
        {
            DelayedEnemySpawnInfo delayedEnemySpawnInfo = m_DelayedEnemySpawnInfoList[i];
            if (delayedEnemySpawnInfo.CountDown(Time.deltaTime))
            {
                SpawnEnemy(delayedEnemySpawnInfo.EnemyData, delayedEnemySpawnInfo.SpawnPosition, delayedEnemySpawnInfo.OverrideActionSequence);
                m_DelayedEnemySpawnInfoList.RemoveAt(i);  // safe thanks to reverse loop
            }
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
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "[EnemyWave] m_TrackedEnemiesCount ({0}) is not positive on {1}, cannot decrement",
                m_TrackedEnemiesCount, this);
        }
        #endif
    }
}
