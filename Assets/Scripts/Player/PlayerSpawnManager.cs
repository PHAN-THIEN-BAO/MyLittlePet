using UnityEngine;
using System.Collections;

/// <summary>
/// Manages player spawning and position loading when scene starts
/// </summary>
public class PlayerSpawnManager : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform defaultSpawnPoint; // Vị trí spawn mặc định
    
    [Header("Position Loading")]
    [SerializeField] private bool autoLoadOnStart = true;
    [SerializeField] private string preferredSavePointId = ""; // Save point ưu tiên để load
    [SerializeField] private bool loadLatestSavePoint = true; // Load save point gần nhất
    [SerializeField] private float initialLoadDelay = 0.1f; // Delay ban đầu
    [SerializeField] private float reapplyDelay = 0.5f; // Delay để reapply position sau khi Timeline có thể đã thay đổi
    [SerializeField] private bool forceReapplyPosition = true; // Force reapply position sau delay
    
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
            
            // Load ngay lập tức
            StartCoroutine(LoadPlayerPositionCoroutine());
        }
    }
    
    /// <summary>
    /// Coroutine để load và reapply player position
    /// </summary>
    private IEnumerator LoadPlayerPositionCoroutine()
    {
        // Delay ban đầu
        yield return new WaitForSeconds(initialLoadDelay);
        
        // Load position lần đầu
        LoadPlayerPosition();
        
        // Nếu có saved position và cần force reapply
        if (hasSavedPosition && forceReapplyPosition)
        {
            // Đợi thêm một chút để Timeline có thể chạy
            yield return new WaitForSeconds(reapplyDelay);
            
            // Reapply position nếu player đã bị di chuyển
            ReapplyPlayerPosition();
        }
    }
    
    /// <summary>
    /// Load vị trí player từ saved data
    /// </summary>
    public void LoadPlayerPosition()
    {
        // Tìm player trong scene
        playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject == null)
        {
            Debug.LogError($"Cannot find player with tag '{playerTag}' in scene!");
            return;
        }
        
        if (enableDebugLogs)
            Debug.Log($"Found player: {playerObject.name} at position: {playerObject.transform.position}");
        
        // Lấy thông tin user hiện tại
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
            // Kiểm tra xem save point có tồn tại không
            bool hasSavedPositionCheck = PlayerPositionManager.HasSavedPosition(userId, savePointToLoad);
            if (enableDebugLogs)
                Debug.Log($"Has saved position for '{savePointToLoad}': {hasSavedPositionCheck}");
            
            if (hasSavedPositionCheck)
            {
                // Load từ save point
                Vector3 loadedPosition = PlayerPositionManager.LoadPlayerPosition(userId, savePointToLoad);
                if (enableDebugLogs)
                    Debug.Log($"Retrieved saved position: {loadedPosition}");
                
                if (loadedPosition != Vector3.zero) // Vector3.zero có nghĩa là không tìm thấy
                {
                    // Lưu position để có thể reapply sau
                    savedPlayerPosition = loadedPosition;
                    hasSavedPosition = true;
                    
                    // Apply position
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
        
        // Nếu không load được, đặt ở vị trí mặc định
        SetPlayerToDefaultPosition(playerObject.transform);
        
        if (enableDebugLogs)
        {
            Debug.Log("No saved position found. Player spawned at default position.");
        }
    }
    
    /// <summary>
    /// Apply player position và log thông tin
    /// </summary>
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
    
    /// <summary>
    /// Reapply saved position nếu player đã bị di chuyển
    /// </summary>
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
        
        // Nếu player đã bị di chuyển khỏi saved position (threshold 0.1f)
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
    
    /// <summary>
    /// Xác định save point nào sẽ được load
    /// </summary>
    private string GetSavePointToLoad(string userId)
    {
        if (enableDebugLogs)
            Debug.Log($"Getting save point to load for user: {userId}");
        
        // Nếu có preferred save point ID và nó tồn tại
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
        
        // Nếu load latest save point
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
    
    /// <summary>
    /// Lấy save point gần nhất (theo tên - có thể cải thiện bằng timestamp)
    /// </summary>
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
            
            string latestSavePoint = savePoints[savePoints.Count - 1]; // Lấy cái cuối cùng
            if (enableDebugLogs)
                Debug.Log($"Selected latest save point: '{latestSavePoint}'");
            
            return latestSavePoint;
        }
        
        if (enableDebugLogs)
            Debug.Log("No save points found for user");
        
        return null;
    }
    
    /// <summary>
    /// Đặt player ở vị trí mặc định
    /// </summary>
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
            // Nếu không có default spawn point, giữ nguyên vị trí hiện tại
            if (enableDebugLogs)
            {
                Debug.LogWarning("No default spawn point set. Player remains at current position.");
            }
        }
    }
    
    /// <summary>
    /// Load từ save point cụ thể
    /// </summary>
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
                
                // Update saved position for reapply
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
    
    /// <summary>
    /// Public method để force reload position
    /// </summary>
    public void ReloadPlayerPosition()
    {
        LoadPlayerPosition();
    }
    
    /// <summary>
    /// Public method để force reapply saved position
    /// </summary>
    public void ForceReapplyPosition()
    {
        ReapplyPlayerPosition();
    }
    
    /// <summary>
    /// Method để monitor và reapply position liên tục (nếu cần)
    /// </summary>
    public void StartPositionMonitoring(float monitorInterval = 1f)
    {
        if (hasSavedPosition)
        {
            InvokeRepeating(nameof(ReapplyPlayerPosition), monitorInterval, monitorInterval);
            
            if (enableDebugLogs)
                Debug.Log($"Started position monitoring with interval: {monitorInterval}s");
        }
    }
    
    /// <summary>
    /// Stop position monitoring
    /// </summary>
    public void StopPositionMonitoring()
    {
        CancelInvoke(nameof(ReapplyPlayerPosition));
        
        if (enableDebugLogs)
            Debug.Log("Stopped position monitoring");
    }
}