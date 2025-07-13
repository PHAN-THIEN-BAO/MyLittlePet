using UnityEngine;

public class TimeScaleChecker : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"🕐 Scene Start - TimeScale: {Time.timeScale}");
        
        if (Time.timeScale != 1f)
        {
            Debug.LogError($"❌ TimeScale is {Time.timeScale}, resetting to 1.0");
            Time.timeScale = 1f;
        }
    }
    
    private void Update()
    {
        // FORCE DESTROY any TimeScale components in SampleScene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SampleScene")
        {
            TimeScale[] timeScaleComponents = FindObjectsOfType<TimeScale>();
            if (timeScaleComponents.Length > 0)
            {
                Debug.LogWarning($"🗑️ Found {timeScaleComponents.Length} TimeScale components in SampleScene, removing components!");
                foreach (var component in timeScaleComponents)
                {
                    Destroy(component); // Chỉ destroy component, không destroy GameObject
                }
            }
        }
        
        // Kiểm tra liên tục
        if (Time.timeScale != 1f)
        {
            Debug.LogWarning($"⚠️ TimeScale changed to {Time.timeScale}, auto-resetting");
            Time.timeScale = 1f;
        }
    }
}