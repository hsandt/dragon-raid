using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEventEffect
{
    /// Callback to call when the event trigger associated to this event effect has fulfilled its condition
    void Trigger();
}
