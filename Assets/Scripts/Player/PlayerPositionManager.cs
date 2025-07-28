using System.Collections.Generic;
using UnityEngine;

public static class PlayerPositionManager
{
    private static Dictionary<string, Dictionary<string, Vector3>> savedPositions = 
        new Dictionary<string, Dictionary<string, Vector3>>();
    
    private static bool isDataLoaded = false;
    
    public static void SavePlayerPosition(string userId, string savePointId, Vector3 position)
    {
        if (!isDataLoaded)
        {
            LoadAllPositions();
            isDataLoaded = true;
        }
        
        if (!savedPositions.ContainsKey(userId))
        {
            savedPositions[userId] = new Dictionary<string, Vector3>();
        }
        
        savedPositions[userId][savePointId] = position;
        
        SaveToPlayerPrefs();
        
        Debug.Log($"Saved position for user {userId} at save point {savePointId}: {position}");
    }
    
    public static Vector3 LoadPlayerPosition(string userId, string savePointId)
    {
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
    
    public static bool HasSavedPosition(string userId, string savePointId)
    {
        if (!isDataLoaded)
        {
            LoadAllPositions();
            isDataLoaded = true;
        }
        
        return savedPositions.ContainsKey(userId) && 
               savedPositions[userId].ContainsKey(savePointId);
    }
    
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
    
    public static void ClearAllUserPositions(string userId)
    {
        if (savedPositions.ContainsKey(userId))
        {
            savedPositions[userId].Clear();
            SaveToPlayerPrefs();
            Debug.Log($"Cleared all saved positions for user {userId}");
        }
    }
    
    public static void ClearAllPositions()
    {
        savedPositions.Clear();
        PlayerPrefs.DeleteKey("SavedPlayerPositions");
        PlayerPrefs.Save();
        isDataLoaded = false;
        Debug.Log("Cleared all saved player positions");
    }
    
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
    
    public static void ForceSave()
    {
        SaveToPlayerPrefs();
    }
}

[System.Serializable]
public class PlayerPositionData
{
    public List<UserPositionData> userPositions;
}

[System.Serializable]
public class UserPositionData
{
    public string userId;
    public List<SavePointData> savePoints;
}

[System.Serializable]
public class SavePointData
{
    public string savePointId;
    public Vector3 position;
}