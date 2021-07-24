using System.Collections;
using System.Collections.Generic;
using CommonsPattern;
using UnityEngine;

public class SessionManager : SingletonManager<SessionManager>
{
    /* State */

    /// Current Player Save for Story mode, or null if not playing Story mode
    private PlayerSaveStory currentPlayerSaveStory;

    /// Current Player Save for Arcade mode, or null if not playing Arcade mode
    private PlayerSaveArcade currentPlayerSaveArcade;

    
    public void StartNewSaveStory()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(currentPlayerSaveStory == null, "[SessionManager] StartNewSaveStory: currentPlayerSaveStory is not null, it will be replaced with new story save and garbage-collected");
        Debug.Assert(currentPlayerSaveArcade == null, "[SessionManager] StartNewSaveStory: currentPlayerSaveArcade is not null, it will coexist with the new currentPlayerSaveStory");
        #endif
        currentPlayerSaveStory = new PlayerSaveStory();
    }

    public void StartNewSaveArcade()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.Assert(currentPlayerSaveArcade == null, "[SessionManager] StartNewSaveArcade: currentPlayerSaveArcade is not null, it will be replaced with new arcade save and garbage-collected");
        Debug.Assert(currentPlayerSaveStory == null, "[SessionManager] StartNewSaveStory: currentPlayerSaveStory is not null, it will coexist with the new currentPlayerSaveArcade");
        #endif
        currentPlayerSaveArcade = new PlayerSaveArcade();
    }

    public void SaveProgressNextLevel(int nextLevelIndex)
    {
        if (currentPlayerSaveStory != null)
        {
            currentPlayerSaveStory.nextLevelIndex = nextLevelIndex;
        }
        else if (currentPlayerSaveArcade != null)
        {
            currentPlayerSaveArcade.nextLevelIndex = nextLevelIndex;
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogError("[SessionManager] SaveProgressNextLevel: no current player save for Story nor Arcade, nothing to save");
        }
        #endif
    }

    public void ClearAnyCurrentSave()
    {
        currentPlayerSaveStory = null;
        currentPlayerSaveArcade = null;
    }
}
