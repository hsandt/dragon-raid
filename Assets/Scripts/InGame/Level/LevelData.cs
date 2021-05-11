using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shoot parameters specific to Enemies
[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Tooltip("BGM played during level")]
    public AudioClip bgm;
}