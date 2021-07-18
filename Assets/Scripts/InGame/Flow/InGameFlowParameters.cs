using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// In-game Flow Parameters
[CreateAssetMenu(fileName = "InGameFlowParameters", menuName = "Data/In Game Flow Parameters")]
public class InGameFlowParameters : ScriptableObject
{
    [Tooltip("Time between Finish Level sequence start and displaying Performance Assessment")]
    [Range(0f, 2f)]
    public float performanceAssessmentDelay = 1f;
    
    // May be replaced by manual input later so player has time to read the assessment
    [Tooltip("Time between displaying Performance Assessment and loading next level (or going back to title menu)")]
    [Range(0f, 2f)]
    public float loadNextLevelDelay = 1f;
}
