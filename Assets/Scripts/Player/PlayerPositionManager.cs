using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static manager để quản lý save/load vị trí player
/// </summary>
public static class PlayerPositionManager
{
    // Dictionary để lưu trữ vị trí player theo userId và savePointId
    private static Dictionary<string, Dictionary<string, Vector3>> savedPositions = 
        new Dictionary<string, Dictionary<string, Vector3>>();
    
    private static bool isDataLoaded = false;
    
    /// <summary>
    /// Save vị trí player cho user và save point cụ thể
    /// </summary>
    public static void SavePlayerPosition(string userId, string savePointId, Vector3 position)
    {
        // Đảm bảo dữ liệu đã được load
        if (!isDataLoaded)
        {
            LoadAllPositions();
            isDataLoaded = true;
        }
        
        // Tạo dictionary cho user nếu chưa có
        if (!savedPositions.ContainsKey(userId))
        {
            savedPositions[userId] = new Dictionary<string, Vector3>();
        }
        
        // Lưu vị trí
        savedPositions[userId][savePointId] = position;
        
        // Save to PlayerPrefs
        SaveToPlayerPrefs();
        
        Debug.Log($"Saved position for user {userId} at save point {savePointId}: {position}");
    }
    
    /// <summary>
    /// Load vị trí player cho user và save point cụ thể
    /// </summary>
    public static Vector3 LoadPlayerPosition(string userId, string savePointId)
    {
        // Đảm bảo dữ liệu đã được load
        if (!isDataLoaded)
        {
            LoadAllPositions();
            isDataLoaded = true;
        }
        
        if (savedPositions.ContainsKey(userId) && 
            savedPositions[userId].ContainsKey(savePointId))
        {
            Vector3 position = savedPositions[userId][savePointId];
            Debug.Log($"Loaded position for user {userId} from save point {savePointId}: {position}");
            return position;
        }
        
        Debug.LogWarning($"No saved position found for user {userId} at save point {savePointId}");
        return Vector3.zero;
    }
    
    /// <summary>
    /// Kiểm tra xem có saved position không
    /// </summary>
    public static bool HasSavedPosition(string userId, string savePointId)
    {
        // Đảm bảo dữ liệu đã được load
        if (!isDataLoaded)
        {
            LoadAllPositions();
            isDataLoaded = true;
        }
        
        return savedPositions.ContainsKey(userId) && 
               savedPositions[userId].ContainsKey(savePointId);
    }
    
    /// <summary>
    /// Xóa saved position cho user và save point cụ thể
    /// </summary>
    public static void ClearSavedPosition(string userId, string savePointId)
    {
        if (savedPositions.ContainsKey(userId) && 
            savedPositions[userId].ContainsKey(savePointId))
        {
            savedPositions[userId].Remove(savePointId);
            SaveToPlayerPrefs();
            Debug.Log($"Cleared saved position for user {userId} at save point {savePointId}");
        }
    }
    
    /// <summary>
    /// Xóa tất cả saved positions cho user
    /// </summary>
    public static void ClearAllUserPositions(string userId)
    {
        if (savedPositions.ContainsKey(userId))
        {
            savedPositions[userId].Clear();
            SaveToPlayerPrefs();
            Debug.Log($"Cleared all saved positions for user {userId}");
        }
    }
    
    /// <summary>
    /// Xóa tất cả saved positions
    /// </summary>
    public static void ClearAllPositions()
    {
        savedPositions.Clear();
        PlayerPrefs.DeleteKey("SavedPlayerPositions");
        PlayerPrefs.Save();
        isDataLoaded = false;
        Debug.Log("Cleared all saved player positions");
    }
    
    /// <summary>
    /// Lấy tất cả save points cho user
    /// </summary>
    public static List<string> GetUserSavePoints(string userId)
    {
        if (!isDataLoaded)
        {
            LoadAllPositions();
            isDataLoaded = true;
        }
        
        if (savedPositions.ContainsKey(userId))
        {
            return new List<string>(savedPositions[userId].Keys);
        }
        
        return new List<string>();
    }
    
    /// <summary>
    /// Save tất cả dữ liệu vào PlayerPrefs
    /// </summary>
    private static void SaveToPlayerPrefs()
    {
        try
        {
            PlayerPositionData data = new PlayerPositionData();
            data.userPositions = new List<UserPositionData>();
            
            foreach (var userKvp in savedPositions)
            {
                UserPositionData userData = new UserPositionData();
                userData.userId = userKvp.Key;
                userData.savePoints = new List<SavePointData>();
                
                foreach (var savePointKvp in userKvp.Value)
                {
                    SavePointData savePointData = new SavePointData();
                    savePointData.savePointId = savePointKvp.Key;
                    savePointData.position = savePointKvp.Value;
                    userData.savePoints.Add(savePointData);
                }
                
                data.userPositions.Add(userData);
            }
            
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString("SavedPlayerPositions", json);
            PlayerPrefs.Save();
            
            Debug.Log("Saved player positions data to PlayerPrefs");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to save player positions: " + ex.Message);
        }
    }
    
    /// <summary>
    /// Load tất cả dữ liệu từ PlayerPrefs
    /// </summary>
    private static void LoadAllPositions()
    {
        try
        {
            savedPositions.Clear();
            
            if (PlayerPrefs.HasKey("SavedPlayerPositions"))
            {
                string json = PlayerPrefs.GetString("SavedPlayerPositions");
                if (!string.IsNullOrEmpty(json))
                {
                    PlayerPositionData data = JsonUtility.FromJson<PlayerPositionData>(json);
                    
                    if (data != null && data.userPositions != null)
                    {
                        foreach (var userData in data.userPositions)
                        {
                            if (userData != null && !string.IsNullOrEmpty(userData.userId) && userData.savePoints != null)
                            {
                                savedPositions[userData.userId] = new Dictionary<string, Vector3>();
                                
                                foreach (var savePointData in userData.savePoints)
                                {
                                    if (savePointData != null && !string.IsNullOrEmpty(savePointData.savePointId))
                                    {
                                        savedPositions[userData.userId][savePointData.savePointId] = savePointData.position;
                                    }
                                }
                            }
                        }
                    }
                }
                
                Debug.Log("Loaded player positions data from PlayerPrefs");
            }
            else
            {
                Debug.Log("No saved player positions data found");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to load player positions: " + ex.Message);
            savedPositions.Clear();
        }
    }
    
    /// <summary>
    /// Force save dữ liệu (có thể gọi trước khi chuyển scene)
    /// </summary>
    public static void ForceSave()
    {
        SaveToPlayerPrefs();
    }
}

/// <summary>
/// Data structure để serialize player positions
/// </summary>
[System.Serializable]
public class PlayerPositionData
{
    public List<UserPositionData> userPositions;
}

/// <summary>
/// Data structure cho thông tin position của user
/// </summary>
[System.Serializable]
public class UserPositionData
{
    public string userId;
    public List<SavePointData> savePoints;
}

/// <summary>
/// Data structure cho thông tin save point
/// </summary>
[System.Serializable]
public class SavePointData
{
    public string savePointId;
    public Vector3 position;
}