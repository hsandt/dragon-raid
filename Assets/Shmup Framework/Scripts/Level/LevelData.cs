using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// Level Data
/// Create one per level, and reference them in the Level Data List.
[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("Level index. Starts at 0. Add 1 to get human-readable level number.")]
    [Min(0)]
    public int levelIndex;

    // Note: we cannot use UnityConstants ScenesEnum as it's defined per project,
    // and we are in generic Shmup Framework code. Consider using a custom dropdown
    // that loads build scenes when opened and shows them for selection.
    // https://github.com/JohannesMP/unity-scene-reference
    [Tooltip("Index of scene containing this level. Must be in build settings.")]
    [FormerlySerializedAs("sceneEnum")]
    public int sceneIndex;

    [Tooltip("BGM played during level")]
    public AudioClip bgm;

    [Tooltip("Default scrolling speed during this level")]
    [Min(0f)]
    public float baseScrollingSpeed = 2f;

    [Tooltip("Scrolling progress required to reach the end of the level. For now, only used by Level Editor window.")]
    [Min(1f)]
    public float maxScrollingProgress = 100f;
}