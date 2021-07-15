using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityConstants;
using CommonsPattern;

/// Main Menu Manager
public class MainMenuManager : SingletonManager<MainMenuManager>
{
    /* State */

    private readonly Stack<Menu> m_MenuStack = new Stack<Menu>();

    
    private void Start()
    {
        // Hide all menus, except the main menu
        GameObject menuParent = LocatorManager.Instance.FindWithTag(Tags.Menus);
        if (menuParent)
        {
            var menus = menuParent.GetComponentsInChildren<Menu>();
            foreach (Menu menu in menus)
            {
                if (menu is MainMenu)
                {
                    // In case MainMenu was disabled (e.g. because we are working on other menus in the scene),
                    // enter menu (it's important to push it to stack, and play any Show animation)
                    EnterMenu(menu);
                }
                else
                {
                    // Do not call Hide() which may contain some animation, immediately deactivate instead
                    menu.gameObject.SetActive(false);
                }
            }
        }
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
