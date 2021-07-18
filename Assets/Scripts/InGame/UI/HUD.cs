using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

public class HUD : SingletonManager<HUD>
{
    [Header("Child references")]
    
    [Tooltip("Health gauge for player character")]
    public GaugeHealth gaugeHealthPlayer;

    
    /* Sibling components */

    private Canvas m_Canvas;
    
    
    protected override void Init()
    {
        base.Init();

        m_Canvas = this.GetComponentOrFail<Canvas>();
        
        // If no camera is set on Canvas, set it to Main
        // This allows us to define a standalone Canvas HUD prefab that doesn't need any external scene reference
        // to the Main Camera, that cannot be saved in the prefab. But you can still set the reference manually
        // on prefab instance for direct access.
        if (m_Canvas.worldCamera == null)
        {
            m_Canvas.worldCamera = Camera.main;
        }
    }

    public void AssignGaugeHealthPlayerTo(HealthSystem healthSystem)
    {
        gaugeHealthPlayer.RegisterHealthSystem(healthSystem);
    }
}