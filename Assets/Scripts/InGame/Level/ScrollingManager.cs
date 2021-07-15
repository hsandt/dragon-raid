using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

public class ScrollingManager : SingletonManager<ScrollingManager>
{
    /* State */
    
    /// Default speed of scrolling, i.e. how fast spatial progress advances with time (m/s)
    private float m_ScrollingSpeed;
    
    /// Default speed of scrolling, i.e. how fast spatial progress advances with time (m/s) (getter)
    public float ScrollingSpeed => m_ScrollingSpeed;

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
        StartScrolling();
    }

    private void FixedUpdate()
    {
        if (m_ScrollingSpeed != 0f)
        {
            m_SpatialProgress += m_ScrollingSpeed * Time.deltaTime;
            SpatialEventManager.Instance.OnSpatialProgressChanged(m_SpatialProgress);
        }
    }

    public void StartScrolling()
    {
        // Always start at base scrolling speed
        m_ScrollingSpeed = InGameManager.Instance.LevelData.baseScrollingSpeed;
    }
    
    public void StopScrolling()
    {
        m_ScrollingSpeed = 0f;
    }

    public float ComputeTotalSpeedWithScrolling(float groundSpeed)
    {
        return - m_ScrollingSpeed + groundSpeed;
    }
}
