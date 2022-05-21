using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Save Slot parameters
[CreateAssetMenu(fileName = "SaveSlotParameters", menuName = "Data/Save Slot Parameters")]
public class SaveSlotParameters : ScriptableObject
{
    [Tooltip("Number of save slots available to the player")]
    public int saveSlotsCount = 3;
}
