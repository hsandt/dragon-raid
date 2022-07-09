using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsHelper;

/// FX component for objects with Particle System (auto-Release when stopped)
public class FXWithParticleSystem : FX
{
    private void FixedUpdate()
    {
        // Unlike animated sprites, Particle System cannot define Animation events to call Release() one frame
        // after the last visible sprite, so instead, we must manually check for when particle system has stopped.
        // IMPORTANT: we assume that all managed particles have been stored in slaveParticles (we recommend using
        // addSiblingComponentsAsSlaves if they are sibling components). If this isn't done, then we won't find any
        // playing particles and will immediately release this object, effectively hiding any sibling/child particles
        // still playing. 
        foreach (var slaveParticle in slaveParticles)
        {
            // Make sure to test for NOT isStopped. This is different from isPlaying, which does not consider the case
            // of isPaused. And since all particles are paused on game pause, only checking isPlaying would effectively
            // make all paused particles considered dead, therefore Releasing all FX with particle systems!
            if (!slaveParticle.isStopped)
            {
                // At least one particle is not stopped (playing or paused), so keep pooled object alive
                return;
            }
        }

        // All particles are stopped, Release pooled object
        Release();
    }
}
