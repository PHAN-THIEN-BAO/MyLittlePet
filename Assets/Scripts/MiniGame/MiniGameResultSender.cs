using UnityEngine;
using UnityEngine.SceneManagement;
public class MiniGameResultSender : MonoBehaviour
{
    public void OnPlayerWin()
    {
        SendResult(true);
    }
    public void OnPlayerLose()
    {
        SendResult(false);
    }
    private void SendResult(bool won)
    {
        Time.timeScale = 1f;
        Debug.Log("?? Time.timeScale reset to 1 before returning to SampleScene");
        PlayerPrefs.SetInt("MiniGameCompleted", 1);
        PlayerPrefs.SetInt("MiniGameWon", won ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log($"?? Mini-game result sent: Won = {won}");
        SceneManager.LoadScene("SampleScene");
    }
    public void OnWinButtonClick()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("MiniGameCompleted", 1);
        PlayerPrefs.SetInt("MiniGameWon", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
    public void OnLoseButtonClick()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("MiniGameCompleted", 1);
        PlayerPrefs.SetInt("MiniGameWon", 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
}