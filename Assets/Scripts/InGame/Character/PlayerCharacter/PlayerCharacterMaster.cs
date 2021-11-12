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
        // Clear reference to player character controller to avoid unwanted control during death
        // that may set sticky intention that would be interpreted just after respawn
        // InGameInputManager.Instance.SetPlayerCharacterController(null);

        if (m_ExtraLivesSystem.GetRemainingExtraLives() > 0)
        {
            m_ExtraLivesSystem.LoseLife();
            InGameManager.Instance.RespawnPlayerCharacterAfterDelay();
        }
        else
        {
            InGameManager.Instance.PlayGameOverRestartSequence();
        }
    }
}
