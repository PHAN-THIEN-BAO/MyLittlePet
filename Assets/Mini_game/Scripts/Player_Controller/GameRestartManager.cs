using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRestartManager : MonoBehaviour
{
    // Hàm này sẽ được gọi khi nhấn button Restart
    public void RestartGame()
    {
        // Reset time scale về 1 để game chạy bình thường
        Time.timeScale = 1;
        
        // Reload scene hiện tại
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    // Hàm này sẽ được gọi khi nhấn button Menu (nếu có)
    public void ReturnToMenu()
    {
        // Reset time scale về 1
        Time.timeScale = 1;
        
        // Load scene Menu (thay "Menu" bằng tên scene menu của bạn)
        SceneManager.LoadScene("Menu");
    }

    // Hàm này sẽ được gọi khi nhấn button Quit
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
