using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Extra Lives parameters for Extra Lives System
[CreateAssetMenu(fileName = "ExtraLivesParameters", menuName = "Data/Extra Lives Parameters")]
public class ExtraLivesParameters : ScriptableObject
{
    [Tooltip("Initial and max extra lives count")]
    public int maxExtraLivesCount = 3;
}
