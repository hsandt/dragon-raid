using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for an enemy character
public class EnemyCharacterMaster : CharacterMaster
{
    protected override void AddSiblingSlaveBehaviours()
    {
        base.AddSiblingSlaveBehaviours();
        
        slaveBehaviours.Add(this.GetComponentOrFail<EnemyShootController>());
    }
}
