using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

public class InGameManager : SingletonManager<InGameManager>
{
    protected override void Init()
    {
        base.Init();
    }

    public void RestartLevel()
    {
        Debug.Log("restart!");
    }
}
