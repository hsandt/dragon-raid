using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;

using CommonsHelper;

/// Action to shoot a bullet pattern
[AddComponentMenu("Game/Action: Shoot Pattern")]
public class Action_ShootPattern : BehaviourAction
{
    [Header("Parameters data")]

    [InspectInline(canEditRemoteTarget = true)]
    public ShootPattern shootPattern;


    /* Owner sibling components */

    private Shoot m_Shoot;
    private ShootIntention m_ShootIntention;


    /* State */

    /// Time since action start, used to temporize the shots
    private float m_Time;

    /// Number of shots already ordered (set intention for them)
    /// The action is over when all shots have been ordered
    private int m_OrderedShotsCount;


    protected override void OnInit()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(shootPattern != null, this, "[Action_ShootPattern] OnInit: Shoot Pattern not set on {0}", this);
        #endif

        m_Shoot = m_EnemyCharacterMaster.GetComponentOrFail<Shoot>();
        m_ShootIntention = m_Shoot.ShootIntention;
    }

    public override void OnStart()
    {
        m_Time = 0f;
        m_OrderedShotsCount = 0;
    }

    public override void RunUpdate()
    {
        m_Time += Time.deltaTime;

        // From the time, determine the total number of shots we should have ordered and done by the end of the frame
        int shotsToOrderTotalCount;

        if (shootPattern.duration <= 0f)
        {
            // instant pattern: shoot all bullets at once
            shotsToOrderTotalCount = shootPattern.bulletCount;
        }
        else
        {
            // compute total progress over time
            // (clamp to avoid trying to shoot more bullets than requested by the pattern)
            float timeProgressRatio = Mathf.Min(m_Time / shootPattern.duration, 1f);

            // the first bullet is always shot at time 0, so start with 1
            // then add 1 bullet for every bullet time interval = shootPattern.duration / (shootPattern.bulletCount - 1)
            // until the last one is shot after shootPattern.duration
            shotsToOrderTotalCount = 1 + Mathf.FloorToInt(timeProgressRatio * (shootPattern.bulletCount - 1));
        }

        // only shoot new bullet(s) if a bullet time interval has pased since the start / the last bullet
        if (m_OrderedShotsCount < shotsToOrderTotalCount)
        {
            // set fire intention once, if there are multiple bullets to shoot, it will cover them all
            m_ShootIntention.fireOnce = true;

            Vector2 referenceDirection = Shoot.GetBaseFireDirection(shootPattern.shootDirectionMode, m_Shoot.shootAnchor);

            // iterate over bullets to shoot that have not been shot yet
            foreach (float fireAngle in ComputeFireAngles(m_OrderedShotsCount, shotsToOrderTotalCount))
            {
                m_ShootIntention.fireDirections.Add(VectorUtil.Rotate(referenceDirection, fireAngle));
            }

            // update the count of ordered shots
            m_OrderedShotsCount = shotsToOrderTotalCount;
        }
    }

    public IEnumerable<float> ComputeFireAngles(int startIndex, int endCount)
    {
        for (int i = startIndex; i < endCount; i++)
        {
            // compute progress ratio for this specific bullet to determine the shot angle
            float bulletProgressRatio;

            if (shootPattern.bulletCount > 1)
            {
                // directly compute ratio from index
                bulletProgressRatio = (float) i / (shootPattern.bulletCount - 1);
            }
            else
            {
                // we cannot divide by 0 so we have to pick a convention here:
                // we decide that a single bullet is always shot at Angle Start, hence ratio = 0
                bulletProgressRatio = 0f;
            }

            // Note we use Lerp so there is no wrapping around -180/180 and if angle start/end are close to those,
            // the resulting pattern will always spread on the wider arc, as often in shmups.
            // If you need to support spread fire behind the character, add a flag that enforces LerpAngle instead.
            // Finally, angle start and end are inclusive, which often desired, but when doing a full turn
            // from -180 to 180, the bullet at 180 will be shot twice. Consider passing -180 and 180 - interval
            // until we support exclusive angle end (by adding a flag).
            float angle = Mathf.Lerp(shootPattern.angleStart, shootPattern.angleEnd, bulletProgressRatio);
            yield return angle;
        }
    }

    protected override bool IsOver()
    {
        // action is over when all bullets have been shot (could be ==)
        return m_OrderedShotsCount >= shootPattern.bulletCount;
    }

    // no need to clear m_ShootIntention.fireDirections in OnEnd, as it is consumed by Shoot

    #if UNITY_EDITOR
    public override string GetNodeName()
    {
        return $"Shoot Pattern with {shootPattern.bulletCount} bullets over {shootPattern.duration} s";
    }
    #endif
}
