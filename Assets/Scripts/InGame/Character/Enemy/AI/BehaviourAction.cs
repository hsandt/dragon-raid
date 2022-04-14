using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BehaviourAction : MonoBehaviour
{
    /* Scene references */

    /// Owner of this script
    protected EnemyCharacterMaster m_EnemyCharacterMaster;


    /// Initialize: set owner and call OnInit
    public void Init(EnemyCharacterMaster enemyCharacterMaster)
    {
        m_EnemyCharacterMaster = enemyCharacterMaster;

        OnInit();
    }

    /// Action initialisation callback. Called only once on Init (called recursively), it should
    /// cache references to any required components, and do any other preprocessing we'd rather avoid later
    /// (this is called in-game at enemy spawn time as we cannot predict BT override).
    /// It should be called without failure even if the action object is inactive (in case we activate it later
    /// at runtime, so we get the preprocessing we need). Therefore, it is recommended to put all initialization code
    /// in OnInit, rather than split across Awake and OnInit, as Awake is not called if the object is inactive.
    protected virtual void OnInit() {}

    /// Action start callback. Useful to setup and cache info (optional)
    public virtual void OnStart() {}

    /// Run Update callback (called every frame while action is active)
    /// This assumes that IsOver returns false before RunUpdate is called.
    public virtual void RunUpdate() {}

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
}
