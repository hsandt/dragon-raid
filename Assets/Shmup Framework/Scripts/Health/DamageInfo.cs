using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

public class DamageInfo
{
    /// How much damage is dealt
    public int damage;

    /// Faction of the entity causing the damage or that spawned the projectile
    /// that causing the damage
    public Faction attackerFaction = Faction.None;

    /// Element type associated to damage
    public ElementType elementType = ElementType.Neutral;

    /// Direction of attack dealing damage
    public HorizontalDirection hitDirection = HorizontalDirection.None;
}
