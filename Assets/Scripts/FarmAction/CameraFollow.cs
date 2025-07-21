using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FarmCameraFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private bool autoFindPlayer = true;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool centerPlayerOnStart = true;
    [Header("Follow Behavior")]
    [SerializeField] private bool useFixedOffset = false;
    [SerializeField] private Vector3 manualOffset = Vector3.zero;
    Vector3 camOffset;
    void Start()
    {
        if (autoFindPlayer && target == null)
        {
            FindPlayer();
        }
        if (target != null)
        {
            if (centerPlayerOnStart)
            {
                CenterPlayerInCamera();
            }
            else
            {
                CalculateOffset();
            }
        }
        else
        {
            Debug.LogError("FarmCameraFollow: Target is not assigned! Please assign a target in the Inspector or ensure the player has the correct tag.");
        }
    }
    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            target = playerObject.transform;
            Debug.Log($"FarmCameraFollow: Found player - {playerObject.name}");
        }
        else
        {
            Debug.LogWarning($"FarmCameraFollow: No GameObject with tag '{playerTag}' found!");
        }
    }
    private void CenterPlayerInCamera()
    {
        Vector3 cameraCenter = new Vector3(transform.position.x, transform.position.y, target.position.z);
        target.position = cameraCenter;
        CalculateOffset();
        Debug.Log("Player positioned at camera center");
    }
    private void CalculateOffset()
    {
        if (useFixedOffset)
        {
            camOffset = manualOffset;
        }
        else
        {
            camOffset = transform.position - target.position;
        }
    }
    private void FixedUpdate()
    {
        transform.position = target.position + camOffset;
    }
}