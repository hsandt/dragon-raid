using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileImpactHandler
{
    /// Handle impact from another projectile causing damage
    bool OnProjectileImpact(DamageInfo damageInfo);
}
