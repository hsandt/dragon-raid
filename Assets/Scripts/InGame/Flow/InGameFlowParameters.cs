using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// In-game Flow Parameters
[CreateAssetMenu(fileName = "InGameFlowParameters", menuName = "Data/In Game Flow Parameters")]
public class InGameFlowParameters : ScriptableObject
{
    [Tooltip("Duration of level fade-in on start and restart")]
    [Range(0f, 2f)]
    public float fadeInDuration = 1f;

    [Tooltip("Time between Finish Level sequence start and displaying Performance Assessment")]
    [Range(0f, 2f)]
    public float performanceAssessmentDelay = 1f;
    
    // May be replaced by manual input later so player has time to read the assessment
    [Tooltip("Time between displaying Performance Assessment and loading next level (or going back to title menu)")]
    [Range(0f, 2f)]
    public float loadNextLevelDelay = 1f;

    [Tooltip("Time between Player Character losing last life and starting game over + restart animation")]
    [Range(0f, 2f)]
    public float gameOverDelay = 1f;
    
    [Tooltip("Duration of level fade-out on gameover")]
    [Range(0f, 2f)]
    public float gameOverFadeOutDuration = 1f;
}
