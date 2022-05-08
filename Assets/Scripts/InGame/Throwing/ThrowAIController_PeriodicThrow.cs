using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

using CommonsHelper;
using CommonsPattern;

/// System for Throw: handles AI control
/// It manages ThrowIntention and is therefore responsible for its Setup.
/// SEO for concrete child classes: before Throw
[AddComponentMenu("Game/Throw AI Controller: Periodic Throw")]
public class ThrowAIController_PeriodicThrow : ClearableBehaviour
{
    [Header("Parameters data")]

    [Tooltip("Periodic Throw AI Parameters Data")]
    [InspectInline(canEditRemoteTarget = true)]
    public PeriodicThrowAIParameters periodicThrowAiParameters;


    /* Sibling components */

    private ThrowIntention m_ThrowIntention;


    /* State */

    private Timer m_ThrowTimer;

    private void Awake()
    {
        m_ThrowIntention = this.GetComponentOrFail<ThrowIntention>();
        m_ThrowTimer = new Timer(callback: OrderThrow);
    }

    public override void Setup()
    {
        m_ThrowTimer.SetTime(periodicThrowAiParameters.initialDelay);
    }

    private void FixedUpdate()
    {
        m_ThrowTimer.CountDown(Time.deltaTime);
    }

    private void OrderThrow()
    {
        m_ThrowIntention.startThrow = true;

        // angle is CW, so we rotate by -angle
        m_ThrowIntention.throwDirection = VectorUtil.Rotate(Vector2.left, -periodicThrowAiParameters.angle);
        m_ThrowIntention.throwSpeed = periodicThrowAiParameters.throwSpeed;

        // reset timer to prepare next throw)
        m_ThrowTimer.SetTime(periodicThrowAiParameters.period);
    }
}
