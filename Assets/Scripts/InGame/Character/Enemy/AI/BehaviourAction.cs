using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourAction : MonoBehaviour
{
    /// Action start callback. Useful to setup and cache info (optional)
    public virtual void OnStart() {}

    /// Run Update callback (called every frame while action is active)
    public abstract void RunUpdate ();
    
    /// Return true iff action is over and we should proceed to next action when executed inside a sequence
    /// (must be called after Update)
    public abstract bool IsOver();
        
    /// Action end callback. Useful to clear any remaining continuous intention not automatically consumed
    /// (optional)
    public virtual void OnEnd() {}
        
    #if UNITY_EDITOR
    /// Return estimated duration of an action having this end condition.
    /// For event-based conditions, there is not much we can guess, so we return an arbitrary value
    /// that will make the BehaviourAction.DrawHandles look good.
    public abstract float GetEstimatedDuration();
    #endif
}
