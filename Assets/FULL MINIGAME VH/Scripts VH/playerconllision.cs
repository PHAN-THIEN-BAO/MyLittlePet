using UnityEngine;

public class playerconllision : MonoBehaviour
{
    GameManager gameManager;
    AudioManager audioManager;
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            audioManager.PlaycoinSound(); 
            gameManager.AddScore(1);
          
        }
       else if (collision.CompareTag("Trap"))
        {
            gameManager.GameOver(); 
        }
        else if (collision.CompareTag("Enemy"))
        {
            gameManager.GameOver();

        }
        else if (collision.CompareTag("Enemy2"))
        {
            gameManager.GameOver(); 

        }
        else if (collision.CompareTag("Key"))
        {
            Destroy(collision.gameObject); 
            gameManager.GameWin(); 
        }
    }

}
