using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using CommonsHelper;
using CommonsPattern;

public class LevelSelectMenu : Menu
{
    [Header("Assets")]
    
    [Tooltip("Level Button Prefab")]
    public GameObject levelButtonPrefab;


    [Header("Child references")]
    
    [Tooltip("Level Buttons parent")]
    public Transform levelButtonsParent;
    
    [Tooltip("Back button")]
    public Button buttonBack;

    
    /* Cached references */

    /// Array of save slot container widgets
    private LevelButtonWidget[] m_LevelButtonWidgets;

    private void Awake()
    {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.AssertFormat(levelButtonPrefab != null, this,
            "[LevelSelectMenu] Awake: Level Button Prefab not set on {0}", this);
        Debug.AssertFormat(levelButtonsParent != null, this,
            "[LevelSelectMenu] Awake: Level Buttons Parent not set on {0}", this);
        Debug.AssertFormat(buttonBack != null, this, "[LevelSelectMenu] Awake: Button Back not set on {0}", this);
        #endif

        buttonBack.onClick.AddListener(GoBack);
        
        // Instance access guaranteed by SEO
        LevelDataList levelDataList = MainMenuManager.Instance.levelDataList;
        int levelCount = levelDataList.levelDataArray.Length;
        m_LevelButtonWidgets = new LevelButtonWidget[levelCount];
        
        UIPoolHelper.LazyInstantiateWidgets(levelButtonPrefab, levelCount, levelButtonsParent);
        
        for (int i = 0; i < levelCount; i++)
        {
            Transform saveSlotTransform = levelButtonsParent.GetChild(i);
            
            // Initialise widget model and view
            m_LevelButtonWidgets[i] = saveSlotTransform.GetComponentOrFail<LevelButtonWidget>();
            m_LevelButtonWidgets[i].Init(i);
        }
    }
    
    private void OnDestroy()
    {
        if (buttonBack)
        {
            buttonBack.onClick.RemoveAllListeners();
        }
    }

    public override void Show()
    {
        gameObject.SetActive(true);

        if (m_LevelButtonWidgets.Length > 0)
        {
            m_LevelButtonWidgets[0].Select();
        }
    }

    public override void Hide()
    {
        EventSystem.current.SetSelectedGameObject(null);

        gameObject.SetActive(false);
    }

    public override bool ShouldShowTitle()
    {
        return false;
    }

    private void GoBack()
    {
        MainMenuManager.Instance.GoBackToPreviousMenu();
    }
}
