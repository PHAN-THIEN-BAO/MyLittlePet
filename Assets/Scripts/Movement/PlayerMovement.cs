//using UnityEngine;
//using UnityEngine.InputSystem;

//public class PlayerMovement : MonoBehaviour













using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private List<GameObject> movementBlockers = new List<GameObject>();
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private bool playingFootsteps = false;

    public float footstepSpeed = 0.5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isMovementBlocked = IsMovementBlocked();

        if (isMovementBlocked)
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsWalking", false);
            StopFootStep();
        }
        else
        {
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
        bool isMovementBlocked = IsMovementBlocked();

        if (isMovementBlocked)
        {
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

    public void AddMovementBlocker(GameObject blocker)
    {
        if (!movementBlockers.Contains(blocker))
        {
            movementBlockers.Add(blocker);
        }
    }

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