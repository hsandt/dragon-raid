using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventEffectOnDamage
{
    /// Callback to call when the event trigger associated to this event effect has fulfilled its condition
    /// Pass damage info
    void Trigger(DamageInfo damageInfo);
}
