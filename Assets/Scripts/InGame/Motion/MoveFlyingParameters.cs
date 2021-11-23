using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Move parameters for flying characters
[CreateAssetMenu(fileName = "MoveFlyingParameters", menuName = "Data/Move Flying Parameters")]
public class MoveFlyingParameters : ScriptableObject
{
    [Tooltip("If checked, the scrolling velocity is added to the character's individual move velocity, " +
             "giving the impression that the character moves freely on the screen. " +
             "Essential for the player character. Most enemies also do this, but sentinel-type enemies should not " +
             "and move relatively to the environment instead.")]
    public bool moveRelativelyToScreen = true;
}
