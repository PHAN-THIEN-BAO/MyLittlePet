using UnityEngine;

public class NPCRandomMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 1.7f;
    public float changeDirectionTime = 2f;
    
    private float timer;
    private Vector2 movement;
    private Animator animator;
    private NPC npcComponent;
    
    private int currentDirection = -1; // Lưu direction hiện tại
    private bool isMovementPaused = false; // Để tạm dừng movement khi đang tương tác
    public bool isMoving; // GOOD: per-pet instance

    void Start()
    {
        animator = GetComponent<Animator>();
        npcComponent = GetComponent<NPC>();
        
        if (npcComponent == null)
        {
            Debug.LogWarning("NPCRandomMovement requires an NPC component on the same GameObject!");
        }
        
        ChangeDirection();
    }

    void Update()
    {
        // Kiểm tra xem NPC có đang tương tác không
        CheckInteractionState();
        
        // Chỉ di chuyển khi không bị tạm dừng
        if (!isMovementPaused)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ChangeDirection();
            }

            transform.Translate(movement * (moveSpeed * Time.deltaTime));
        }
        
        UpdateAnimation();
    }

    void CheckInteractionState()
    {
        if (npcComponent != null)
        {
            // Kiểm tra xem NPC có đang trong dialogue không
            bool wasMovementPaused = isMovementPaused;
            isMovementPaused = !npcComponent.CanInteract(); // CanInteract() trả về false khi đang tương tác
            
            // Nếu vừa bắt đầu tương tác, dừng animation
            if (!wasMovementPaused && isMovementPaused)
            {
                StopMovement();
            }
            // Nếu vừa kết thúc tương tác, có thể tiếp tục di chuyển
            else if (wasMovementPaused && !isMovementPaused)
            {
                ResumeMovement();
            }
        }
    }

    void StopMovement()
    {
        movement = Vector2.zero;
        UpdateAnimation();
    }

    void ResumeMovement()
    {
        // Đặt lại timer để thay đổi direction ngay lập tức khi resume
        timer = 0f;
    }

    void ChangeDirection()
    {
        // Không thay đổi direction khi đang tương tác
        if (isMovementPaused)
            return;
            
        int newDirection;
        do
        {
            newDirection = Random.Range(0, 5); // 0 = Up, 1 = Down, 2 = Left, 3 = Right, 4 = Idle
        } while (newDirection == currentDirection && (currentDirection != -1 || newDirection != 4)); // Đảm bảo khác direction hiện tại, trừ lần đầu

        currentDirection = newDirection;

        switch (newDirection)
        {
            case 0: movement = new Vector2(0, 1); break;      // Up
            case 1: movement = new Vector2(0, -1); break;     // Down
            case 2: movement = new Vector2(-1, 0); break;     // Left
            case 3: movement = new Vector2(1, 0); break;      // Right
            case 4: movement = Vector2.zero; break;           // Idle
        }

        timer = changeDirectionTime;
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Move X", movement.x);
            animator.SetFloat("Move Y", movement.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Chỉ thay đổi direction khi va chạm nếu không đang tương tác
        if (!isMovementPaused && collision.collider != null && collision.collider is BoxCollider2D)
        {
            ChangeDirection();
        }
    }

    // Public methods để có thể control từ bên ngoài nếu cần
    public void PauseMovement()
    {
        isMovementPaused = true;
        StopMovement();
    }

    public void ResumeMovementExternal()
    {
        isMovementPaused = false;
        ResumeMovement();
    }

    public bool IsMovementPaused()
    {
        return isMovementPaused;
    }
}