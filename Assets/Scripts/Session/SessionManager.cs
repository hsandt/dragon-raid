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
        currentPlayerSaveStory = new PlayerSaveStory();
    }

    public void StartNewSaveArcade()
    {
        currentPlayerSaveArcade = new PlayerSaveArcade();
    }

    public void ClearAnyCurrentSave()
    {
        currentPlayerSaveStory = null;
        currentPlayerSaveArcade = null;
    }
}
