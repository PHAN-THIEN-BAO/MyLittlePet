using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestartManager : MonoBehaviour
{
    // Hàm này sẽ được gọi khi nhấn button Restart
    public void RestartGame()
    {
        // reset time scale to 1 to ensure the game runs normally after restart
        Time.timeScale = 1;

        // Reload the current scene to restart the game
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // this function will be called when the Return to Menu button is pressed
    public void ReturnToMenu()
    {
        // Reset time scale về 1
        Time.timeScale = 1;

        // Load scene Menu, replacing the current scene
        SceneManager.LoadScene("Menu");
    }

    // this function will be called when the Quit button is pressed
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
