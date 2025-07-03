using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]private float speed = 2f;
    [SerializeField] private float distance = 5f;
    private Vector3 startPos;
    private bool movingRight = true;
    // Range within which the enemy can move
    // Speed of the enemy movement
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position; // Store the initial position of the enemy
    }

    // Update is called once per frame
    void Update()
    {
        float leftBound = startPos.x - distance; // Calculate the left boundaryB
        float rightBound = startPos.x + distance; // Calculate the right boundary
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime); // Move the enemy to the right
            if(transform.position.x >= rightBound)
            {
                movingRight = false; // Change direction when reaching the right boundary
                Flip(); // Flip the enemy's direction
            }
        }
        else
        {
            {
                transform.Translate(Vector2.left * speed * Time.deltaTime); // Move the enemy to the left
                if (transform.position.x <= leftBound)
                {
                    movingRight = true; // Change direction when reaching the left boundary
                    Flip();

                }
            }
        }

    }
    void Flip()
    {
        Vector3 scaler = transform.localScale; // Get the current scale of the enemy
        scaler.x *= -1; // Flip the x scale to change direction
        transform.localScale = scaler; // Apply the new scale to the enemy  
    }
}
