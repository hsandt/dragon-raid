using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Cook parameters for Cook System
[CreateAssetMenu(fileName = "CookParameters", menuName = "Data/Cook Parameters")]
public class CookParameters : ScriptableObject
{
    [Tooltip("Cook progress to reach to get the meat well done (eat it for healing or power-up)")]
    public int wellDoneThreshold = 5;
}
