using UnityEngine;

public class playerconllision : MonoBehaviour
{
    GameManager gameManager;
    AudioManager audioManager; // Assuming you have an AudioManager for sound effects
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>(); // Find the AudioManager in the scene
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            audioManager.PlaycoinSound(); // Destroy the coin object when collected
            gameManager.AddScore(1);
          
        }
       else if (collision.CompareTag("Trap"))
        {
            gameManager.GameOver(); // Call GameOver method from GameManager when player collides with a trap
           
        }
        else if (collision.CompareTag("Enemy"))
        {
            gameManager.GameOver(); // Call GameOver method from GameManager when player collides with a trap

        }
        else if (collision.CompareTag("Enemy2"))
        {
            gameManager.GameOver(); // Call GameOver method from GameManager when player collides with a trap

        }
        else if (collision.CompareTag("Key"))
        {
            Destroy(collision.gameObject); // Destroy the key object when collected
            gameManager.GameWin(); // Call GameWin method from GameManager when player collects a key
        }
    }

}
