using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;
using UnityConstants;

/// Spatial Event Manager
/// System for Spatial Event Trigger x Enemy Wave/Event Finish Level
public class SpatialEventManager : SingletonManager<SpatialEventManager>
{
    /* Cached scene references */
    
    /// List of pairs (spatial event trigger, event effect) found in the scene
    /// This includes all enemy wave events.
    private readonly List<Pair<SpatialEventTrigger, IEventEffect>> m_AllSpatialEventPairs = new List<Pair<SpatialEventTrigger, IEventEffect>>();

    
    /* State */

    /// List of pairs (spatial event trigger, event effect) found in the scene
    /// and not triggered yet. This includes remaining enemy wave events.
    private List<Pair<SpatialEventTrigger, IEventEffect>> m_RemainingSpatialEventPairs;

    
    protected override void Init()
    {
        // Find all Spatial Event Triggers in the scene, then find any associated effect
        // We dropped the ECS-tag-component approach and prefer a classic interface approach with IEventEffect,
        // so we can move handling code to each of the event effect classes
        GameObject spatialEventsParent = LocatorManager.Instance.FindWithTag(Tags.SpatialEvents);
        var allSpatialEventTriggers = spatialEventsParent.GetComponentsInChildren<SpatialEventTrigger>();
        
        foreach (var spatialEventTrigger in allSpatialEventTriggers)
        {
            var eventEffect = spatialEventTrigger.GetComponent<IEventEffect>();
            if (eventEffect != null)
            {
                // Store pair (event trigger, effect) to quickly access both later
                // so we can simulate a 2-component iteration, ECS-style
                m_AllSpatialEventPairs.Add(Pair.Create(spatialEventTrigger, eventEffect));
            }
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            else
            {
                Debug.LogWarningFormat(spatialEventTrigger, "No event effect (IEventEffect) component found along " +
                    "SpatialEventTrigger {0}, it will do nothing even when fulfilled", spatialEventTrigger);
            }
            #endif
        }
    }

    /// Setup is managed by InGameManager, so not called on Start
    public void Setup()
    {
        // Reset all enemy waves
        m_RemainingSpatialEventPairs = m_AllSpatialEventPairs.ToList();
    }

    public void OnSpatialProgressChanged(float spatialProgress)
    {
        // Spatial progress has changed, check for any spatial event trigger condition being fulfilled
        // (we don't do this in FixedUpdate so we do no work while scrolling is paused)
        
        // Do a reverse iteration so we can remove event pairs by index safely
        
        // OPTIMIZATION: when we have many events (esp. waves), this will start getting slow
        // in this case, it's better to enforce convention on level design that all event objects
        // are ordered chronologically, so we can only check the next one i.e. the first in the
        // remaining list
        for (int i = m_RemainingSpatialEventPairs.Count - 1; i >= 0; i--)
        {
            var eventPair = m_RemainingSpatialEventPairs[i];
            SpatialEventTrigger eventTrigger = eventPair.First;
            IEventEffect eventEffect = eventPair.Second;
            
            if (spatialProgress >= eventTrigger.RequiredSpatialProgress)
            {
                eventEffect.Trigger();
                m_RemainingSpatialEventPairs.RemoveAt(i);
            }
        }
    }
}