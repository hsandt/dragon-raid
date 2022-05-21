using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [Tooltip("Rigidbody of parallax layer")]
    public Rigidbody2D rigidbody;
    
    [Tooltip("Parallax speed factor. 1 to move exactly at scrolling speed. Increase for foreground, reduce for background.")]
    [Min(0f)]
    public float parallaxSpeedFactor = 1f;
}
