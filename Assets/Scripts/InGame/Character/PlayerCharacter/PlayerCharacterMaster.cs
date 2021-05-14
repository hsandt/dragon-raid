using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for player character
public class PlayerCharacterMaster : CharacterMaster
{
    protected override void AddSiblingSlaveBehaviours()
    {
        base.AddSiblingSlaveBehaviours();
        
        slaveBehaviours.Add(this.GetComponentOrFail<PlayerMoveController_Flying>());
    }
}
