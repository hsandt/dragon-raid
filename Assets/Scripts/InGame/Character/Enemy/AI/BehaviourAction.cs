using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourAction : MonoBehaviour
{
    /* Scene references */
    
    /// Owner of this script
    protected EnemyCharacterMaster m_EnemyCharacterMaster;


    /// Initialize: set owner
    public void Init(EnemyCharacterMaster enemyCharacterMaster)
    {
        m_EnemyCharacterMaster = enemyCharacterMaster;
        
        OnInit();
    }
    
    /// Action initialisation callback. Called only once on Init (called recursively), it should
    /// cache references to any required components, and do any other preprocessing we'd rather avoid later
    /// (this is still be called in-game at enemy spawn time as we cannot predict action sequence override)
    protected virtual void OnInit() {}
    
    /// Action start callback. Useful to setup and cache info (optional)
    public virtual void OnStart() {}

    /// Run Update callback (called every frame while action is active)
    public abstract void RunUpdate ();

    /// Return true iff action is over or deactivated. Allows to manually deactivate action objects in the editor
    /// to "mute" them for testing, etc.
    public bool IsOverOrDeactivated()
    {
        return !gameObject.activeSelf || IsOver();
    }
        
    /// Return true iff action is over and we should proceed to next action when executed inside a sequence.
    /// Protected because external objects should call IsOverOrDeactivated instead.
    protected abstract bool IsOver();
        
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
