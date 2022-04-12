using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;
using CommonsPattern;

/// Master behaviour for a character
public class CharacterMaster : MasterBehaviour, IPooledObject
{
    /* Sibling components */

    private Rigidbody2D m_Rigidbody2D;


    protected override void Init()
    {
        base.Init();

        m_Rigidbody2D = this.GetComponentOrFail<Rigidbody2D>();
    }

    // Do not define a Start method to call Setup, as Setup is managed
    // Instead, each Character PoolManager will Spawn an instance of Character,
    // and Spawn will call Setup.


    /* Own methods */

    /// Teleport character to passed position and setup
    /// This is like a Spawn, except it doesn't activate the game object: AcquireFreeObject is responsible for it.
    public void WarpAndSetup(Vector2 position)
    {
        // We now set position before calling Setup as some Setup may use the position e.g. to spawn related objects.
        // Make sure to set Transform position, not position, to avoid 1 frame lag when spawning character in
        // non-Fixed-Update context (it happens with EnemyWave when it uses Coroutine to delay spawn enemy)
        transform.position = position;

        Setup();
    }

    // Callback on Death (for Player Character) or Exit (for all characters)
    public virtual void OnDeathOrExit() {}


    /* IPooledObject interface */

    public void Acquire()
    {
        gameObject.SetActive(true);
    }

    public bool IsInUse()
    {
        return gameObject.activeSelf;
    }

    public void Release()
    {
        Clear();
        gameObject.SetActive(false);
    }
}
