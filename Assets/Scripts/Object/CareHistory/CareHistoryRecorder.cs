using UnityEngine;
using System.Collections;

/// <summary>
/// Central system để ghi lại lịch sử chăm sóc pet từ tất cả các manager
/// </summary>
public class CareHistoryRecorder : MonoBehaviour
{
    [Header("Care History Settings")]
    [SerializeField] private bool enableHistoryRecording = true;
    [SerializeField] private bool showRecordingMessages = false;
    
    // Activity IDs mapping
    private const int FEEDING_ACTIVITY_ID = 1;
    private const int PLAYING_ACTIVITY_ID = 3;
    private const int SLEEPING_ACTIVITY_ID = 2;
    
    // Singleton instance
    public static CareHistoryRecorder Instance { get; private set; }
    
    // Events for UI updates
    public System.Action<CareHistory> OnCareHistoryRecorded;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ CareHistoryRecorder initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Auto-register với tất cả các managers
        RegisterWithManagers();
    }
    
    /// <summary>
    /// Tự động đăng ký với tất cả pet care managers trong scene
    /// </summary>
    private void RegisterWithManagers()
    {
        // Register với PetInfoUIManager
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null)
        {
            // Chúng ta sẽ hook vào các method của PetInfoUIManager
            Debug.Log("🔗 CareHistoryRecorder found PetInfoUIManager");
        }
        
        // Register với FeedingManager
        FeedingManager feedingManager = FindObjectOfType<FeedingManager>();
        if (feedingManager != null)
        {
            Debug.Log("🔗 CareHistoryRecorder found FeedingManager");
        }
        
        // Register với PlayingManager  
        PlayingManager playingManager = FindObjectOfType<PlayingManager>();
        if (playingManager != null)
        {
            Debug.Log("🔗 CareHistoryRecorder found PlayingManager");
        }
        
        // Register với PetSleepManager
        PetSleepManager sleepManager = FindObjectOfType<PetSleepManager>();
        if (sleepManager != null)
        {
            // Subscribe to sleep events
            sleepManager.OnPetStartSleep += OnPetStartedSleeping;
            Debug.Log("🔗 CareHistoryRecorder subscribed to PetSleepManager events");
        }
    }
    
    /// <summary>
    /// Ghi lại lịch sử chăm sóc feeding
    /// </summary>
    public void RecordFeedingHistory(int playerPetId, int playerId)
    {
        if (!enableHistoryRecording) return;
        
        StartCoroutine(CreateCareHistoryRecord(playerPetId, playerId, FEEDING_ACTIVITY_ID, "Feeding"));
    }
    
    /// <summary>
    /// Ghi lại lịch sử chăm sóc playing  
    /// </summary>
    public void RecordPlayingHistory(int playerPetId, int playerId)
    {
        if (!enableHistoryRecording) return;
        
        StartCoroutine(CreateCareHistoryRecord(playerPetId, playerId, PLAYING_ACTIVITY_ID, "Playing"));
    }
    
    /// <summary>
    /// Ghi lại lịch sử chăm sóc sleeping
    /// </summary>
    public void RecordSleepingHistory(int playerPetId, int playerId)
    {
        if (!enableHistoryRecording) return;
        
        StartCoroutine(CreateCareHistoryRecord(playerPetId, playerId, SLEEPING_ACTIVITY_ID, "Sleeping"));
    }
    
    /// <summary>
    /// Event handler khi pet bắt đầu ngủ
    /// </summary>
    private void OnPetStartedSleeping(int petId)
    {
        // Lấy player ID từ pet ID
        int playerId = GetPlayerIdFromPetId(petId);
        if (playerId != -1)
        {
            RecordSleepingHistory(petId, playerId);
        }
    }
    
    /// <summary>
    /// Tạo care history record thông qua API
    /// </summary>
    private IEnumerator CreateCareHistoryRecord(int playerPetId, int playerId, int activityId, string activityName)
    {
        if (showRecordingMessages)
        {
            Debug.Log($"📝 Recording {activityName} history for Pet {playerPetId}, Player {playerId}");
        }
        
        bool success = false;
        
        // Gọi API để tạo care history record
        yield return StartCoroutine(APICareHistory.CreateCareHistoryCoroutine(
            playerPetId, 
            playerId, 
            activityId, 
            (result) => success = result
        ));
        
        if (success)
        {
            if (showRecordingMessages)
            {
                Debug.Log($"✅ {activityName} history recorded successfully");
            }
            
            // Tạo CareHistory object cho events
            CareHistory recordedHistory = new CareHistory
            {
                playerPetId = playerPetId,
                playerId = playerId,
                activityId = activityId,
                performedAt = System.DateTime.Now
            };
            
            // Fire event để UI có thể update
            OnCareHistoryRecorded?.Invoke(recordedHistory);
            
            // Refresh care history panel nếu đang mở
            RefreshCareHistoryPanelIfOpen();
        }
        else
        {
            Debug.LogError($"❌ Failed to record {activityName} history");
        }
    }
    
    /// <summary>
    /// Lấy Player ID từ Pet ID
    /// </summary>
    private int GetPlayerIdFromPetId(int petId)
    {
        try
        {
            // Thử tìm PlayerPet từ petId
            PlayerPet playerPet = APIPlayerPet.GetPlayerPetById(petId);
            if (playerPet != null)
            {
                return playerPet.playerID;
            }
            
            // Fallback: lấy current user
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser != null)
            {
                return currentUser.id;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error getting player ID from pet ID {petId}: {ex.Message}");
        }
        
        return -1;
    }
    
    /// <summary>
    /// Refresh care history panel nếu đang mở
    /// </summary>
    private void RefreshCareHistoryPanelIfOpen()
    {
        CareHistoryManager careHistoryManager = FindObjectOfType<CareHistoryManager>();
        if (careHistoryManager != null && careHistoryManager.IsPanelOpen())
        {
            careHistoryManager.RefreshIfOpen();
        }
    }
    
    /// <summary>
    /// Public method để các manager khác có thể gọi
    /// </summary>
    public static void RecordCareActivity(int playerPetId, int playerId, int activityId)
    {
        if (Instance != null)
        {
            string activityName = GetActivityName(activityId);
            Instance.StartCoroutine(Instance.CreateCareHistoryRecord(playerPetId, playerId, activityId, activityName));
        }
    }
    
    /// <summary>
    /// Helper method để lấy tên activity từ ID
    /// </summary>
    private static string GetActivityName(int activityId)
    {
        switch (activityId)
        {
            case FEEDING_ACTIVITY_ID: return "Feeding";
            case PLAYING_ACTIVITY_ID: return "Playing";
            case SLEEPING_ACTIVITY_ID: return "Sleeping";
            default: return $"Activity {activityId}";
        }
    }
    
    /// <summary>
    /// Enable/disable history recording
    /// </summary>
    public void SetHistoryRecording(bool enabled)
    {
        enableHistoryRecording = enabled;
        Debug.Log($"Care history recording {(enabled ? "enabled" : "disabled")}");
    }
    
    /// <summary>
    /// Enable/disable recording messages
    /// </summary>
    public void SetShowRecordingMessages(bool show)
    {
        showRecordingMessages = show;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        PetSleepManager sleepManager = FindObjectOfType<PetSleepManager>();
        if (sleepManager != null)
        {
            sleepManager.OnPetStartSleep -= OnPetStartedSleeping;
        }
    }
}