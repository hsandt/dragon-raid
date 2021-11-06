using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for player character
public class PlayerCharacterMaster : CharacterMaster
{
    public override void OnDeathOrExit()
    {
        InGameManager.Instance.PlayGameOverRestart();
    }
}
