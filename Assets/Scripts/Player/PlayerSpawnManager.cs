using UnityEngine;
using System.Collections;

public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform defaultSpawnPoint;
    
    [Header("Position Loading")]
    [SerializeField] private bool autoLoadOnStart = true;
    [SerializeField] private string preferredSavePointId = "";
    [SerializeField] private bool loadLatestSavePoint = true;
    [SerializeField] private float initialLoadDelay = 0.1f;
    [SerializeField] private float reapplyDelay = 0.5f;
    [SerializeField] private bool forceReapplyPosition = true;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private Vector3 savedPlayerPosition = Vector3.zero;
    private bool hasSavedPosition = false;
    private GameObject playerObject = null;
    
    private void Start()
    {
        if (autoLoadOnStart)
        {
            if (enableDebugLogs)
                Debug.Log("PlayerSpawnManager: Starting to load player position...");
            
            StartCoroutine(LoadPlayerPositionCoroutine());
        }
    }
    
    private IEnumerator LoadPlayerPositionCoroutine()
    {
        yield return new WaitForSeconds(initialLoadDelay);
        
        LoadPlayerPosition();
        
        if (hasSavedPosition && forceReapplyPosition)
        {
            yield return new WaitForSeconds(reapplyDelay);
            
            ReapplyPlayerPosition();
        }
    }
    
    public void LoadPlayerPosition()
    {
        playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject == null)
        {
            Debug.LogError($"Cannot find player with tag '{playerTag}' in scene!");
            return;
        }
        
        if (enableDebugLogs)
            Debug.Log($"Found player: {playerObject.name} at position: {playerObject.transform.position}");
        
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser == null)
        {
            Debug.LogWarning("No user information found. Player will spawn at default position.");
            SetPlayerToDefaultPosition(playerObject.transform);
            return;
        }
        
        string userId = currentUser.id.ToString();
        if (enableDebugLogs)
            Debug.Log($"Current user ID: {userId}");
        
        string savePointToLoad = GetSavePointToLoad(userId);
        
        if (enableDebugLogs)
            Debug.Log($"Save point to load: '{savePointToLoad}'");
        
        if (!string.IsNullOrEmpty(savePointToLoad))
        {
            bool hasSavedPositionCheck = PlayerPositionManager.HasSavedPosition(userId, savePointToLoad);
            if (enableDebugLogs)
                Debug.Log($"Has saved position for '{savePointToLoad}': {hasSavedPositionCheck}");
            
            if (hasSavedPositionCheck)
            {
                Vector3 loadedPosition = PlayerPositionManager.LoadPlayerPosition(userId, savePointToLoad);
                if (enableDebugLogs)
                    Debug.Log($"Retrieved saved position: {loadedPosition}");
                
                if (loadedPosition != Vector3.zero)
                {
                    savedPlayerPosition = loadedPosition;
                    hasSavedPosition = true;
                    
                    ApplyPlayerPosition(loadedPosition);
                    
                    if (enableDebugLogs)
                    {
                        Debug.Log($"SUCCESS: Loaded player position from save point '{savePointToLoad}': {loadedPosition}");
                    }
                    return;
                }
                else
                {
                    if (enableDebugLogs)
                        Debug.LogWarning($"Saved position returned Vector3.zero for save point '{savePointToLoad}'");
                }
            }
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("No save point to load found.");
        }
        
        SetPlayerToDefaultPosition(playerObject.transform);
        
        if (enableDebugLogs)
        {
            Debug.Log("No saved position found. Player spawned at default position.");
        }
    }
    
    private void ApplyPlayerPosition(Vector3 position)
    {
        if (playerObject != null)
        {
            Vector3 oldPosition = playerObject.transform.position;
            playerObject.transform.position = position;
            
            if (enableDebugLogs)
            {
                Debug.Log($"Applied player position: {oldPosition} -> {position}");
            }
        }
    }
    
    private void ReapplyPlayerPosition()
    {
        if (!hasSavedPosition || playerObject == null)
            return;
        
        Vector3 currentPosition = playerObject.transform.position;
        float distance = Vector3.Distance(currentPosition, savedPlayerPosition);
        
        if (enableDebugLogs)
        {
            Debug.Log($"Checking if position needs reapplying...");
            Debug.Log($"Current position: {currentPosition}");
            Debug.Log($"Saved position: {savedPlayerPosition}");
            Debug.Log($"Distance: {distance}");
        }
        
        if (distance > 0.1f)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"Player position was changed! Reapplying saved position...");
            }
            
            ApplyPlayerPosition(savedPlayerPosition);
            
            if (enableDebugLogs)
            {
                Debug.Log($"REAPPLIED: Player position restored to saved position: {savedPlayerPosition}");
            }
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.Log("Player position is correct, no reapply needed.");
            }
        }
    }
    
    private string GetSavePointToLoad(string userId)
    {
        if (enableDebugLogs)
            Debug.Log($"Getting save point to load for user: {userId}");
        
        if (!string.IsNullOrEmpty(preferredSavePointId))
        {
            if (enableDebugLogs)
                Debug.Log($"Checking preferred save point: '{preferredSavePointId}'");
            
            if (PlayerPositionManager.HasSavedPosition(userId, preferredSavePointId))
            {
                if (enableDebugLogs)
                    Debug.Log($"Using preferred save point: '{preferredSavePointId}'");
                return preferredSavePointId;
            }
            else
            {
                if (enableDebugLogs)
                    Debug.Log($"Preferred save point '{preferredSavePointId}' not found");
            }
        }
        
        if (loadLatestSavePoint)
        {
            if (enableDebugLogs)
                Debug.Log("Attempting to load latest save point...");
            return GetLatestSavePoint(userId);
        }
        
        if (enableDebugLogs)
            Debug.Log("No save point selection method enabled");
        
        return null;
    }
    
    private string GetLatestSavePoint(string userId)
    {
        var savePoints = PlayerPositionManager.GetUserSavePoints(userId);
        
        if (enableDebugLogs)
            Debug.Log($"Available save points for user {userId}: {savePoints.Count} points");
        
        if (savePoints.Count > 0)
        {
            for (int i = 0; i < savePoints.Count; i++)
            {
                if (enableDebugLogs)
                    Debug.Log($"Save point {i}: '{savePoints[i]}'");
            }
            
            string latestSavePoint = savePoints[savePoints.Count - 1];
            if (enableDebugLogs)
                Debug.Log($"Selected latest save point: '{latestSavePoint}'");
            
            return latestSavePoint;
        }
        
        if (enableDebugLogs)
            Debug.Log("No save points found for user");
        
        return null;
    }
    
    private void SetPlayerToDefaultPosition(Transform playerTransform)
    {
        if (defaultSpawnPoint != null)
        {
            Vector3 oldPosition = playerTransform.position;
            playerTransform.position = defaultSpawnPoint.position;
            playerTransform.rotation = defaultSpawnPoint.rotation;
            
            if (enableDebugLogs)
                Debug.Log($"Set player to default spawn point: {oldPosition} -> {defaultSpawnPoint.position}");
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning("No default spawn point set. Player remains at current position.");
            }
        }
    }
    
    public bool LoadFromSavePoint(string savePointId)
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return false;
        
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser == null) return false;
        
        string userId = currentUser.id.ToString();
        
        if (PlayerPositionManager.HasSavedPosition(userId, savePointId))
        {
            Vector3 loadedPosition = PlayerPositionManager.LoadPlayerPosition(userId, savePointId);
            if (loadedPosition != Vector3.zero)
            {
                ApplyPlayerPosition(loadedPosition);
                
                savedPlayerPosition = loadedPosition;
                hasSavedPosition = true;
                
                if (enableDebugLogs)
                {
                    Debug.Log($"Manually loaded player position from save point '{savePointId}': {loadedPosition}");
                }
                return true;
            }
        }
        
        return false;
    }
    
    public void ReloadPlayerPosition()
    {
        LoadPlayerPosition();
    }
    
    public void ForceReapplyPosition()
    {
        ReapplyPlayerPosition();
    }
    
    public void StartPositionMonitoring(float monitorInterval = 1f)
    {
        if (hasSavedPosition)
        {
            InvokeRepeating(nameof(ReapplyPlayerPosition), monitorInterval, monitorInterval);
            
            if (enableDebugLogs)
                Debug.Log($"Started position monitoring with interval: {monitorInterval}s");
        }
    }
    
    public void StopPositionMonitoring()
    {
        CancelInvoke(nameof(ReapplyPlayerPosition));
        
        if (enableDebugLogs)
            Debug.Log("Stopped position monitoring");
    }
}