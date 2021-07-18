using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// Script to attach to Canvas Title Menu
/// Tag: CanvasTitleMenu 
public class CanvasTitleMenu : MonoBehaviour
{
    [Header("Scene references")]
    
    [Tooltip("Title Text (TMP)")]
    public TextMeshProUGUI titleText;
    
    [Tooltip("Menus Parent")]
    public Transform menusParent;
    
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    
    public void ShowTitle()
    {
        titleText.enabled = true;
    }

    public void HideTitle()
    {
        titleText.enabled = false;
    }
}
