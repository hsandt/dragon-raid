using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using CommonsHelper;
using CommonsPattern;

/// Main Menu Manager
/// SEO: after LocatorManager, before all sub-menu scripts (for instance Instance access)
public class MainMenuManager : SingletonManager<MainMenuManager>
{
    [Header("Assets")]

    [Tooltip("Level Data List asset")]
    public LevelDataList levelDataList;


    [Header("Scene references")]

    [Tooltip("Canvas Title Menu")]
    public CanvasTitleMenu canvasTitleMenu;


    /* Cached scene references */

    private MainMenu m_MainMenu;


    /* State */

    private readonly Stack<Menu> m_MenuStack = new Stack<Menu>();


    protected override void Init()
    {
        base.Init();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(levelDataList != null, this, "[MainMenuManager] Awake: Level Data List not set on {0}", this);
        #endif

        // Retrieve Canvas Main Menu is not set
        if (canvasTitleMenu == null)
        {
            GameObject canvasTitleMenuObject = LocatorManager.Instance.FindWithTag(ConstantsManager.Tags.CanvasTitleMenu);
            if (canvasTitleMenuObject)
            {
                canvasTitleMenu = canvasTitleMenuObject.GetComponentOrFail<CanvasTitleMenu>();
            }
        }
    }

    private void Start()
    {
        // Note that we prefer hiding things in Start than in Awake,
        // because SEO guarantees this class is initialized before any sub-menu, so Awake would be too early,
        // while Start allows sub-menus to call their own Awake to setup things and assert on bad things early.
        // ! This means sub-menus should *not* rely on their Start to initialize things required before their Show.

        // Immediately hide whole main menu  until we want to show it. Don't use Hide(), which may contain an animation
        // (sub-menus will be hidden below anything, but not Title + Version)
        canvasTitleMenu.gameObject.SetActive(false);

        // Hide all menus and remember which was the main menu (for ShowMainMenu later)
        // Of course the whole tree is hidden for now, but it will allow us not to have to hide all irrelevant sub-menus
        // later in ShowMainMenu
        Transform menusParent = canvasTitleMenu.menusParent;
        if (menusParent)
        {
            var menus = menusParent.GetComponentsInChildren<Menu>();
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
            Debug.AssertFormat(m_MainMenu != null, menusParent, "No Main Menu component found under {0}", menusParent);
            #endif
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            Debug.LogErrorFormat(canvasTitleMenu, "No Menu parent set on {0}", canvasTitleMenu);
        }
        #endif
    }

    /// Show canvas and enter main menu
    /// Should be called once by TitleManager, when ready
    public void ShowMainMenu()
    {
        canvasTitleMenu.Show();
        EnterMenu(m_MainMenu);
    }

    public void EnterMenu(Menu menu)
    {
        if (menu == null)
        {
            throw new ArgumentNullException(nameof(menu));
        }

        // Hide current menu, if any
        if (m_MenuStack.Count > 0)
        {
            m_MenuStack.Peek().Hide();
        }

        // Push and show next menu
        m_MenuStack.Push(menu);
        menu.Show();

        UpdateTitleVisibility(menu);
    }

    public void GoBackToPreviousMenu()
    {
        // Pop and hide current menu
        Menu menu = m_MenuStack.Pop();
        menu.Hide();

        // Show previous menu, if any
        if (m_MenuStack.Count > 0)
        {
            Menu previousMenu = m_MenuStack.Peek();
            previousMenu.Show();
            UpdateTitleVisibility(previousMenu);
        }
    }

    private void UpdateTitleVisibility(Menu lastMenu)
    {
        if (lastMenu.ShouldShowTitle())
        {
            // if title is already shown, does nothing
            canvasTitleMenu.ShowTitle();
        }
        else
        {
            // if title is already hidden, does nothing
            canvasTitleMenu.HideTitle();
        }
    }

    /// Start level with given levelIndex
    /// This is a common functionality across Story, Arcade and Level Select so we defined the method in this class
    public void StartLevel(int levelIndex)
    {
        if (levelDataList.levelDataArray.Length > levelIndex)
        {
            LevelData levelData = levelDataList.levelDataArray[levelIndex];
            if (levelData != null)
            {
                SceneManager.LoadScene(levelData.sceneIndex);
            }
            else
            {
                Debug.LogErrorFormat(levelDataList, "[MainMenuManager] StartGame: Level Data List entry " +
                    "for levelIndex {0} is null", levelIndex);
            }
        }
        else
        {
            Debug.LogErrorFormat(levelDataList, "[MainMenuManager] StartGame: Level Data List has only {0} entries, " +
                "cannot get entry for levelIndex {1}",
                levelDataList.levelDataArray.Length, levelIndex);
        }
    }

    /// PlayerInput action message callback for Cancel
    private void OnCancel(InputValue value)
    {
        Debug.AssertFormat(value.isPressed, this, "value.isPressed is false. Make sure that InputActions only detect Cancel on press.");

        if (m_MenuStack.Count > 0)
        {
            Menu menu = m_MenuStack.Peek();
            if (menu.CanGoBack())
            {
                // For now, works in all cases, but when we add special behaviour on Back
                // like prompting for changes or auto-applying changes in Options,
                // we may want to call some overridden OnBack method on the current menu
                GoBackToPreviousMenu();
            }
        }
    }
}
