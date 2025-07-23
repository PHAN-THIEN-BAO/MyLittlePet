using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private Animator animator;
    private bool isGrounded;
    // Transform to check if the player is on the ground
    // Speed at which the player moves
    private Rigidbody2D rb;
    private GameManager gameManager;
    private AudioManager audioManager;// Reference to the GameManager script
    private void Awake()
    {
        animator = GetComponent<Animator>(); // Get the Animator component attached to the player
        rb = GetComponent<Rigidbody2D>();
        gameManager =FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>(); // Find the AudioManager in the scene

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.IsGameOver()||gameManager.IsGameWin()) return;
        HandleMovement();
        HandleJump(); // Call the jump handling method
        UpdateAnimation();
         }
    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal"); // Get horizontal input (A/D or Left/Right arrow keys)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y); // Set the horizontal velocity while keeping the vertical velocity unchanged
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1); // Face right when moving right
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1); // Face left when moving left
    }
    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded) // Check if the jump button is pressed and the player is on the ground
        {
            audioManager.PlayJumpSound(); // Play jump sound effect
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); // Apply an upward force for jumping

        }
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer); // Check if the player is on the ground using a circle overlap check
    }
    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);

        // Check if the player is moving horizontally
    }
}
