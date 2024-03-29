using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

public class ExtraLivesSystem : ClearableBehaviour
{
    [Header("Parameters data")]
    
    [Tooltip("Extra Lives Parameters Data")]
    public ExtraLivesParameters extraLivesParameters;

    
    /* Dynamic external references */
    
    /// List of lives views observing lives data
    private readonly List<ExtraLivesView> m_LivesViewList = new List<ExtraLivesView>();
    
    
    /* Sibling components */

    private ExtraLives m_ExtraLives;

    
        
    private void Awake()
    {
        Debug.AssertFormat(extraLivesParameters != null, this, "[ExtraLivesSystem] No Extra Lives Parameters asset set on {0}", this);

        m_ExtraLives = this.GetComponentOrFail<ExtraLives>();
        m_ExtraLives.maxCount = extraLivesParameters.maxExtraLivesCount;
    }

    /// Initialize extra lives to max count
    /// Exceptionally called manually and not done in Setup, to avoid resetting the number of extra lives
    /// each time the player character dies and respawn, which would defeat the concept of extra lives
    public void InitExtraLives()
    {
        m_ExtraLives.count = m_ExtraLives.maxCount;
        
        // It is optional to notify observers here if this is called before HUD.AssignExtraLivesViewTo
        // in InitialSpawnPlayerCharacter, but required if called after. For safety, we call it.
        NotifyValueChangeToObservers();
    }

    public int GetRemainingExtraLives()
    {
        return m_ExtraLives.count;
    }

    public void LoseLife()
    {
        if (m_ExtraLives.count > 0)
        {
            m_ExtraLives.count--;
            NotifyValueChangeToObservers();
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(this, "[LivesSystem] No life left on {0}, cannot Lose Life.", this);
        }
        #endif
    }
    
    
    /* Observer pattern */
    
    public void RegisterObserver(ExtraLivesView extraLivesView)
    {
        if (!m_LivesViewList.Contains(extraLivesView))
        {
            m_LivesViewList.Add(extraLivesView);
        }
    }
    
    public void UnregisterObserver(ExtraLivesView extraLivesView)
    {
        if (m_LivesViewList.Contains(extraLivesView))
        {
            m_LivesViewList.Remove(extraLivesView);
        }
    }
    
    private void NotifyValueChangeToObservers()
    {
        foreach (ExtraLivesView livesView in m_LivesViewList)
        {
            livesView.RefreshView();
        }
    }
}
