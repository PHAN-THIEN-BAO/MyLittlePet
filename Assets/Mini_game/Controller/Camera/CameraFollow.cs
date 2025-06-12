// 6/9/2025 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Tham chiếu đến Player
    public Vector3 offset = new Vector3(0, 5, -10); // Khoảng cách giữa Camera và Player
    public Tilemap tilemap; // Tham chiếu đến Tilemap

    private Vector2 minBounds; // Giới hạn dưới của Tilemap
    private Vector2 maxBounds; // Giới hạn trên của Tilemap
    private Camera cam; // Camera chính

    void Start()
    {
        // Lấy thông tin giới hạn của Tilemap
        Bounds tilemapBounds = tilemap.localBounds;
        minBounds = tilemapBounds.min;
        maxBounds = tilemapBounds.max;

        // Lấy tham chiếu đến Camera
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (player != null)
        {
            // Theo dõi vị trí của Player (cộng với offset)
            Vector3 targetPosition = player.position + offset;

            // Tính toán kích thước của Camera
            float cameraHalfHeight = cam.orthographicSize;
            float cameraHalfWidth = cam.aspect * cameraHalfHeight;

            // Giới hạn vị trí của Camera trong Tilemap
            float clampedX = Mathf.Clamp(targetPosition.x, minBounds.x + cameraHalfWidth, maxBounds.x - cameraHalfWidth);
            float clampedY = Mathf.Clamp(targetPosition.y, minBounds.y + cameraHalfHeight, maxBounds.y - cameraHalfHeight);

            // Cập nhật vị trí của Camera
            transform.position = new Vector3(clampedX, clampedY, targetPosition.z);
        }
    }
}