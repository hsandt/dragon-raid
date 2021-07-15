using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Put this script on any game object with a Text or TextMeshProUGUI to set the text to the current level on Start
/// (it shows 00-based index not readable 1-based number) 
public class LevelLabel : MonoBehaviour
{
	private Text m_TextWidget;
	private TextMeshProUGUI m_TMPWidget;

	void Awake () {
		m_TextWidget = GetComponent<Text>();
		m_TMPWidget = GetComponent<TextMeshProUGUI>();
	}

	void Start () {
		UpdateText();
	}

	private void UpdateText () {
		string version = $"Level {InGameManager.Instance.LevelData.levelIndex:00}";
		
		if (m_TextWidget)
		{
			m_TextWidget.text = version;
		}
		
		if (m_TMPWidget)
		{
			m_TMPWidget.text = version;
		}
	}
}

