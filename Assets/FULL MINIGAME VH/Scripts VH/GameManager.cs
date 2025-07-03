using UnityEngine;
using TMPro; // Assuming you want to use TextMeshPro for UI text display
using UnityEngine.SceneManagement; // For scene management, if needed
public class GameManager : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText; // Reference to the TextMeshProUGUI component for displaying score
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWinUI;
    private bool isGameOver = false; // Flag to check if the game is over
    private bool isGameWin = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScore(); // Initialize the score display at the start of the game
        gameOverUI.SetActive(false); // Ensure the Game Over UI is not visible at the start
        gameWinUI.SetActive(false); // Ensure the Game Win UI is not visible at the start
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AddScore(int points)
    {
        if (!isGameOver && !isGameWin)
        {
            score += points;
            UpdateScore();
        }


    }
    private void UpdateScore()
    {
        scoreText.text = score.ToString(); // Update the score text display
    }
    public void GameOver()
    {
        isGameOver = true; // Set the game over flag to true
        Time.timeScale = 0; // Pause the game by setting time scale to 0
        gameOverUI.SetActive(true); // Activate the Game Over UI
    }
    public void GameWin()
    {
        isGameWin = true; // Set the game win flag to true
        Time.timeScale = 0; // Pause the game by setting time scale to 0
        gameWinUI.SetActive(true); // Activate the Game Win UI
    }
    public void RestartGame()
    {
        isGameOver = false; // Reset the game over flag
        Time.timeScale = 1; // Resume the game by setting time scale back to 1
        score = 0; // Reset the score
        UpdateScore(); // Update the score display
        SceneManager.LoadScene("Game"); // Reload the game scene to restart the game

    }
    public void GotoMenu()
    { 
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;// Load the main menu scene
    }    
    public bool IsGameOver()
    {
        return isGameOver; // Return the current state of the game over flag
    }
    public bool IsGameWin()
    {
        return isGameWin; // Return the current state of the game win flag

    }
}
