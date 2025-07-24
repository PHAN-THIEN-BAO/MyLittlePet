using UnityEngine;
using TMPro; 
using UnityEngine.SceneManagement; 
public class GameManager : MonoBehaviour
{
    private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText; 
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private GameObject gameWinUI;
    private bool isGameOver = false; 
    private bool isGameWin = false;
    
    void Start()
    {
        UpdateScore(); 
        gameOverUI.SetActive(false); 
        gameWinUI.SetActive(false); 
    }

  
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
        scoreText.text = score.ToString(); 
    }
    public void GameOver()
    {
        isGameOver = true; 
        Time.timeScale = 0; 
        gameOverUI.SetActive(true); 
    }
    public void GameWin()
    {
        isGameWin = true; 
        Time.timeScale = 0; 
        gameWinUI.SetActive(true); 
    }
    public void RestartGame()
    {
        isGameOver = false;
        Time.timeScale = 1; 
        score = 0; 
        UpdateScore(); 
        SceneManager.LoadScene("Game"); 

    }
    public void GotoMenu()
    { 
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }    
    public bool IsGameOver()
    {
        return isGameOver; 
    }
    public bool IsGameWin()
    {
        return isGameWin; 

    }
}
