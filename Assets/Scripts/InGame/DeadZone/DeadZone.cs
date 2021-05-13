using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CommonsPattern;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var pooledObject = other.GetComponent<IPooledObject>();
        pooledObject?.Release();
    }
}
