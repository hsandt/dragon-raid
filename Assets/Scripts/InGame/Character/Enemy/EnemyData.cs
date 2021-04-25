using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Shoot parameters specific to Enemies
[CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Tooltip("Name of enemy prefab located in Resources/Enemies")]
    public string enemyName;
    
#if UNITY_EDITOR
    [Tooltip(" enemy prefab located in Resources/Enemies")]
    public Texture editorSpawnPreviewTexture;
#endif
}