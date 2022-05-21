using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;
using UnityConstants;

/// Spatial Event Manager
/// System for Spatial Event Trigger x Event Effect
/// SEO: after LocatorManager
public class SpatialEventManager : SingletonManager<SpatialEventManager>
{
    /* Cached scene references */
    
    /// List of pairs (spatial event trigger, event effect) found in the scene
    /// This includes all enemy wave spatial events.
    private readonly List<Pair<EventTrigger_SpatialProgress, IEventEffect>> m_AllSpatialEventPairs = new List<Pair<EventTrigger_SpatialProgress, IEventEffect>>();

    
    /* State */

    /// List of pairs (spatial event trigger, event effect) found in the scene
    /// and still unprocessed (not triggered yet). This includes remaining enemy wave spatial events.
    private List<Pair<EventTrigger_SpatialProgress, IEventEffect>> m_RemainingSpatialEventPairs;

    
    protected override void Init()
    {
        // Find all Spatial Event Triggers in the scene, then find any associated effect
        // We dropped the ECS-tag-component approach and prefer a classic interface approach with IEventEffect,
        // so we can move handling code to each of the event effect classes
        GameObject spatialEventsParent = LocatorManager.Instance.FindWithTag(Tags.SpatialEvents);
        var allSpatialEventTriggers = spatialEventsParent.GetComponentsInChildren<EventTrigger_SpatialProgress>();
        
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
                    "EventTrigger_SpatialProgress {0}, it will do nothing even when fulfilled", spatialEventTrigger);
            }
            #endif
        }
    }

    /// Setup is managed by InGameManager, so no need to call it in this script's Start
    public void Setup()
    {
        // Reset list of unprocessed event pairs
        m_RemainingSpatialEventPairs = m_AllSpatialEventPairs.ToList();
    }

    public void OnSpatialProgressChanged(float oldSpatialProgress, float newSpatialProgress)
    {
        // Spatial progress has changed, check for any spatial event trigger condition being fulfilled
        // (we don't do this in FixedUpdate so we do no work while scrolling is paused)
        
        // Do a reverse iteration so we can remove event pairs by index safely
        
        // OPTIMIZATION: when we have many events (esp. waves), this will be slow at the beginning
        // where we have not removed many event pairs (O(N) for N event pairs)
        // In this case, it's better to enforce convention on level design that all event objects
        // are ordered chronologically, so we can only check the next one i.e. the first in the
        // remaining list
        for (int i = m_RemainingSpatialEventPairs.Count - 1; i >= 0; i--)
        {
            var eventPair = m_RemainingSpatialEventPairs[i];
            EventTrigger_SpatialProgress eventTrigger = eventPair.First;
            IEventEffect eventEffect = eventPair.Second;

            if (eventTrigger.RequiredSpatialProgress <= newSpatialProgress)
            {
                // Note that we're now checking for less than *or equal* to support events at time = 0,
                // and trigger events at time = T when cheat-warping to time T exactly.
                // This seems redundant with the comparison above, as if on one frame we fall exactly on
                // RequiredSpatialProgress, next frame we would enter this block again.
                // But since we Remove the event below, we guarantee not to check this event again,
                // so there is no risk to Trigger it again.
                if (oldSpatialProgress <= eventTrigger.RequiredSpatialProgress)
                {
                    // We moved from old to new progress, going through (or just reaching) the required spatial progress
                    // so trigger the event effect.
                    // Check for game flow blocking spatial progress events too. While events triggered by specific
                    // gameplay events like character death are easily prevented by ignoring all damage during special
                    // game flow sequences, scrolling tends to continue, even during sequences like game over,
                    // so we must prevent random events from being triggered because of that.
                    // Of course, this means we'll be missing on possibly important events like finish level,
                    // but CanTriggerSpatialProgressEvent is only true when game is paused (in which case scrolling is
                    // paused), or level will be soon restarted/finished, so this is okay.
                    // Design note: we could tolerate minor events like enemies or projectiles spawning.
                    // In this case, either (a) remove this check and use a Try... method inside each Trigger implementation
                    // of an major event, or (b) add IEventEffect interface method CanTriggerDuringGameFlowSequence
                    // with only minor event effects returning true.
                    if (InGameManager.Instance.CanTriggerSpatialProgressEvent)
                    {
                        eventEffect.Trigger();
                    }
                }
                
                // Either we triggered the event effect, or the required spatial progress was less than
                // the old progress, which means we were already past the event yet it wasn't removed (only possible
                // when using CheatAdvanceScrolling). In both cases, remove the event to avoid processing it again
                // (it also avoids a duplicate Trigger if one a given frame, we fall right on RequiredSpatialProgress
                // as explained above)
                m_RemainingSpatialEventPairs.RemoveAt(i);
            }
        }
    }
}
