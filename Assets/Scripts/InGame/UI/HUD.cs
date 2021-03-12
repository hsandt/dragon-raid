using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;
using UnityConstants;

public class HUD : MonoBehaviour
{
	[Header("Child references")]

	[Tooltip("Health gauge for player character")]
	public GaugeHealth gaugeHealthPlayer;
	
	
    private void Awake()
    {
        GameObject playerCharacter = Locator.FindWithTag(Tags.Player);
		if (playerCharacter != null)
		{
			gaugeHealthPlayer.trackedHealthSystem = playerCharacter.GetComponent<HealthSystem>();
		}
    }
}
