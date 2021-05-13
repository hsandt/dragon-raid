using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;
using UnityConstants;

public class HUD : SingletonManager<HUD>
{
	[Header("Child references")]

	[Tooltip("Health gauge for player character")]
	public GaugeHealth gaugeHealthPlayer;
	
	
    public void AssignGaugeHealthPlayerTo(HealthSystem healthSystem)
    {
	    gaugeHealthPlayer.RegisterHealthSystem(healthSystem);
    }
}
