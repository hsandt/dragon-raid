using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// Handles background parallax scrolling
public class Background : MonoBehaviour
{
    [Tooltip("Sprite renderer of sky, useful to determine screen size and place parallax layer duplicates")]
    public SpriteRenderer skySpriteRenderer;
    
    [Tooltip("Array of rigidbodies of parallax layers, from farthest to closest")]
    public Rigidbody2D[] parallaxLayerRigidbodies;

    [SerializeField, Tooltip("Scrolling speed of the farthest parallax layer")]
    private float parallaxSpeedBase = 1f;
    
    [SerializeField, Tooltip("Scrolling speed increment for each parallax layer closer to the camera")]
    private float parallaxSpeedIncrement = 1f;

    /// Array of duplicates of rigidbodies of parallax layers, from farthest to closest, swapped with the original ones
    /// for continuous loop
    private Rigidbody2D[] m_DuplicateParallaxLayerRigidbodies;
    
    
    void Start()
    {
        InstantiateDuplicateParallaxLayers();
        SetupAllParallaxLayersVelocity();
    }
    
    
    private void InstantiateDuplicateParallaxLayers()
    {
        // Create a duplicate for each parallax layer so we can have them loop continuously by swapping 
        // the left and right copies when the left one has completely exited the screen to the left.
        // We iterate forward so the rendering layer order is the same as the original: farthest, then closest layers.
        m_DuplicateParallaxLayerRigidbodies = new Rigidbody2D[parallaxLayerRigidbodies.Length];
        for (int i = 0; i < parallaxLayerRigidbodies.Length; ++i)
        {
            // Place the duplicate under Background, just on the right of the original, i.e. 1 screen width to the right
            // of the original (which is at the origin). Note that for more variety we may want to avoid
            // looping over one screen width. If so, just make wider parallax layers and chain them with
            // more than one screen width of interval on X.
            Vector2 duplicatePosition = skySpriteRenderer.size.x * Vector2.right;
            m_DuplicateParallaxLayerRigidbodies[i] = Instantiate(parallaxLayerRigidbodies[i], duplicatePosition,
                Quaternion.identity, transform);
        }
    }

    private void SetupAllParallaxLayersVelocity()
    {
        for (int i = 0; i < parallaxLayerRigidbodies.Length; ++i)
        {
            Rigidbody2D parallaxLayerRigidbody2D = parallaxLayerRigidbodies[i];
            Rigidbody2D duplicateParallaxLayerRigidbody2D = m_DuplicateParallaxLayerRigidbodies[i];

            // Player character is advancing to the right, so the background scrolls to the left
            parallaxLayerRigidbody2D.velocity = (parallaxSpeedBase + i * parallaxSpeedIncrement) * Vector2.left;
            duplicateParallaxLayerRigidbody2D.velocity = (parallaxSpeedBase + i * parallaxSpeedIncrement) * Vector2.left;
        }
    }
}
