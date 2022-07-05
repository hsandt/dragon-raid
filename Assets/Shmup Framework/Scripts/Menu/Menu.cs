using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : MonoBehaviour
{
    public abstract void Show();
    public abstract void Hide();
    public abstract bool ShouldShowTitle();
    public abstract bool CanGoBack();
}
