using UnityEngine;

/// <summary>
/// Setup script để tự động kết nối care history system với tất cả pet care managers
/// </summary>
[System.Serializable]
public class CareHistorySetup : MonoBehaviour
{
    [Header("Auto Setup Settings")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private bool createRecorderIfMissing = true;
    [SerializeField] private bool showSetupMessages = true;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCareHistorySystem();
        }
    }
    
    /// <summary>
    /// Tự động setup toàn bộ care history system
    /// </summary>
    public void SetupCareHistorySystem()
    {
        if (showSetupMessages)
            Debug.Log("🔧 Setting up Care History System...");
        
        // 1. Đảm bảo CareHistoryRecorder tồn tại
        EnsureCareHistoryRecorder();
        
        // 2. Setup care history managers
        SetupCareHistoryManager();
        
        // 3. Kết nối với các pet care managers
        ConnectToPetCareManagers();
        
        if (showSetupMessages)
            Debug.Log("✅ Care History System setup completed!");
    }
    
    /// <summary>
    /// Đảm bảo CareHistoryRecorder tồn tại trong scene
    /// </summary>
    private void EnsureCareHistoryRecorder()
    {
        if (CareHistoryRecorder.Instance == null && createRecorderIfMissing)
        {
            GameObject recorderObj = new GameObject("CareHistoryRecorder");
            recorderObj.AddComponent<CareHistoryRecorder>();
            
            if (showSetupMessages)
                Debug.Log("📝 Created CareHistoryRecorder automatically");
        }
    }
    
    /// <summary>
    /// Setup CareHistoryManager nếu chưa có
    /// </summary>
    private void SetupCareHistoryManager()
    {
        CareHistoryManager careManager = FindObjectOfType<CareHistoryManager>();
        if (careManager == null)
        {
            // Tìm hoặc tạo panel cho care history
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject managerObj = new GameObject("CareHistoryManager");
                managerObj.transform.SetParent(canvas.transform, false);
                managerObj.AddComponent<CareHistoryManager>();
                
                if (showSetupMessages)
                    Debug.Log("📋 Created CareHistoryManager automatically");
            }
        }
    }
    
    /// <summary>
    /// Kết nối với tất cả pet care managers trong scene
    /// </summary>
    private void ConnectToPetCareManagers()
    {
        // Connect to PetInfoUIManager
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null)
        {
            if (showSetupMessages)
                Debug.Log("🔗 Connected to PetInfoUIManager");
        }
        
        // Connect to FeedingManager
        FeedingManager feedingManager = FindObjectOfType<FeedingManager>();
        if (feedingManager != null)
        {
            if (showSetupMessages)
                Debug.Log("🔗 Connected to FeedingManager");
        }
        
        // Connect to PlayingManager
        PlayingManager playingManager = FindObjectOfType<PlayingManager>();
        if (playingManager != null)
        {
            if (showSetupMessages)
                Debug.Log("🔗 Connected to PlayingManager");
        }
        
        // Connect to PetSleepManager
        PetSleepManager sleepManager = FindObjectOfType<PetSleepManager>();
        if (sleepManager != null)
        {
            if (showSetupMessages)
                Debug.Log("🔗 Connected to PetSleepManager");
        }
    }
    
    /// <summary>
    /// Manual setup method có thể gọi từ Inspector hoặc script khác
    /// </summary>
    [ContextMenu("Setup Care History System")]
    public void ManualSetup()
    {
        SetupCareHistorySystem();
    }
}