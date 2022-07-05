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

    [Tooltip("Health gauge for boss (if any)")]
    public GaugeHealth gaugeBoss;

    [Tooltip("Extra Lives view")]
    public ExtraLivesView extraLivesView;


    /* Sibling components */

    private Canvas m_Canvas;


    protected override void Init()
    {
        base.Init();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(gaugeHealthPlayer != null, this, "[HUD] Awake: Gauge Health Player not set on {0}", this);
        Debug.AssertFormat(gaugeBoss != null, this, "[HUD] Awake: Gauge Boss not set on {0}", this);
        Debug.AssertFormat(extraLivesView != null, this, "[HUD] Awake: Extra Lives View not set on {0}", this);
        #endif

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

    public void Setup()
    {
        // Hide boss health gauge until it appears
        gaugeBoss.gameObject.SetActive(false);
    }

    public void AssignGaugeHealthPlayerTo(HealthSystem healthSystem)
    {
        gaugeHealthPlayer.RegisterHealthSystem(healthSystem);
    }

    public void ShowAndAssignGaugeBossTo(HealthSystem healthSystem)
    {
        gaugeBoss.gameObject.SetActive(true);
        gaugeBoss.RegisterHealthSystem(healthSystem);
    }

    public void HideAndUnassignGaugeBoss()
    {

        gaugeBoss.UnregisterHealthSystem();
        gaugeBoss.gameObject.SetActive(false);
    }

    public void AssignExtraLivesViewTo(ExtraLivesSystem extraLivesSystem)
    {
        extraLivesView.RegisterExtraLivesSystem(extraLivesSystem);
    }
}