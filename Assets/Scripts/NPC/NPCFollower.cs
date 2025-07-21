using System.Collections.Generic;
using UnityEngine;
public class NPCFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float followDistance = 2f;
    public int maxQueueSize = 15;
    public float moveSpeed = 2.5f;
    public float stoppingDistance = 0.8f;
    [Header("Movement Behavior")]
    public float minimumMoveDistance = 0.5f;
    public float accelerationTime = 0.3f;
    public float decelerationTime = 0.2f;
    public bool smoothMovement = true;
    [Header("Animation & Direction")]
    public bool enableDirectionFlip = true;
    public bool enableAnimation = true;
    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Queue<Vector2> positionHistory = new Queue<Vector2>();
    private Vector2 currentTarget;
    private Vector2 currentVelocity;
    private Vector2 lastPlayerPosition;
    private bool hasTarget = false;
    private bool isMoving = false;
    private float currentSpeed = 0f;
    private bool isInitialized = false;
    void Start()
    {
        InitializeNPC();
    }
    void InitializeNPC()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                lastPlayerPosition = player.position;
            }
            else
            {
                Debug.LogError("NPCFollower: Không tìm th?y Player!");
                enabled = false;
                return;
            }
        }
        if (SetupComponents())
        {
            isInitialized = true;
            lastPlayerPosition = player.position;
        }
        else
        {
            Debug.LogError("NPCFollower: Không th? kh?i t?o components!");
            enabled = false;
        }
    }
    bool SetupComponents()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            rb2D = gameObject.AddComponent<Rigidbody2D>();
        }
        if (rb2D == null)
        {
            Debug.LogError("NPCFollower: Không th? t?o Rigidbody2D!");
            return false;
        }
        rb2D.gravityScale = 0f;
        rb2D.linearDamping = 0f;
        rb2D.angularDamping = 5f;
        rb2D.freezeRotation = true;
        rb2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        return true;
    }
    void FixedUpdate()
    {
        if (!isInitialized || player == null || rb2D == null) return;
        UpdatePlayerMovementTracking();
        UpdateNPCMovement();
        UpdateAnimationAndDirection();
    }
    void UpdatePlayerMovementTracking()
    {
        Vector2 currentPlayerPos = player.position;
        float distanceMoved = Vector2.Distance(currentPlayerPos, lastPlayerPosition);
        if (distanceMoved >= minimumMoveDistance)
        {
            positionHistory.Enqueue(lastPlayerPosition);
            lastPlayerPosition = currentPlayerPos;
            while (positionHistory.Count > maxQueueSize)
            {
                currentTarget = positionHistory.Dequeue();
                hasTarget = true;
            }
        }
    }
    void UpdateNPCMovement()
    {
        if (!hasTarget)
        {
            StopMovement();
            return;
        }
        Vector2 currentPos = transform.position;
        float distanceToTarget = Vector2.Distance(currentPos, currentTarget);
        float distanceToPlayer = Vector2.Distance(currentPos, player.position);
        if (distanceToTarget <= stoppingDistance)
        {
            StopMovement();
            hasTarget = false;
            return;
        }
        if (distanceToPlayer < followDistance * 0.7f && distanceToTarget > stoppingDistance * 2f)
        {
            StopMovement();
            return;
        }
        MoveToTarget(currentPos, distanceToTarget);
    }
    void MoveToTarget(Vector2 currentPos, float distanceToTarget)
    {
        Vector2 direction = (currentTarget - currentPos).normalized;
        if (smoothMovement)
        {
            float targetSpeed = Mathf.Min(moveSpeed, distanceToTarget * 2f);
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime / accelerationTime);
            currentVelocity = direction * currentSpeed;
        }
        else
        {
            currentVelocity = direction * moveSpeed;
        }
        if (rb2D != null)
        {
            rb2D.linearVelocity = currentVelocity;
        }
        isMoving = true;
    }
    void StopMovement()
    {
        if (smoothMovement)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0f, Time.fixedDeltaTime / decelerationTime);
            currentVelocity = Vector2.Lerp(currentVelocity, Vector2.zero, Time.fixedDeltaTime / decelerationTime);
            if (currentSpeed < 0.1f)
            {
                currentSpeed = 0f;
                currentVelocity = Vector2.zero;
                isMoving = false;
            }
        }
        else
        {
            currentVelocity = Vector2.zero;
            isMoving = false;
        }
        if (rb2D != null)
        {
            rb2D.linearVelocity = currentVelocity;
        }
    }
    void UpdateAnimationAndDirection()
    {
        if (enableAnimation && animator != null)
        {
            animator.SetFloat("Move X", currentVelocity.x);
            animator.SetFloat("Move Y", currentVelocity.y);
            animator.SetBool("IsMoving", isMoving);
        }
        if (enableDirectionFlip && spriteRenderer != null)
        {
            Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)transform.position).normalized;
            if (Mathf.Abs(directionToPlayer.x) > 0.1f)
            {
                if (directionToPlayer.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if (player == null) return;
        Vector3 position = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, followDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, stoppingDistance);
        if (hasTarget)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(currentTarget, 0.3f);
            Gizmos.DrawLine(position, currentTarget);
        }
        if (positionHistory != null && positionHistory.Count > 1)
        {
            Gizmos.color = Color.yellow;
            Vector2 prevPos = position;
            int step = Mathf.Max(1, positionHistory.Count / 10);
            var positions = new List<Vector2>(positionHistory);
            for (int i = 0; i < positions.Count; i += step)
            {
                Gizmos.DrawLine(prevPos, positions[i]);
                Gizmos.DrawSphere(positions[i], 0.1f);
                prevPos = positions[i];
            }
        }
    }
    public void SetFollowTarget(Transform newPlayer)
    {
        if (newPlayer == null)
        {
            Debug.LogWarning("NPCFollower: Attempted to set null player target!");
            return;
        }
        player = newPlayer;
        lastPlayerPosition = player.position;
        positionHistory.Clear();
        hasTarget = false;
        StopMovement();
    }
    public void StopFollowing()
    {
        hasTarget = false;
        StopMovement();
        positionHistory.Clear();
    }
    public void ResumeFollowing()
    {
        if (player != null)
        {
            lastPlayerPosition = player.position;
            positionHistory.Clear();
        }
    }
    public bool IsFollowing()
    {
        return hasTarget && isMoving;
    }
    public bool IsInitialized()
    {
        return isInitialized;
    }
}