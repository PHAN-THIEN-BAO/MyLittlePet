using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Tên scene sẽ được load khi nhấn phím F")]
    public string targetSceneName = "YourSceneName";
    
    [Header("Input Settings")]
    [Tooltip("Phím để chuyển scene")]
    public KeyCode switchSceneKey = KeyCode.F;
    
    [Header("Debug Settings")]
    [Tooltip("Hiển thị thông báo debug khi chuyển scene")]
    public bool showDebugLog = true;
    
    void Update()
    {
        // Kiểm tra khi người chơi nhấn phím F
        if (Input.GetKeyDown(switchSceneKey))
        {
            SwitchScene();
        }
    }
    
    public void SwitchScene()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target Scene Name chưa được thiết lập!");
            return;
        }
        
        if (showDebugLog)
        {
            Debug.Log($"Chuyển đến scene: {targetSceneName}");
        }
        
        SceneManager.LoadScene(targetSceneName);
    }
    
    public void SwitchToScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name không hợp lệ!");
            return;
        }
        
        if (showDebugLog)
        {
            Debug.Log($"Chuyển đến scene: {sceneName}");
        }
        
        SceneManager.LoadScene(sceneName);
    }
}