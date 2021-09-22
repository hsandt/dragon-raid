using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

public class ScrollingManager : SingletonManager<ScrollingManager>
{
    [Header("Parameters")]
    
    [SerializeField, Tooltip("Visual left limit X: Environment Props that go beyond the left edge of the screen by more than " +
             "this value - their defined half width (+ small margin for safety) can be removed from the game. " +
             "You can either set this to the actual fixed screen half-width in meters, signed (-10), or the LivingZone " +
             "- Box Collider 2D width / 2 (-10.5) to match gameplay living zone.")]
    private float visualLeftLimitX = -10f;
    public float VisualLeftLimitX => visualLeftLimitX;
    
    
    /* Cached scene references */
    
    /// Cached background reference
    private Background m_Background;

    
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
    
    
    protected override void Init()
    {
        base.Init();

        m_Background = LocatorManager.Instance.FindWithTag(Tags.Background)?.GetComponent<Background>();
        
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(m_Background != null, "[ScrollingManager] Could not find active Background object > Background component", this);
        #endif
    }
    
    /// Setup is managed by InGameManager, so no need to call it in this script's Start
    public void Setup()
    {
        m_SpatialProgress = 0f;
        
        // Always start at base scrolling speed
        StartScrollingAtLevelNormalSpeed();
    }

    public void Pause()
    {
        enabled = false;
        m_Background.Pause();          // the important part, it will set parallax velocities to zero
        m_Background.enabled = false;  // optional, this will just stop checking parallax layer wrapping (which can't happen when not moving anyway)
    }

    public void Resume()
    {
        // We assume we didn't change scrolling speed while paused, so no need to RefreshBackgroundScrollingSpeed
        enabled = true;
        m_Background.Resume();
        m_Background.enabled = true;
    }

    private void FixedUpdate()
    {
        if (m_ScrollingSpeed > 0f)
        {
            AdvanceScrolling(m_ScrollingSpeed * Time.deltaTime);
        }
    }
        
    /// Advance scrolling by offset and notify SpatialEventManager of progress change
    public void AdvanceScrolling(float offset)
    {
        if (offset > 0)
        {
            SpatialEventManager.Instance.OnSpatialProgressChanged(m_SpatialProgress, m_SpatialProgress + offset);
            m_SpatialProgress += offset;
        }
    }
    
    #if UNITY_EDITOR || DEVELOPMENT_BUILD
    public void CheatAdvanceScrolling(float offset)
    {
        // Do not test for positive offset to allow going back if needed for testing
        // Do not notify spatial progress to avoid triggering a bunch of enemy waves when doing a big advance
        // Caution: other events like StopScrolling will not be triggered either, which may cause inconsistent state
        // such as keeping scrolling before a boss (so avoid warping right into a peculiar zone of the level)
        // OnSpatialProgressChanged now takes the old spatial progress, so it will not trigger all the events
        // we skipped on next FixedUpdate.
        m_SpatialProgress += offset;
    }
    #endif

    /// Notify Background of scrolling speed change, as ScrollingManager manages Background
    private void RefreshBackgroundScrollingSpeed()
    {
        m_Background.SetScrollingSpeedAndUpdateVelocity(m_ScrollingSpeed);
    }

    public void StartScrollingAtLevelNormalSpeed()
    {
        Debug.Assert(enabled, "[ScrollingManager] StartScrollingAtLevelNormalSpeed: should never be called while disabled", this);
        
        m_ScrollingSpeed = InGameManager.Instance.LevelData.baseScrollingSpeed;
        RefreshBackgroundScrollingSpeed();
    }

    public void StopScrolling()
    {
        Debug.Assert(enabled, "[ScrollingManager] StopScrolling: should never be called while disabled", this);

        // True scrolling stop in the game world, unlike Pause it really resets scrolling speed for both this script and
        // Background.
        // It's important to distinguish this from Pause behavior. For instance, if we were only setting
        // m_Background.enabled = false, then if player Pause and Resume game, it would resume background scrolling
        // with the old speed!
        m_ScrollingSpeed = 0f;
        RefreshBackgroundScrollingSpeed();
    }

    public float ComputeTotalSpeedWithScrolling(float groundSpeed)
    {
        return - m_ScrollingSpeed + groundSpeed;
    }
    
    public Rigidbody2D GetMidgroundLayer()
    {
        return m_Background.midgroundLayerRigidbody;
    }
}
