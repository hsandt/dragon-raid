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

    /// Array of references to the current left and right parallax layer transforms, for each level.
    /// All left layers are the original ones on start, but they swap with their duplicate every full scrolling period.
    /// However, this period depends on the parallax level, so we must track them independently for each level.
    /// Note that C# System.Tuple is immutable, so we use our custom Pair struct instead.
    /// Also note that we store Transform instead of Rigidbody2D just because we only use those to warp position
    /// directly, and warping is more reliable with Transform (avoids lag when done outside FixedUpdate context).
    private Pair<Transform, Transform>[] leftAndRightParallaxLayerTransforms;

    
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
        leftAndRightParallaxLayerTransforms = new Pair<Transform, Transform>[parallaxLayerRigidbodies.Length];
        
        // Create a duplicate for this parallax layer so we can have it loop continuously by swapping 
        // the left and right copies when the left one has completely exited the screen to the left.
        // We iterate forward so the rendering layer order is the same as the original: farthest, then closest layers.
        for (int i = 0; i < parallaxLayerRigidbodies.Length; ++i)
        {
            // Place the duplicate under Background, just on the right of the original, i.e. 1 screen width to the right
            // of the original (which is at the origin). Note that for more variety we may want to avoid
            // looping over one screen width. If so, just make wider parallax layers and chain them with
            // more than one screen width of interval on X.
            Vector2 duplicatePosition = skySpriteRenderer.size.x * Vector2.right;
            m_DuplicateParallaxLayerRigidbodies[i] = Instantiate(parallaxLayerRigidbodies[i], duplicatePosition,
                Quaternion.identity, transform);

            // Also initialize reference pair: original parallax layer starts on the left, duplicate on the right
            leftAndRightParallaxLayerTransforms[i].First = parallaxLayerRigidbodies[i].transform;
            leftAndRightParallaxLayerTransforms[i].Second = m_DuplicateParallaxLayerRigidbodies[i].transform;
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
            Transform leftParallaxLayerTransform = leftAndRightParallaxLayerTransforms[i].First;
            Transform rightParallaxLayerTransform = leftAndRightParallaxLayerTransforms[i].Second;

            if (rightParallaxLayerTransform.position.x <= 0f)
            {
                // the left parallax layer stops covering the right part of the screen, warp the original layer
                // just to the right of the right layer for perfect looping
                float warpedPositionX = rightParallaxLayerTransform.position.x + skySpriteRenderer.size.x;
                
                // position y is always 0
                leftParallaxLayerTransform.position = warpedPositionX * Vector2.right;
                
                // swap both roles now, we start another scrolling loop for this layer level
                Transform tempParallaxLayerTransform = leftParallaxLayerTransform;
                leftAndRightParallaxLayerTransforms[i].First = rightParallaxLayerTransform;
                leftAndRightParallaxLayerTransforms[i].Second = tempParallaxLayerTransform;
            }
        }
    }
}
