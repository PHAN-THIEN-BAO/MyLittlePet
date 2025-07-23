// 6/9/2025 AI-Tag
// This was created with assistance from Muse, a Unity Artificial Intelligence product

using UnityEngine;

public class EnemyRandomMovement : MonoBehaviour
{
    public float moveSpeed = 1.7f; // Tốc độ di chuyển
    public float changeDirectionTime = 2f; // Thời gian để thay đổi hướng
    private float timer; // Đếm ngược để đổi hướng
    private Vector2 movement; // Hướng di chuyển hiện tại
    private Animator animator; // Animator để điều khiển animation

    void Start()
    {
        animator = GetComponent<Animator>(); // Lấy Animator gắn trên GameObject
        ChangeDirection(); // Khởi tạo hướng di chuyển ban đầu
    }

    void Update()
    {
        // Đếm ngược và đổi hướng khi cần
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChangeDirection();
        }

        // Di chuyển enemy
        transform.Translate(movement * (moveSpeed * Time.deltaTime));

        // Cập nhật animation dựa trên hướng di chuyển
        UpdateAnimation();
    }

    void ChangeDirection()
    {
        // Random chọn hướng di chuyển hoặc đứng yên
        int direction = Random.Range(0, 5); // 0 = Up, 1 = Down, 2 = Left, 3 = Right, 4 = Idle
        
        switch (direction)
        {
            case 0: // Lên trên
                movement = new Vector2(0, 1);
                break;
            case 1: // Xuống dưới
                movement = new Vector2(0, -1);
                break;
            case 2: // Sang trái
                movement = new Vector2(-1, 0);
                break;
            case 3: // Sang phải
                movement = new Vector2(1, 0);
                break;
            case 4: // Đứng yên
                movement = Vector2.zero;
                break;
        }

        // Reset thời gian thay đổi hướng
        timer = changeDirectionTime;
    }

    void UpdateAnimation()
    {
        // Cập nhật các parameter Move X và Move Y trong Animator
        animator.SetFloat("Move X", movement.x);
        animator.SetFloat("Move Y", movement.y);
    }
}