using UnityEngine;
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
    public void SetupCareHistorySystem()
    {
        if (showSetupMessages)
            Debug.Log("?? Setting up Care History System...");
        EnsureCareHistoryRecorder();
        SetupCareHistoryManager();
        ConnectToPetCareManagers();
        if (showSetupMessages)
            Debug.Log("? Care History System setup completed!");
    }
    private void EnsureCareHistoryRecorder()
    {
        if (CareHistoryRecorder.Instance == null && createRecorderIfMissing)
        {
            GameObject recorderObj = new GameObject("CareHistoryRecorder");
            recorderObj.AddComponent<CareHistoryRecorder>();
            if (showSetupMessages)
                Debug.Log("?? Created CareHistoryRecorder automatically");
        }
    }
    private void SetupCareHistoryManager()
    {
        CareHistoryManager careManager = FindObjectOfType<CareHistoryManager>();
        if (careManager == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                GameObject managerObj = new GameObject("CareHistoryManager");
                managerObj.transform.SetParent(canvas.transform, false);
                managerObj.AddComponent<CareHistoryManager>();
                if (showSetupMessages)
                    Debug.Log("?? Created CareHistoryManager automatically");
            }
        }
    }
    private void ConnectToPetCareManagers()
    {
        PetInfoUIManager petInfoManager = FindObjectOfType<PetInfoUIManager>();
        if (petInfoManager != null)
        {
            if (showSetupMessages)
                Debug.Log("?? Connected to PetInfoUIManager");
        }
        FeedingManager feedingManager = FindObjectOfType<FeedingManager>();
        if (feedingManager != null)
        {
            if (showSetupMessages)
                Debug.Log("?? Connected to FeedingManager");
        }
        PlayingManager playingManager = FindObjectOfType<PlayingManager>();
        if (playingManager != null)
        {
            if (showSetupMessages)
                Debug.Log("?? Connected to PlayingManager");
        }
        PetSleepManager sleepManager = FindObjectOfType<PetSleepManager>();
        if (sleepManager != null)
        {
            if (showSetupMessages)
                Debug.Log("?? Connected to PetSleepManager");
        }
    }
    [ContextMenu("Setup Care History System")]
    public void ManualSetup()
    {
        SetupCareHistorySystem();
    }
}