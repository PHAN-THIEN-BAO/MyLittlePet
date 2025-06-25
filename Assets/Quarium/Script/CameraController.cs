//using UnityEngine;

//public class CameraController : MonoBehaviour
//{
//    [Header("Movement Settings")]
//    [SerializeField] private float manualMoveSpeed = 5.0f;
//    [SerializeField] private float followSpeed = 5.0f;

//    [Header("Following Options")]
//    [SerializeField] private bool followFish = true;
//    [SerializeField] private Transform fishTarget;
//    [SerializeField] private float followSmoothTime = 0.1f;

//    [Header("Boundaries")]
//    [SerializeField] private bool useBoundaries = true;
//    [SerializeField] private float minX = -50f;
//    [SerializeField] private float maxX = 50f;
//    [SerializeField] private float minY = -30f;
//    [SerializeField] private float maxY = 30f;

//    private Vector3 currentVelocity = Vector3.zero;

//    void Start()
//    {
//        // Auto-find fish if not assigned
//        if (fishTarget == null)
//        {
//            GameObject fish = GameObject.Find("Fish_main");
//            if (fish != null)
//            {
//                fishTarget = fish.transform;
//            }
//            else
//            {
//                Debug.LogError("Không tìm thấy đối tượng Fish_main!");
//            }
//        }
//    }

//    void LateUpdate()
//    {
//        Vector3 targetPosition = transform.position;

//        // Follow fish if enabled and target exists
//        if (followFish && fishTarget != null)
//        {
//            targetPosition = new Vector3(
//                fishTarget.position.x,
//                fishTarget.position.y,
//                transform.position.z
//            );
//        }

//        // Manual override with keyboard
//        float horizontalInput = Input.GetAxis("Horizontal");
//        float verticalInput = Input.GetAxis("Vertical");

//        if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
//        {
//            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0);
//            transform.position += moveDirection * manualMoveSpeed * Time.deltaTime;

//            // If manual input, temporarily disable following
//            if (moveDirection.magnitude > 0.1f)
//            {
//                targetPosition = transform.position;
//            }
//        }
//        else if (followFish && fishTarget != null)
//        {
//            // Smooth follow when not manually controlling
//            transform.position = Vector3.SmoothDamp(
//                transform.position,
//                targetPosition,
//                ref currentVelocity,
//                followSmoothTime,
//                followSpeed
//            );
//        }

//        // Apply boundaries after all movement
//        if (useBoundaries)
//        {
//            transform.position = new Vector3(
//                Mathf.Clamp(transform.position.x, minX, maxX),
//                Mathf.Clamp(transform.position.y, minY, maxY),
//                transform.position.z
//            );
//        }
//    }

//    // Public method to toggle fish following (can be called from UI)
//    public void ToggleFishFollowing(bool value)
//    {
//        followFish = value;
//    }
//}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Following Options")]
    [SerializeField] private bool followFish = true;
    [SerializeField] private Transform fishTarget;
    [SerializeField] private float followSmoothTime = 0.05f; // Giảm xuống để theo sát hơn
    [SerializeField] private float followSpeed = 20.0f;     // Tăng lên để theo sát hơn

    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minY = -30f;
    [SerializeField] private float maxY = 30f;

    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        // Auto-find fish if not assigned
        if (fishTarget == null)
        {
            GameObject fish = GameObject.Find("Fish_main");
            if (fish != null)
            {
                fishTarget = fish.transform;
            }
            else
            {
                Debug.LogError("Không tìm thấy đối tượng Fish_main!");
            }
        }
    }

    void LateUpdate()
    {
        if (followFish && fishTarget != null)
        {
            // Tính toán vị trí mục tiêu (vị trí của cá)
            Vector3 targetPosition = new Vector3(
                fishTarget.position.x,
                fishTarget.position.y,
                transform.position.z
            );

            // Sử dụng SmoothDamp với thời gian smoothing thấp và tốc độ cao
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                followSmoothTime,
                followSpeed
            );

            // Áp dụng giới hạn sau khi di chuyển
            if (useBoundaries)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, minX, maxX),
                    Mathf.Clamp(transform.position.y, minY, maxY),
                    transform.position.z
                );
            }
        }
    }

    // Public method to toggle fish following (can be called from UI)
    public void ToggleFishFollowing(bool value)
    {
        followFish = value;
    }
}