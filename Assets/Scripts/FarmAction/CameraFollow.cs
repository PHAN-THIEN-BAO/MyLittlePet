using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // The target to follow

    Vector3 camOffset; // Offset from the target position

    void Start()
    {
        camOffset = transform.position - target.position;// Calculate the initial offset
    }

    private void FixedUpdate()
    {
        transform.position = target.position + camOffset; // Update the camera position to follow the target
    }
}