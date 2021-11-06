using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for player character
public class PlayerCharacterMaster : CharacterMaster
{
    /* Sibling components */

    private ExtraLivesSystem m_ExtraLivesSystem;


    protected override void Init()
    {
        base.Init();
        
        m_ExtraLivesSystem = this.GetComponentOrFail<ExtraLivesSystem>();
    }

    public override void OnDeathOrExit()
    {
        if (m_ExtraLivesSystem.GetRemainingExtraLives() > 0)
        {
            m_ExtraLivesSystem.LoseLife();
            InGameManager.Instance.RespawnPlayerCharacterAfterDelay();
        }
        else
        {
            InGameManager.Instance.PlayGameOverRestart();
        }
    }
}
