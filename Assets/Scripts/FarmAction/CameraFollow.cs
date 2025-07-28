using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmCameraFollow : MonoBehaviour
{
    [Header("Camera Follow Settings")]
    [SerializeField] private Transform target; // The target to follow
    [SerializeField] private bool autoFindPlayer = true; // Tự động tìm player
    [SerializeField] private string playerTag = "Player"; // Tag của player
    [SerializeField] private bool centerPlayerOnStart = true; // Đặt player ở giữa camera khi bắt đầu

    [Header("Follow Behavior")]
    [SerializeField] private bool useFixedOffset = false; // Sử dụng offset cố định
    [SerializeField] private Vector3 manualOffset = Vector3.zero; // Offset thủ công

    Vector3 camOffset; // Offset from the target position

    void Start()
    {
        // Tự động tìm player nếu chưa được gán và autoFindPlayer = true
        if (autoFindPlayer && target == null)
        {
            FindPlayer();
        }

        if (target != null)
        {
            if (centerPlayerOnStart)
            {
                // Đặt nhân vật ở giữa camera
                CenterPlayerInCamera();
            }
            else
            {
                // Tính offset dựa trên vị trí hiện tại
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
        // Đặt player ở vị trí giữa camera (giữ nguyên Z của player)
        Vector3 cameraCenter = new Vector3(transform.position.x, transform.position.y, target.position.z);
        target.position = cameraCenter;
        
        // Tính offset sau khi đã đặt player ở giữa
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
        transform.position = target.position + camOffset; // Update the camera position to follow the target
    }
}