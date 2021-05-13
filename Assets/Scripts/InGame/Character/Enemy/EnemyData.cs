using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Enemy data
/// Even when name is enough, it makes it more convenient to select as serialized reference than inputting
/// the enemy name manually in a string field. It also avoids breaking if name changes later.
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