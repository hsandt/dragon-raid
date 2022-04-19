using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsDebug;

/// Master behaviour for an enemy character
public class EnemyCharacterMaster : CharacterMaster
{
    [Header("Parameters")]

    [Tooltip("Reference to Enemy Data")]
    public EnemyData enemyData;


    /* Parameter references */

    /// Enemy wave that spawned this enemy
    private EnemyWave m_EnemyWave;


    protected override void Init()
    {
        base.Init();

        DebugUtil.AssertFormat(enemyData != null, this, "[EnemyCharacterMaster] Init: Enemy Data not set on {0}", this);
    }

    public override void Clear()
    {
        base.Clear();

        m_EnemyWave = null;
    }

    public void SetEnemyWave(EnemyWave enemyWave)
    {
        DebugUtil.AssertFormat(m_EnemyWave == null, this, "[EnemyCharacterMaster] SetEnemyWave: m_EnemyWave already set on {0} as {1}, " +
            "it will be replaced with {2}. Make sure to clear reference on Clear.", this, m_EnemyWave, enemyWave);

         m_EnemyWave = enemyWave;
    }

    /// Notify Enemy Wave of death of living zone exit
    /// This method allows to distinguish a low-level Pooled Object Release from an actual gameplay death or exit
    /// This avoids triggering gameplay events during Restart (which also Clears all enemies), or having to track
    /// if we are restarting the level with some IsRestarting flag
    public override void OnDeathOrExit()
    {
        if (m_EnemyWave)
        {
            m_EnemyWave.DecrementTrackedEnemiesCount();

            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            m_EnemyWave.DebugUnregisterSpawnedEnemy(this);
            m_EnemyWave.DebugCheckDesync();
            #endif

            if (enemyData.isBoss)
            {
                // Boss was defeated, hide and unassign boss health gauge
                // Note that at once, we don't work from individual components (in this case, HealthSystem),
                // because the Boss gauge is a bit special, it's unique and we may want to plug an animation
                // on hide, etc.
                HUD.Instance.HideAndUnassignGaugeBoss();
            }
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "[EnemyCharacterMaster] m_EnemyWave is null on {0}, cannot DecrementTrackedEnemiesCount.", this);
        }
        #endif
    }
}
