using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimeScaleManager : MonoBehaviour
{
    [Header("Debug Settings")]
    public bool enableDebugLogs = true;
    public bool forceResetOnSceneLoad = true;
    
    private void Awake()
    {
        // Đảm bảo script này không bị destroy khi load scene mới
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"🎬 Scene loaded: {scene.name}, TimeScale: {Time.timeScale}");
        }
        
        // Reset timeScale nếu được enable hoặc nếu không phải là 1
        if (forceResetOnSceneLoad || Time.timeScale != 1f)
        {
            float oldTimeScale = Time.timeScale;
            Time.timeScale = 1f;
            
            if (enableDebugLogs)
            {
                Debug.Log($"🔧 TimeScale reset from {oldTimeScale} to {Time.timeScale} in scene: {scene.name}");
            }
        }
    }
    
    private void Update()
    {
        // Hiển thị current timeScale trong debug mode
        if (enableDebugLogs && Time.timeScale != 1f)
        {
            Debug.LogWarning($"⚠️ TimeScale is {Time.timeScale} (not 1.0)");
        }
    }
}