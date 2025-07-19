//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerMovement : MonoBehaviour
//{
//    [SerializeField] private float moveSpeed = 5f;
//    [SerializeField] private GameObject movementBlocker; // GameObject that blocks movement when active
//    private Rigidbody2D rb;
//    private Vector2 moveInput;
//    private Animator animator;
//    private bool playingFootsteps = false;

//    public float footstepSpeed = 0.5f;

//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        animator = GetComponent<Animator>();
//    }

//    void Update()
//    {
//        // Check if movement is blocked
//        bool isMovementBlocked = movementBlocker != null && movementBlocker.activeInHierarchy;

//        if (isMovementBlocked)
//        {
//            // Stop movement when blocked
//            rb.linearVelocity = Vector2.zero;
//            animator.SetBool("IsWalking", false);
//            StopFootStep();
//        }
//        else
//        {
//            // Normal movement logic
//            rb.linearVelocity = moveInput * moveSpeed;
//            animator.SetBool("IsWalking", rb.linearVelocity.magnitude > 0);

//            if (rb.linearVelocity.magnitude > 0 && !playingFootsteps)
//            {
//                StartFootSteps();
//            }
//            else if (rb.linearVelocity.magnitude == 0)
//            {
//                StopFootStep();
//            }
//        }
//    }

//    public void Move(InputAction.CallbackContext context)
//    {
//        // Check if movement is blocked before processing input
//        bool isMovementBlocked = movementBlocker != null && movementBlocker.activeInHierarchy;

//        if (isMovementBlocked)
//        {
//            // Don't process movement input when blocked
//            return;
//        }

//        animator.SetBool("IsWalking", true);

//        if (context.canceled)
//        {
//            animator.SetBool("IsWalking", false);
//            animator.SetFloat("LastInputX", moveInput.x);
//            animator.SetFloat("LastInputY", moveInput.y);
//        }
//        moveInput = context.ReadValue<Vector2>();
//        animator.SetFloat("InputX", moveInput.x);
//        animator.SetFloat("InputY", moveInput.y);
//    }

//    void StartFootSteps()
//    {
//        playingFootsteps = true;
//        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
//    }

//    void PlayFootstep()
//    {
//        SoundEffectManager.Play("Footstep");
//    }

//    void StopFootStep()
//    {
//        playingFootsteps = false;
//        CancelInvoke(nameof(PlayFootstep));
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private List<GameObject> movementBlockers = new List<GameObject>(); // Multiple GameObjects that block movement when active
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool playingFootsteps = false;

    public float footstepSpeed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if movement is blocked by any blocker
        bool isMovementBlocked = IsMovementBlocked();

        if (isMovementBlocked)
        {
            // Stop movement when blocked
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            StopFootStep();
        }
        else
        {
            // Normal movement logic
            rb.linearVelocity = moveInput * moveSpeed;
            animator.SetBool("IsWalking", rb.linearVelocity.magnitude > 0);

            if (rb.linearVelocity.magnitude > 0 && !playingFootsteps)
            {
                StartFootSteps();
            }
            else if (rb.linearVelocity.magnitude == 0)
            {
                StopFootStep();
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        // Check if movement is blocked before processing input
        bool isMovementBlocked = IsMovementBlocked();

        if (isMovementBlocked)
        {
            // Don't process movement input when blocked
            return;
        }

        animator.SetBool("IsWalking", true);

        if (context.canceled)
        {
            animator.SetBool("IsWalking", false);
            animator.SetFloat("LastInputX", moveInput.x);
            animator.SetFloat("LastInputY", moveInput.y);
        }
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);
    }

    // Check if any movement blocker is active
    private bool IsMovementBlocked()
    {
        foreach (GameObject blocker in movementBlockers)
        {
            if (blocker != null && blocker.activeInHierarchy)
            {
                return true;
            }
        }
        return false;
    }

    // Method to add a new movement blocker at runtime
    public void AddMovementBlocker(GameObject blocker)
    {
        if (!movementBlockers.Contains(blocker))
        {
            movementBlockers.Add(blocker);
        }
    }

    // Method to remove a movement blocker at runtime
    public void RemoveMovementBlocker(GameObject blocker)
    {
        if (movementBlockers.Contains(blocker))
        {
            movementBlockers.Remove(blocker);
        }
    }

    void StartFootSteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep");
    }

    void StopFootStep()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }
}