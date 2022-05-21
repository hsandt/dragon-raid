using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

public class ExtraLivesView : MonoBehaviour
{
    [Header("Assets")]

    [Tooltip("Life Icon Prefab")]
    public GameObject lifeIconPrefab;

    
    /// Tracked lives system
    private ExtraLivesSystem m_TrackedExtraLivesSystem;

    
    public void RegisterExtraLivesSystem(ExtraLivesSystem trackedExtraLivesSystem)
    {
        // Note: on Restart, the 2 next lines do nothing
        m_TrackedExtraLivesSystem = trackedExtraLivesSystem;
        m_TrackedExtraLivesSystem.RegisterObserver(this);
        RefreshView();
    }

    private void OnDestroy()
    {
        if (m_TrackedExtraLivesSystem != null)
        {
            m_TrackedExtraLivesSystem.UnregisterObserver(this);
        }
    }

    /// Refresh gauge to reflect value change
    /// Call it on Start after identifying the value to track, and on every value change.
    public void RefreshView()
    {
        UIPoolHelper.LazyInstantiateWidgets(lifeIconPrefab, m_TrackedExtraLivesSystem.GetRemainingExtraLives(), transform);
    }
}
