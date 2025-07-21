using UnityEngine;
public class RandomMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float changeDirectionTime = 3.0f;
    [SerializeField] private bool useVerticalMovement = true;
    [SerializeField] private float verticalMovementRange = 0.5f;
    [SerializeField] private float stopDuration = 2.0f;
    [SerializeField] private float moveDuration = 4.0f;
    [SerializeField] private FlipMethod flipMethod = FlipMethod.Scale;
    private Vector3 moveDirection;
    private float directionTimer;
    private float movementStateTimer;
    private bool isMoving = true;
    private Vector3 originalScale;
    public enum FlipMethod
    {
        Scale,
        Rotation
    }
    private void Start()
    {
        originalScale = transform.localScale;
        ChangeDirection();
        movementStateTimer = moveDuration;
    }
    private void Update()
    {
        UpdateMovementState();
        if (isMoving)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
            UpdateOrientation();
        }
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
            isMoving = !isMoving;
            movementStateTimer = isMoving ? moveDuration : stopDuration;
        }
    }
    private void UpdateOrientation()
    {
        if (Mathf.Abs(moveDirection.x) > 0.01f)
        {
            bool facingRight = moveDirection.x > 0;
            switch (flipMethod)
            {
                case FlipMethod.Scale:
                    Vector3 newScale = originalScale;
                    newScale.x = facingRight ? Mathf.Abs(originalScale.x) : -Mathf.Abs(originalScale.x);
                    transform.localScale = newScale;
                    break;
                case FlipMethod.Rotation:
                    float yRotation = facingRight ? 0f : 180f;
                    transform.rotation = Quaternion.Euler(0, yRotation, 0);
                    break;
            }
        }
    }
    private void ChangeDirection()
    {
        float randomAngle = Random.Range(0f, 360f);
        float yMovement = useVerticalMovement ? Random.Range(-verticalMovementRange, verticalMovementRange) : 0f;
        moveDirection = new Vector3(
            Mathf.Cos(randomAngle * Mathf.Deg2Rad),
            yMovement,
            Mathf.Sin(randomAngle * Mathf.Deg2Rad)
        ).normalized;
        directionTimer = changeDirectionTime;
    }
}