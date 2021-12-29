using System.Collections;
using System.Collections.Generic;
using UnityConstants;
using UnityEngine;

/// Level Data
/// Create one per level, and reference them in the Level Data List.
[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("Level index. Starts at 0. Add 1 to get human-readable level number.")]
    [Min(0)]
    public int levelIndex;
    
    [Tooltip("Scene containing this level. Must be in build settings. Cast to int to LoadScene.")]
    public ScenesEnum sceneEnum;
    
    [Tooltip("BGM played during level")]
    public AudioClip bgm;
    
    [Tooltip("Default scrolling speed during this level")]
    [Min(0f)]
    public float baseScrollingSpeed = 2f;
    
    [Tooltip("Scrolling progress required to reach the end of the level. For now, only used by Level Editor window.")]
    [Min(1f)]
    public float maxScrollingProgress = 100f;
}