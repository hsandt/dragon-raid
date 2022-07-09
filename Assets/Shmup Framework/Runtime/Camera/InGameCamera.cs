using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    [Header("Child references")]

    [Tooltip("Player Respawn Position. Unlike the tagged Player Spawn Position, it is relative to the camera.")]
    public Transform playerRespawnPosition;
}
