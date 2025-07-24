using UnityEngine;

public class EnemyRandomMovement : MonoBehaviour
{
    public float moveSpeed = 1.7f;
    public float changeDirectionTime = 2f;
    private float timer;
    private Vector2 movement;
    private Animator animator;

    private int currentDirection = -1; // Lưu direction hiện tại

    void Start()
    {
        animator = GetComponent<Animator>();
        ChangeDirection();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChangeDirection();
        }

        transform.Translate(movement * (moveSpeed * Time.deltaTime));
        UpdateAnimation();
    }

    void ChangeDirection()
    {
        int newDirection;
        do
        {
            newDirection = Random.Range(0, 5); // 0 = Up, 1 = Down, 2 = Left, 3 = Right, 4 = Idle
        } while (newDirection == currentDirection && (currentDirection != -1 || newDirection != 4)); // Đảm bảo khác direction hiện tại, trừ lần đầu

        currentDirection = newDirection;

        switch (newDirection)
        {
            case 0: movement = new Vector2(0, 1); break;
            case 1: movement = new Vector2(0, -1); break;
            case 2: movement = new Vector2(-1, 0); break;
            case 3: movement = new Vector2(1, 0); break;
            case 4: movement = Vector2.zero; break;
        }

        timer = changeDirectionTime;
    }

    void UpdateAnimation()
    {
        animator.SetFloat("Move X", movement.x);
        animator.SetFloat("Move Y", movement.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider is BoxCollider2D)
        {
            ChangeDirection();
        }
    }
}
