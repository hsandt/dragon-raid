// This is literally a script to prevent async methods leaking into the game after stopping Play in Editor.
// Place it on some EditorOnly game object in scenes that have this issue.
// Currently, the only visible issue is Tween Drivers being added on game objects after stopping Play.
// This is particularly visible with the Splash Screen, so place a game object with this script in the Main Menu at least.
// Source: https://forum.unity.com/threads/non-stopping-async-method-after-in-editor-game-is-stopped.558283/
// Author: jasonmcguirk
// Minor changes by huulong: surround the whole method with UNITY_EDITOR instead of just its body

using System;
using System.Reflection;
using System.Threading;
using UnityEngine;

public class SynchronizationContextSwitcher : MonoBehaviour
{
    #if UNITY_EDITOR
    void OnApplicationQuit()
    {
        var constructor = SynchronizationContext.Current.GetType().GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof(int)}, null);
        var newContext = constructor.Invoke(new object[] {Thread.CurrentThread.ManagedThreadId });
        SynchronizationContext.SetSynchronizationContext(newContext as SynchronizationContext);  
    }
    #endif
}
