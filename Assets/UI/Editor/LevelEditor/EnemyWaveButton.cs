using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class EnemyWaveButton : Button
{
    public new class UxmlFactory : UxmlFactory<EnemyWaveButton> {}
    
    
    /* Queried elements */
    
    /// Label showing enemy wave name
    private Label m_Label;
    

    /// Parameterless constructor to allow UxmlFactory for manual editing (mostly for testing)
    public EnemyWaveButton()
    {
        Init();
    }
    
    public EnemyWaveButton(string waveName)
    {
        Init();

        if (m_Label != null)
        {
            m_Label.text = waveName;
        }
    }
    
    private void Init()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Editor/LevelEditor/EnemyWaveButton.uxml");
        visualTree.CloneTree(this);

        m_Label = this.Q<Label>("Label");
    }
}