using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameResultSender : MonoBehaviour
{
    /// <summary>
    /// Gọi method này khi người chơi THẮNG mini-game
    /// </summary>
    public void OnPlayerWin()
    {
        SendResult(true);
    }
    
    /// <summary>
    /// Gọi method này khi người chơi THUA mini-game
    /// </summary>
    public void OnPlayerLose()
    {
        SendResult(false);
    }
    
    /// <summary>
    /// Gửi kết quả và quay về main scene
    /// </summary>
    private void SendResult(bool won)
    {
        // Reset Time.timeScale về 1 trước khi chuyển scene
        Time.timeScale = 1f;
        Debug.Log("🔧 Time.timeScale reset to 1 before returning to SampleScene");
        
        // Lưu kết quả vào PlayerPrefs (như PlayingManager đã expect)
        PlayerPrefs.SetInt("MiniGameCompleted", 1);
        PlayerPrefs.SetInt("MiniGameWon", won ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log($"🎮 Mini-game result sent: Won = {won}");
        
        // Quay về main scene (thay "MainScene" bằng tên scene chính của bạn)
        SceneManager.LoadScene("SampleScene"); 
    }

    /// <summary>
    /// Gắn vào Button "Win" trong mini-game UI
    /// </summary>
    public void OnWinButtonClick()
    {
        // Reset Time.timeScale về 1 trước khi chuyển scene
        Time.timeScale = 1f;
        
        PlayerPrefs.SetInt("MiniGameCompleted", 1);
        PlayerPrefs.SetInt("MiniGameWon", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    /// <summary>
    /// Gắn vào Button "Lose" trong mini-game UI
    /// </summary>
    public void OnLoseButtonClick()
    {
        // Reset Time.timeScale về 1 trước khi chuyển scene
        Time.timeScale = 1f;
        
        PlayerPrefs.SetInt("MiniGameCompleted", 1);
        PlayerPrefs.SetInt("MiniGameWon", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
}