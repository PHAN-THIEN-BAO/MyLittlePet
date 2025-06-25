//using UnityEngine;
//using UnityEngine.InputSystem;
//public class FishControlMovement : MonoBehaviour
//{
//    [SerializeField] float speed = 5f;

//    public InputAction LeftAction;
//    public InputAction RightAction;
//    public InputAction TopAction;
//    public InputAction BottomAction;

//    private Rigidbody2D rigidbody2D;
//    private Vector2 move;

//    void Start()
//    {
//        rigidbody2D = GetComponent<Rigidbody2D>();

//        LeftAction.Enable();
//        RightAction.Enable();
//        TopAction.Enable();
//        BottomAction.Enable();
//    }

//    void Update()
//    {
//        move = Vector2.zero;

//        if (LeftAction.IsPressed())
//        {
//            move.x = -1;
//        }
//        else if (RightAction.IsPressed())
//        {
//            move.x = 1;
//        }

//        if (BottomAction.IsPressed())
//        {
//            move.y = -1;
//        }
//        else if (TopAction.IsPressed())
//        {
//            move.y = 1;
//        }
//    }

//    void FixedUpdate()
//    {
//        Vector2 newPos = rigidbody2D.position + move.normalized * (speed * Time.fixedDeltaTime);
//        rigidbody2D.MovePosition(newPos);
//    }
//}


using UnityEngine;
using UnityEngine.InputSystem;

public class FishControlMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float speed = 5f;
    [SerializeField] float acceleration = 8f;
    [SerializeField] float deceleration = 10f;

    [Header("Animation")]
    [SerializeField] bool enableSwimEffect = true;
    [SerializeField] float swayAmount = 0.05f;
    [SerializeField] float swaySpeed = 3f;

    public InputAction LeftAction;
    public InputAction RightAction;
    public InputAction TopAction;
    public InputAction BottomAction;

    private Rigidbody2D rigidbody2D;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private float swimTimer = 0f;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Kiểm tra và thêm Rigidbody2D nếu cần
        rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
        {
            rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
            Debug.Log("Đã tự động thêm Rigidbody2D vào Fish_main");
        }

        // Cấu hình Rigidbody2D cho chuyển động mượt mà
        rigidbody2D.gravityScale = 0f;
        rigidbody2D.linearDamping = 0.5f;
        rigidbody2D.angularDamping = 0.5f;
        rigidbody2D.freezeRotation = true;
        rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        // Lấy SpriteRenderer nếu có
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Kích hoạt input actions
        LeftAction.Enable();
        RightAction.Enable();
        TopAction.Enable();
        BottomAction.Enable();
    }

    void Update()
    {
        // Đọc input
        moveInput = Vector2.zero;

        if (LeftAction.IsPressed())
        {
            moveInput.x = -1;
        }
        else if (RightAction.IsPressed())
        {
            moveInput.x = 1;
        }

        if (BottomAction.IsPressed())
        {
            moveInput.y = -1;
        }
        else if (TopAction.IsPressed())
        {
            moveInput.y = 1;
        }

        // Chuẩn hóa vector di chuyển
        if (moveInput.magnitude > 1f)
        {
            moveInput.Normalize();
        }

        // Cập nhật hướng nhìn của cá
        UpdateFishOrientation();

        // Cập nhật hiệu ứng bơi
        if (enableSwimEffect && currentVelocity.magnitude > 0.1f)
        {
            swimTimer += Time.deltaTime * swaySpeed;
        }
    }

    void UpdateFishOrientation()
    {
        // Chỉ cập nhật hướng khi có chuyển động
        if (currentVelocity.sqrMagnitude > 0.1f)
        {
            // Quay cá theo hướng di chuyển
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Lật sprite nếu cần
            if (spriteRenderer != null)
            {
                spriteRenderer.flipY = Mathf.Abs(angle) > 90f;
            }
        }
    }

    void FixedUpdate()
    {
        // Di chuyển mượt mà với gia tốc/giảm tốc
        if (moveInput.magnitude > 0.01f)
        {
            // Gia tốc dần đến tốc độ tối đa
            currentVelocity = Vector2.Lerp(
                currentVelocity,
                moveInput * speed,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            // Giảm tốc dần khi không có đầu vào
            currentVelocity = Vector2.Lerp(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        // Thêm hiệu ứng bơi lượn
        Vector2 finalVelocity = currentVelocity;
        if (enableSwimEffect && currentVelocity.magnitude > 0.1f)
        {
            // Tạo chuyển động lượn sóng vuông góc với hướng di chuyển
            Vector2 perpendicularDir = new Vector2(-currentVelocity.y, currentVelocity.x).normalized;
            finalVelocity += perpendicularDir * Mathf.Sin(swimTimer) * swayAmount;
        }

        // Di chuyển cá
        rigidbody2D.MovePosition(rigidbody2D.position + finalVelocity * Time.fixedDeltaTime);
    }
}