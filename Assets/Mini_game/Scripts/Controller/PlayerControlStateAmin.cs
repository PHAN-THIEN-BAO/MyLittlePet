using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControlStateAmin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Animator animator;
    private float MoveX = 0.0f;
    private float MoveY = 0.0f;
    private float acceleration = 10f;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentY = animator.GetFloat("Move Y");
        float currentX = animator.GetFloat("Move X");
        bool fowardPressed = Keyboard.current.wKey.isPressed;
        bool leftPressed = Keyboard.current.aKey.isPressed;
        bool rightPressed = Keyboard.current.dKey.isPressed;
        bool downPressed = Keyboard.current.sKey.isPressed;
        
        // if player presseed foward
        if (fowardPressed && currentY < 1f)
        {
            currentY += Time.deltaTime * acceleration;
        }

        if (fowardPressed && currentY > 1)
        {
            currentY = 1.0f;
        }
        //if player release foward key
        if (!fowardPressed && currentY > 0.05f)
        {
            currentY -= Time.deltaTime * acceleration;
        }

        if (downPressed && currentY > -1.0f)
        {
            currentY -= Time.deltaTime * acceleration;
        }

        if (!downPressed && currentY < 0.0f)
        {
            currentY += Time.deltaTime * acceleration;
        }
        //player press right key
        if (rightPressed && currentX < 1.0f)
        {
            currentX += Time.deltaTime * acceleration;
        }

        if (rightPressed && currentX > 1.0f)
        {
            currentX = 1.0f;
        }
        
        if (!rightPressed && currentX > 0.0f)
        {
            currentX -= Time.deltaTime * acceleration;
        }
        
        
        //player pressed left key
        if (leftPressed && currentX > -1.0f)
        {
            currentX -= Time.deltaTime * acceleration;
        }

        if (leftPressed && currentX < -1.0f)
        {
            currentX = -1.0f;
        }
        
        if (!leftPressed && currentX < 0.0f)
        {
            currentX += Time.deltaTime * acceleration;
        }
        
        

        
        
        
        
        animator.SetFloat("Move Y", currentY);
        animator.SetFloat("Move X", currentX);
        
    }
}
