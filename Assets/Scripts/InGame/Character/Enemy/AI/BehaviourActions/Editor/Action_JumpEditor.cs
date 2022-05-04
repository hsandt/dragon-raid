using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// We only define custom editor to inherit OnInspectorGUI and refresh node name on property change
[CustomEditor(typeof(Action_Jump))]
public class Action_JumpEditor : BehaviourActionEditor
{
}