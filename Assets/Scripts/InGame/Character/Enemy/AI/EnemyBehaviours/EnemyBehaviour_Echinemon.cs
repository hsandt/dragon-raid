using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Behaviour for Enemy: Echinemon
/// Echinemon has very simple logic already handled by low-level controllers,
/// so this script is only for aesthetics: to spawn soil lump and animate it properly.
/// SEO: after ScrollingManager just to access Midground Layer via ScrollingManager > Background at Awake time
/// (otherwise, need to set parent later)
[AddComponentMenu("Game/Enemy Behaviour: Echinemon")]
public class EnemyBehaviour_Echinemon : ClearableBehaviour
{
    /* Animator hashes */

    private static readonly int holeHash = Animator.StringToHash("Hole");
    
    
    [Header("Assets")]
    
    [Tooltip("Prefab for soil")]
    public GameObject soilPrefab;

    
    /* Sibling components */
    
    private SpriteRenderer m_EchinemonSpriteRenderer;

    
    /* Instantiated object components */
    
    private Animator m_SoilAnimator;

    
    /* State */

    private Timer m_StickHeadOutTimer;
    
    
    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(soilPrefab, this, "[EnemyBehaviour_Echinemon] Soil Prefab on Enemy Behaviour: Echinemon component {0}", this);
        #endif

        m_EchinemonSpriteRenderer = this.GetComponentOrFail<SpriteRenderer>();

        // Instantiate Soil object on midground layer so it follows scrolling perfectly
        // This requires particular SEO
        // Note that while this doesn't seem to use the Pool Pattern (no direct Pool Manager for the Soil objects),
        // Echinemon enemies themselves should be pooled, effectively creating as many soil objects, making them
        // pseudo-pooled objects with an actual PooledVisual script so they can be removed by LivingZoneTracker
        GameObject soilObject = Instantiate(soilPrefab, ScrollingManager.Instance.GetMidgroundLayer().transform);
        m_SoilAnimator = soilObject.GetComponentOrFail<Animator>();
        soilObject.SetActive(false);
        
        m_StickHeadOutTimer = new Timer(callback: StickHeadOut);
    }
    
    public override void Setup()
    {
        // Hide Echinemon at first
        m_EchinemonSpriteRenderer.enabled = false;
        
        // Show Soil Lump where Echinemon's head is instead
        m_SoilAnimator.gameObject.SetActive(true);
        m_SoilAnimator.transform.position = transform.position;
        m_SoilAnimator.SetBool(holeHash, false);
        
        // Prepare timer to stick head out (hardcoded for now)
        m_StickHeadOutTimer.SetTime(1f);
    }

    private void FixedUpdate()
    {
        // Count down timer to stick head out
        // Note: if player character is close to screen's right edge, Echinemon may jump out of hole
        // before even sticking head out, causing it to not show its sprite and not showing the hole early enough...
        // Consider tracking the jump by adding some property HasJumped to MoveGrounded, using an event,
        // or overriding MoveGrounded itself for full control.
        // It looks like timer is not really good as it starts counting down even when enemy is out of view,
        // so consider checking if enemy is visible on screen, or reveal enemy when close to player character.
        m_StickHeadOutTimer.CountDown(Time.deltaTime);
    }

    private void StickHeadOut()
    {
        // Show Echinemon sprite and Soil Hole
        m_EchinemonSpriteRenderer.enabled = true;
        m_SoilAnimator.SetBool(holeHash, true);
    }

    // Do nothing on Clear, Soil is now independent from this object and will not be released until exiting
    // LivingZoneTracker
}
