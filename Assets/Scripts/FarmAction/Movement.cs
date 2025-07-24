using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    public float speed;

    public Animator animator;

    private Vector3 direction;

    //get input from the player
    //apply movement to sprite

    private void Update()
    {
        //get input from the player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //create normalized direction vector
        direction = new Vector3(horizontal, vertical, 0);

        AnimateMovement(direction);
    }

    private void FixedUpdate()
    {
        //apply movement to sprite
        if (direction.magnitude > 0)
        {
            this.transform.position += direction.normalized * speed * Time.deltaTime;
        }
    }

    void AnimateMovement(Vector3 direction)
    {
        if (animator != null)
        {
            if (direction.magnitude > 0)
            {
                animator.SetBool("isMoving", true);

                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.y);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }
    }

}