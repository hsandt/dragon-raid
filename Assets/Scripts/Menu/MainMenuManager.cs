using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Main Menu Manager
public class MainMenuManager : SingletonManager<MainMenuManager>
{
    [Header("Scene references")]
    
    [Tooltip("Canvas Main Menu")]
    public Canvas canvasMainMenu;
    
    
    /* Cached scene references */
    
    private MainMenu m_MainMenu;
    
    
    /* State */

    private readonly Stack<Menu> m_MenuStack = new Stack<Menu>();

    
    private void Start()
    {
        // Hide all menus and remember which on was the main menu
        GameObject menuParent = LocatorManager.Instance.FindWithTag(Tags.Menus);
        if (menuParent)
        {
            var menus = menuParent.GetComponentsInChildren<Menu>();
            foreach (Menu menu in menus)
            {
                var mainMenu = menu as MainMenu;
                if (mainMenu != null)
                {
                    m_MainMenu = mainMenu;
                }

                // Do not call Hide() which may contain some animation, immediately deactivate instead
                menu.gameObject.SetActive(false);
            }
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.AssertFormat(m_MainMenu != null, menuParent, "No Main Menu component found under {0}", menuParent);
            #endif
        }

        // Hide whole main menu (sub-menus are already hidden, but not Title + Version) until we want to show it
        canvasMainMenu.enabled = false;
    }

    /// Show canvas and enter main menu
    /// Should be called once by TitleManager, when ready
    public void ShowMainMenu()
    {
        canvasMainMenu.enabled = true;
        EnterMenu(m_MainMenu);
    }
    
    public void EnterMenu(Menu menu)
    {
        // Hide current menu, if any
        if (m_MenuStack.Count > 0)
        {
            m_MenuStack.Peek().Hide();
        }
        
        // Push and show next menu
        m_MenuStack.Push(menu);
        menu.Show();
    }
    
    public void GoBackToPreviousMenu()
    {
        // Pop and hide current menu
        Menu menu = m_MenuStack.Pop();
        menu.Hide();

        // Show previous menu, if any
        if (m_MenuStack.Count > 0)

        {
            m_MenuStack.Peek().Show();
        }
    }
}
