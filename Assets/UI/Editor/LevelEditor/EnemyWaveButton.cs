using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class EnemyWaveButton : VisualElement
{
    public new class UxmlFactory : UxmlFactory<EnemyWaveButton> {}
    
    
    /* Queried elements */
    
    /// Button showing enemy wave name
    private Button m_Button;
    

    /// Parameterless constructor to allow UxmlFactory for manual editing (mostly for testing)
    public EnemyWaveButton()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/LevelEditor/EnemyWaveButton.uxml");
        visualTree.CloneTree(this);

        m_Button = this.Q<Button>("Button");
        Debug.AssertFormat(m_Button != null, "[LevelEditor] No Button 'EnemyWavePreviewArea' found on Enemy Wave Button UXML");
    }
    
    public void Init(EventTrigger_SpatialProgress spatialEventTrigger, EnemyWave enemyWave)
    {
        if (m_Button != null)
        {
            m_Button.text = enemyWave.name;
            
            // Bind behaviour to select game object on button click
            m_Button.clicked += () => { Selection.activeObject = enemyWave.gameObject; };
        }
    }
}