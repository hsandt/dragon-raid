using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// Cook parameters for Cook System
[CreateAssetMenu(fileName = "CookParameters", menuName = "Data/Cook Parameters")]
public class CookParameters : ScriptableObject
{
    [Tooltip("Probability of spawning Cooked Enemy (at current Cook Level) on death")]
    [Range(0f, 1f)]
    public float cookedEnemySpawnProbability = 0.5f;
    
    [Tooltip("Cook progress to reach so the enemy spawns a certain level of CookedEnemy pick-up on death.\n" +
             "Rare below Element 0\n" +
             "Medium between Element 0 and Element 1 (excluded)\n" +
             "WellDone between Element 1 and Element 2 (excluded)\n" +
             "Carbonized from Element 2")]
    public int[] cookLevelThresholds = new int[EnumUtil.GetCount<CookLevel>() - 1];
}
