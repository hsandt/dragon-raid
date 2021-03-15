using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

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

    /// Array of references to the current left and right parallax layers, for each level.
    /// All left layers are the original ones on start, but they swap with their duplicate every full scrolling period.
    /// However, this period depends on the parallax level, so we must track them independently for each level.
    /// Note that C# System.Tuple is immutable, so we use our custom Pair struct instead.
    private Pair<Rigidbody2D, Rigidbody2D>[] leftAndRightParallaxLayerRigidbodies;

    
    void Start()
    {
        InstantiateDuplicateParallaxLayers();
        SetupAllParallaxLayersVelocity();
    }
    
    private void InstantiateDuplicateParallaxLayers()
    {
        // Create duplicate array to fill
        m_DuplicateParallaxLayerRigidbodies = new Rigidbody2D[parallaxLayerRigidbodies.Length];
        
        // Construct array of pairs too
        leftAndRightParallaxLayerRigidbodies = new Pair<Rigidbody2D, Rigidbody2D>[parallaxLayerRigidbodies.Length];
        
        for (int i = 0; i < parallaxLayerRigidbodies.Length; ++i)
        {
            // Create a duplicate for this parallax layer so we can have it loop continuously by swapping 
            // the left and right copies when the left one has completely exited the screen to the left.
            // We iterate forward so the rendering layer order is the same as the original: farthest, then closest layers.
            
            // Place the duplicate under Background, just on the right of the original, i.e. 1 screen width to the right
            // of the original (which is at the origin). Note that for more variety we may want to avoid
            // looping over one screen width. If so, just make wider parallax layers and chain them with
            // more than one screen width of interval on X.
            Vector2 duplicatePosition = skySpriteRenderer.size.x * Vector2.right;
            m_DuplicateParallaxLayerRigidbodies[i] = Instantiate(parallaxLayerRigidbodies[i], duplicatePosition,
                Quaternion.identity, transform);

            // original parallax layer starts on the left, duplicate on the right
            leftAndRightParallaxLayerRigidbodies[i].First = parallaxLayerRigidbodies[i];
            leftAndRightParallaxLayerRigidbodies[i].Second = m_DuplicateParallaxLayerRigidbodies[i];
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

    private void Update()
    {
        // Check for each left parallax layer exiting screen to the left, which is equivalent to right parallax
        // layer entering and covering the screen completely, which is equivalent to its position crossing 0 toward left
        for (int i = 0; i < parallaxLayerRigidbodies.Length; ++i)
        {
            Rigidbody2D leftParallaxLayerRigidbody2D = leftAndRightParallaxLayerRigidbodies[i].First;
            Rigidbody2D rightParallaxLayerRigidbody2D = leftAndRightParallaxLayerRigidbodies[i].Second;

            if (rightParallaxLayerRigidbody2D.position.x <= 0f)
            {
                // the duplicate parallax layer stops covering the right part of the screen, warp the original layer
                // there to loop
                // there may be a small accumulation of floating errors, it's hard to tell
                // if you notice the two layers getting far from each other, or overlapping each other
                // after a long play time, prefer a solution that sets position relatively to the other layer
                leftParallaxLayerRigidbody2D.position += 2 * skySpriteRenderer.size.x * Vector2.right;
                
                // swap both roles now, we start another scrolling loop for this layer level
                Rigidbody2D tempParallaxLayerRigidbody = leftAndRightParallaxLayerRigidbodies[i].First;
                leftAndRightParallaxLayerRigidbodies[i].First = leftAndRightParallaxLayerRigidbodies[i].Second;
                leftAndRightParallaxLayerRigidbodies[i].Second = tempParallaxLayerRigidbody;
            }
        }
    }
}
