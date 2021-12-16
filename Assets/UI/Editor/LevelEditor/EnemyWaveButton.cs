using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class EnemyWaveButton : Button
{
    public new class UxmlFactory : UxmlFactory<EnemyWaveButton> {}
    
    
    /* Queried elements */
    
    /// Button showing enemy wave name
    private Button m_Button;
    

    /// Parameterless constructor to allow UxmlFactory for manual editing (mostly for testing)
    public EnemyWaveButton()
    {
        Init();
    }
    
    public EnemyWaveButton(string waveName)
    {
        Init();

        if (m_Button != null)
        {
            m_Button.text = waveName;
        }
    }
    
    private void Init()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/LevelEditor/EnemyWaveButton.uxml");
        visualTree.CloneTree(this);

        m_Button = this.Q<Button>("Button");
    }
}