using UnityEngine;

public class TimeScale : MonoBehaviour
{
    public void PauseGame()
    {
        Time.timeScale = 0f; // set time scale to 0 to pause the game
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f; // set time scale to 1 to resume the game
    }
}
