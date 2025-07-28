using UnityEngine;
using System.Collections;

public class CareHistoryRecorder : MonoBehaviour
{
    [Header("Care History Settings")]
    [SerializeField] private bool enableHistoryRecording = true;
    [SerializeField] private bool showRecordingMessages = false;
    
    private const int FEEDING_ACTIVITY_ID = 1;
    private const int PLAYING_ACTIVITY_ID = 3;
    private const int SLEEPING_ACTIVITY_ID = 2;
    
    public static CareHistoryRecorder Instance { get; private set; }
    
    public System.Action<CareHistory> OnCareHistoryRecorded;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("? CareHistoryRecorder initialized");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        RegisterWithManagers();
    }
    
    private void RegisterWithManagers()
    {
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null)
        {
            Debug.Log("?? CareHistoryRecorder found PetInfoUIManager");
        }
        
        FeedingManager feedingManager = FindObjectOfType<FeedingManager>();
        if (feedingManager != null)
        {
            Debug.Log("?? CareHistoryRecorder found FeedingManager");
        }
        
        PlayingManager playingManager = FindObjectOfType<PlayingManager>();
        if (playingManager != null)
        {
            Debug.Log("?? CareHistoryRecorder found PlayingManager");
        }
        
        PetSleepManager sleepManager = FindObjectOfType<PetSleepManager>();
        if (sleepManager != null)
        {
            sleepManager.OnPetStartSleep += OnPetStartedSleeping;
            Debug.Log("?? CareHistoryRecorder subscribed to PetSleepManager events");
        }
    }
    
    public void RecordFeedingHistory(int playerPetId, int playerId)
    {
        if (!enableHistoryRecording) return;
        
        StartCoroutine(CreateCareHistoryRecord(playerPetId, playerId, FEEDING_ACTIVITY_ID, "Feeding"));
    }
    
    public void RecordPlayingHistory(int playerPetId, int playerId)
    {
        if (!enableHistoryRecording) return;
        
        StartCoroutine(CreateCareHistoryRecord(playerPetId, playerId, PLAYING_ACTIVITY_ID, "Playing"));
    }
    
    public void RecordSleepingHistory(int playerPetId, int playerId)
    {
        if (!enableHistoryRecording) return;
        
        StartCoroutine(CreateCareHistoryRecord(playerPetId, playerId, SLEEPING_ACTIVITY_ID, "Sleeping"));
    }
    
    private void OnPetStartedSleeping(int petId)
    {
        int playerId = GetPlayerIdFromPetId(petId);
        if (playerId != -1)
        {
            RecordSleepingHistory(petId, playerId);
        }
    }
    
    private IEnumerator CreateCareHistoryRecord(int playerPetId, int playerId, int activityId, string activityName)
    {
        if (showRecordingMessages)
        {
            Debug.Log($"?? Recording {activityName} history for Pet {playerPetId}, Player {playerId}");
        }
        
        bool success = false;
        
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
                Debug.Log($"? {activityName} history recorded successfully");
            }
            
            CareHistory recordedHistory = new CareHistory
            {
                playerPetId = playerPetId,
                playerId = playerId,
                activityId = activityId,
                performedAt = System.DateTime.Now
            };
            
            OnCareHistoryRecorded?.Invoke(recordedHistory);
            
            RefreshCareHistoryPanelIfOpen();
        }
        else
        {
            Debug.LogError($"? Failed to record {activityName} history");
        }
    }
    
    private int GetPlayerIdFromPetId(int petId)
    {
        try
        {
            PlayerPet playerPet = APIPlayerPet.GetPlayerPetById(petId);
            if (playerPet != null)
            {
                return playerPet.playerID;
            }
            
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
    
    private void RefreshCareHistoryPanelIfOpen()
    {
        CareHistoryManager careHistoryManager = FindObjectOfType<CareHistoryManager>();
        if (careHistoryManager != null && careHistoryManager.IsPanelOpen())
        {
            careHistoryManager.RefreshIfOpen();
        }
    }
    
    public static void RecordCareActivity(int playerPetId, int playerId, int activityId)
    {
        if (Instance != null)
        {
            string activityName = GetActivityName(activityId);
            Instance.StartCoroutine(Instance.CreateCareHistoryRecord(playerPetId, playerId, activityId, activityName));
        }
    }
    
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
    
    public void SetHistoryRecording(bool enabled)
    {
        enableHistoryRecording = enabled;
        Debug.Log($"Care history recording {(enabled ? "enabled" : "disabled")}");
    }
    
    public void SetShowRecordingMessages(bool show)
    {
        showRecordingMessages = show;
    }
    
    private void OnDestroy()
    {
        PetSleepManager sleepManager = FindObjectOfType<PetSleepManager>();
        if (sleepManager != null)
        {
            sleepManager.OnPetStartSleep -= OnPetStartedSleeping;
        }
    }
}