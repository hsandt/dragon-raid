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
        private EnemySpawnData m_EnemySpawnData;
        public EnemySpawnData EnemySpawnData => m_EnemySpawnData;
        
        private float m_TimeLeft;

        public DelayedEnemySpawnInfo(EnemySpawnData enemySpawnData)
        {
            m_EnemySpawnData = enemySpawnData;
            m_TimeLeft = enemySpawnData.delay;
        }
        
        /// Countdown the time by deltaTime, and return true if timer reached 0
        /// Must be called in FixedUpdate
        public bool CountDown(float deltaTime)
        {
            // Implementation is much simpler than CommonsHelper.Timer.CountDown, since we're never supposed to
            // keep a finished timer around, so don't do the m_TimeLeft > 0 check and don't
            // clamp the time to 0.
            m_TimeLeft -= deltaTime;
            
            if (m_TimeLeft <= 0)
            {
                return true;
            }
			
            return false;
        }
    }
    
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
        // Important to avoid keeping a delayed spawn around that would happen out of nowhere after level restart
        StopAllCoroutines();
    }
    
    private void SpawnEnemyFromData(EnemySpawnData enemySpawnData)
    {
        EnemyCharacterMaster enemyCharacterMaster = EnemyPoolManager.Instance.SpawnCharacter(enemySpawnData.enemyData.enemyName, enemySpawnData.spawnPosition, this);
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
                    SpawnEnemyFromData(enemySpawnData);
                }
                else
                {
                    m_DelayedEnemySpawnInfoList.Add(new DelayedEnemySpawnInfo(enemySpawnData));
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
                SpawnEnemyFromData(delayedEnemySpawnInfo.EnemySpawnData);
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
