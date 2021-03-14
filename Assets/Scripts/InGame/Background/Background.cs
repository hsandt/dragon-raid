using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// Handles background parallax scrolling
public class Background : MonoBehaviour
{
    [Tooltip("Array of rigidbodies of parallax layers, from farthest to closest")]
    public Rigidbody2D[] parallaxLayerRigidbodies;

    [SerializeField, Tooltip("Scrolling speed of the farthest parallax layer")]
    private float parallaxSpeedBase = 1f;
    
    [SerializeField, Tooltip("Scrolling speed increment for each parallax layer closer to the camera")]
    private float parallaxSpeedIncrement = 1f;
    
    
    void Start()
    {
        for (int i = 0; i < parallaxLayerRigidbodies.Length; ++i)
        {
            Rigidbody2D parallaxLayerRigidbody2D = parallaxLayerRigidbodies[i];
            // Player character is advancing to the right, so the background scrolls to the left
            parallaxLayerRigidbody2D.velocity = (parallaxSpeedBase + i * parallaxSpeedIncrement) * Vector2.left;
        }
    }
}
