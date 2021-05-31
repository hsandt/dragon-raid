using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

public class ScrollingManager : SingletonManager<ScrollingManager>
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Default speed of scrolling, i.e. how fast spatial progress advances with time (m/s)")]
    private float m_ScrollingSpeed = 1f;
    
    /// Default speed of scrolling, i.e. how fast spatial progress advances with time (m/s) (getter)
    private float ScrollingSpeed => m_ScrollingSpeed;
    
    /* State */

    /// How much level midground (gameplay plane) was scrolled since level start
    /// This is broadly proportional to time spent since level start, but takes into account
    /// any scrolling slowdown or pause (e.g. when scrolling stops during mandatory gate blast).
    private float m_SpatialProgress;
    
    /// How much level midground (gameplay plane) was scrolled since level start (getter)
    public float SpatialProgress => m_SpatialProgress;
    
    /// Setup is managed by InGameManager, so not called on Start
    public void Setup()
    {
        m_SpatialProgress = 0f;
    }

    private void FixedUpdate()
    {
        m_SpatialProgress += m_ScrollingSpeed * Time.deltaTime;
        SpatialEventManager.Instance.OnSpatialProgressChanged(m_SpatialProgress);
    }
}
