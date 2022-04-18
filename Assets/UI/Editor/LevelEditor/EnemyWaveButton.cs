using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class EnemyWaveButton : VisualElement
{
    public new class UxmlFactory : UxmlFactory<EnemyWaveButton> {}


    /* Injected parameters */

    private EventTrigger_SpatialProgress m_SpatialEventTrigger;
    private EnemyWave m_EnemyWave;


    /* Properties*/

    public float SpatialEventRequiredSpatialProgress => m_SpatialEventTrigger.RequiredSpatialProgress;


    /* Queried elements */

    /// Button showing enemy wave name
    private Button m_Button;


    /// Parameterless constructor to allow UxmlFactory for manual editing (mostly for testing)
    public EnemyWaveButton()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/LevelEditor/EnemyWaveButton.uxml");
        visualTree.CloneTree(this);

        m_Button = this.Q<Button>("Button");
        Debug.AssertFormat(m_Button != null, "[EnemyWaveButton] No Button 'Button' found on Enemy Wave Button UXML");
    }

    public void Init(EventTrigger_SpatialProgress spatialEventTrigger, EnemyWave enemyWave)
    {
        m_SpatialEventTrigger = spatialEventTrigger;
        m_EnemyWave = enemyWave;

        if (m_Button != null)
        {
            m_Button.text = enemyWave.name;

            // Bind behaviour to select game object on button click
            m_Button.clicked += OnClick;
        }
    }

    private void OnClick()
    {
        if (m_EnemyWave != null)
        {
            Selection.activeObject = m_EnemyWave.gameObject;
        }
        else
        {
            Debug.LogError("[EnemyWaveButton] Enemy Wave not set, cannot select game object");
        }
    }
}