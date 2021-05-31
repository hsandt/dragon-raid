using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

public class Brighten : ClearableBehaviour
{
    /* Property hashes */
    
    private readonly int brightnessPropertyID = Shader.PropertyToID("Brightness");
    
    
    /* Sibling components */

    private SpriteRenderer m_SpriteRenderer;


    /* Custom components */
    
    /// Timer counting down toward end of brightness effect
    private Timer m_BrightnessEndTimer;

    
    /* State */

    /// Current brightness to apply to sprite material
    private float m_Brightness;


    private void Awake()
    {
        m_SpriteRenderer = this.GetComponentOrFail<SpriteRenderer>();
        m_BrightnessEndTimer = new Timer(callback: ResetBrightness);
    }

    public override void Setup()
    {
        // initialise brightness in state and on sprite (do not use SetBrightness which does an old/new comparison)
        m_Brightness = 0f;
        RefreshSpriteBrightness();
    }

    private void Update()
    {
        m_BrightnessEndTimer.CountDown(Time.deltaTime);
    }

    /// Refresh sprite material brightness based on current brightness value
    private void RefreshSpriteBrightness()
    {
        m_SpriteRenderer.material.SetFloat(brightnessPropertyID, m_Brightness);
    }
    
    /// Set sprite material brightness
    public void SetBrightness(float brightness)
    {
        if (m_Brightness != brightness)
        {
            m_Brightness = brightness;
            RefreshSpriteBrightness();
        }
    }
    
    /// Reset brightness to 0 (original color)
    public void ResetBrightness()
    {
        SetBrightness(0f);
    }

    /// Set sprite material brightness for given duration
    /// Note that after duration, brightness is reset to 0 (original color) even if the previous brightness was not 0,
    /// and even if it was the same as the new brightness.
    public void SetBrightnessForDuration(float brightness, float duration)
    {
        SetBrightness(brightness);
        m_BrightnessEndTimer.SetTime(duration);
    }
}
