using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

// DEPRECATED
// Use Living Zone in the complementary area instead, to allow big objects to be cleaned up
// after completely leaving the screen
public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var pooledObject = other.GetComponent<IPooledObject>();
        pooledObject?.Release();
    }
}
