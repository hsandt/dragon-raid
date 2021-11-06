using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for an enemy character
public class EnemyCharacterMaster : CharacterMaster
{
    /* Parameter references */
    
    /// Enemy wave that spawned this enemy
    private EnemyWave m_EnemyWave;


    public override void Clear()
    {
        base.Clear();

        m_EnemyWave = null;
    }

    public void SetEnemyWave(EnemyWave enemyWave)
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(m_EnemyWave == null, this, "[EnemyCharacterMaster] m_EnemyWave already set on {0} as {1}, " +
            "it will be replaced with {2}. Make sure to clear reference on Clear.", this, m_EnemyWave, enemyWave);
        #endif
        
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
            m_EnemyWave.UnregisterSpawnedEnemy(this);
            m_EnemyWave.CheckDesync();
            #endif
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "[EnemyCharacterMaster] m_EnemyWave is null on {0}, cannot DecrementTrackedEnemiesCount.", this);
        }
        #endif
    }
}
