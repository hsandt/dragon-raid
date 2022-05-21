using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

/// Interface that a sibling component of PickUp should implement to trigger an effect on pick-up
public interface IPickUpEffect
{
    /// Apply pick-up effect to entity that picked this (pickUpCollector)
    void OnPick(PickUpCollector pickUpCollector);
}
