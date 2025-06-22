//using UnityEngine;

//public class CameraFollowMouse : MonoBehaviour
//{
//    [Header("Cài đặt di chuyển")]
//    [SerializeField] private float movementSpeed = 2.0f;
//    [SerializeField] private float smoothTime = 0.3f;

//    [Header("Giới hạn di chuyển")]
//    [SerializeField] private float maxHorizontalMovement = 3.0f;
//    [SerializeField] private float maxVerticalMovement = 2.0f;

//    [Header("Vị trí ban đầu")]
//    [SerializeField] private Vector3 initialPosition;

//    private Vector3 velocity = Vector3.zero;
//    private Vector3 targetPosition;

//    void Start()
//    {
//        // Lưu vị trí ban đầu của camera
//        initialPosition = transform.position;
//        targetPosition = initialPosition;
//    }

//    void Update()
//    {
//        // Lấy vị trí chuột (0,0 ở giữa màn hình)
//        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
//        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;

//        // Tính toán vị trí mục tiêu mới
//        Vector3 newTargetPosition = initialPosition;
//        newTargetPosition.x += mouseX * maxHorizontalMovement;
//        newTargetPosition.y += mouseY * maxVerticalMovement;

//        // Cập nhật vị trí mục tiêu với tốc độ di chuyển
//        targetPosition = Vector3.Lerp(targetPosition, newTargetPosition, Time.deltaTime * movementSpeed);

//        // Di chuyển camera mượt mà đến vị trí mục tiêu
//        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
//    }
//}

using UnityEngine;

public class CameraFollowMouse : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 2.0f;
    [SerializeField] private float smoothTime = 0.3f;
    
    [Header("Movement Limits")]
    [SerializeField] private float maxHorizontalMovement = 3.0f;
    [SerializeField] private float maxVerticalMovement = 2.0f;
    
    [Header("Initial Position")]
    [SerializeField] private Vector3 initialPosition;
    
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPosition;

    void Start()
    {
        // Save the initial camera position
        initialPosition = transform.position;
        targetPosition = initialPosition;
    }

    void Update()
    {
        // Get mouse position (0,0 is at screen center)
        float mouseX = (Input.mousePosition.x / Screen.width) - 0.5f;
        float mouseY = (Input.mousePosition.y / Screen.height) - 0.5f;
        
        // Calculate new target position
        Vector3 newTargetPosition = initialPosition;
        newTargetPosition.x += mouseX * maxHorizontalMovement;
        newTargetPosition.y += mouseY * maxVerticalMovement;
        
        // Update target position with movement speed
        targetPosition = Vector3.Lerp(targetPosition, newTargetPosition, Time.deltaTime * movementSpeed);
        
        // Smoothly move camera to target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}