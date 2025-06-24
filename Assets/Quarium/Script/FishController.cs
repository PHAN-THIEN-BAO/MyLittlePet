using UnityEngine;

public class FishController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float distanceThreshold = 0.1f;

    [Header("Swimming Animation")]
    [SerializeField] private bool enableSwimmingMotion = true;
    [SerializeField] private float swayAmount = 0.1f;
    [SerializeField] private float swaySpeed = 5f;

    private Camera mainCamera;
    private Vector3 targetPosition;
    private Vector3 lastPosition;
    private float swayTimer = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        targetPosition = transform.position;
        lastPosition = transform.position;
    }

    void Update()
    {
        // Lấy vị trí chuột trong không gian thế giới
        if (Input.GetMouseButton(0)) // Giữ chuột trái để di chuyển cá
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.z; // Điều chỉnh độ sâu
            targetPosition = mainCamera.ScreenToWorldPoint(mousePos);
        }

        // Di chuyển cá đến vị trí mục tiêu
        float distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.y),
                                                 new Vector2(targetPosition.x, targetPosition.y));

        if (distanceToTarget > distanceThreshold)
        {
            // Lưu vị trí trước khi di chuyển
            lastPosition = transform.position;

            // Di chuyển đến vị trí mục tiêu
            transform.position = Vector3.MoveTowards(transform.position,
                                                    new Vector3(targetPosition.x, targetPosition.y, transform.position.z),
                                                    moveSpeed * Time.deltaTime);

            // Xoay cá theo hướng di chuyển
            if ((transform.position - lastPosition).sqrMagnitude > 0.001f)
            {
                Vector3 direction = transform.position - lastPosition;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Xoay sprite (giả sử sprite cá nhìn sang phải là góc 0 độ)
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                // Lật sprite nếu cá bơi sang trái
                Vector3 scale = transform.localScale;
                if (Mathf.Abs(angle) > 90)
                {
                    scale.y = -Mathf.Abs(scale.y);
                }
                else
                {
                    scale.y = Mathf.Abs(scale.y);
                }
                transform.localScale = scale;
            }
        }

        // Thêm chuyển động bơi lượn
        if (enableSwimmingMotion && distanceToTarget > distanceThreshold)
        {
            swayTimer += Time.deltaTime * swaySpeed;
            Vector3 position = transform.position;
            position.y += Mathf.Sin(swayTimer) * swayAmount * Time.deltaTime;
            transform.position = position;
        }
    }
}