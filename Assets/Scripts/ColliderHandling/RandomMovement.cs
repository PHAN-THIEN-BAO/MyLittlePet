using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f; // Low speed
    [SerializeField] private float changeDirectionTime = 3.0f; // Time between direction changes
    [SerializeField] private bool useVerticalMovement = true; // Toggle for Y-axis movement
    [SerializeField] private float verticalMovementRange = 0.5f; // Controls intensity of vertical movement
    [SerializeField] private float stopDuration = 2.0f; // Duration to stop moving
    [SerializeField] private float moveDuration = 4.0f; // Duration to keep moving
    [SerializeField] private FlipMethod flipMethod = FlipMethod.Scale; // How to flip the object

    private Vector3 moveDirection;
    private float directionTimer;
    private float movementStateTimer;
    private bool isMoving = true;
    private Vector3 originalScale;

    public enum FlipMethod
    {
        Scale,      // Flip by inverting scale (for sprites)
        Rotation    // Flip by rotating (for 3D models)
    }

    private void Start()
    {
        // Store original scale for flipping
        originalScale = transform.localScale;
        
        // Set initial random direction
        ChangeDirection();
        
        // Initialize movement state timer
        movementStateTimer = moveDuration;
    }

    private void Update()
    {
        // Handle movement state (moving or stopped)
        UpdateMovementState();

        // Only move and flip if in moving state
        if (isMoving)
        {
            // Move in the current direction
            transform.Translate(moveDirection * speed * Time.deltaTime);
            
            // Update orientation based on movement direction
            UpdateOrientation();
        }

        // Update direction timer and change direction when needed
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0)
        {
            ChangeDirection();
        }
    }

    private void UpdateMovementState()
    {
        movementStateTimer -= Time.deltaTime;
        
        if (movementStateTimer <= 0)
        {
            // Toggle between moving and stopped states
            isMoving = !isMoving;
            
            // Set appropriate timer based on new state
            movementStateTimer = isMoving ? moveDuration : stopDuration;
        }
    }

    private void UpdateOrientation()
    {
        // Only flip if there's significant horizontal movement
        if (Mathf.Abs(moveDirection.x) > 0.01f)
        {
            bool facingRight = moveDirection.x > 0;
            
            switch (flipMethod)
            {
                case FlipMethod.Scale:
                    // Flip scale on X axis
                    Vector3 newScale = originalScale;
                    newScale.x = facingRight ? Mathf.Abs(originalScale.x) : -Mathf.Abs(originalScale.x);
                    transform.localScale = newScale;
                    break;
                    
                case FlipMethod.Rotation:
                    // Flip by rotation around Y axis
                    float yRotation = facingRight ? 0f : 180f;
                    transform.rotation = Quaternion.Euler(0, yRotation, 0);
                    break;
            }
        }
    }

    private void ChangeDirection()
    {
        // Generate random 3D direction
        float randomAngle = Random.Range(0f, 360f);
        float yMovement = useVerticalMovement ? Random.Range(-verticalMovementRange, verticalMovementRange) : 0f;
        
        moveDirection = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            yMovement, // Now includes Y-axis movement if enabled
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        ).normalized;

        // Reset direction timer
        directionTimer = changeDirectionTime;
    }
}