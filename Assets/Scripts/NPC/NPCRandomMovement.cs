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
    
    private int currentDirection = -1;
    private bool isMovementPaused = false;
    public bool isMoving;

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
        CheckInteractionState();
        
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
            bool wasMovementPaused = isMovementPaused;
            isMovementPaused = !npcComponent.CanInteract();
            
            if (!wasMovementPaused && isMovementPaused)
            {
                StopMovement();
            }
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
        timer = 0f;
    }

    void ChangeDirection()
    {
        if (isMovementPaused)
            return;
            
        int newDirection;
        do
        {
            newDirection = Random.Range(0, 5);
        } while (newDirection == currentDirection && (currentDirection != -1 || newDirection != 4));

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
        if (animator != null)
        {
            animator.SetFloat("Move X", movement.x);
            animator.SetFloat("Move Y", movement.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isMovementPaused && collision.collider != null && collision.collider is BoxCollider2D)
        {
            ChangeDirection();
        }
    }

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