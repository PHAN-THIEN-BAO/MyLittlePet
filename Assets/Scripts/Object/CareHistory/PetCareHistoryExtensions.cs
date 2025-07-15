using UnityEngine;

/// <summary>
/// Extension methods để tích hợp care history recording vào các pet care managers
/// </summary>
public static class PetCareHistoryExtensions
{
    /// <summary>
    /// Extension method cho PetInfoUIManager để record feeding history
    /// </summary>
    public static void RecordFeedingAction(this PetInfoUIManager petInfoManager)
    {
        if (CareHistoryRecorder.Instance != null && petInfoManager != null)
        {
            var (playerPetId, playerId) = petInfoManager.GetCurrentPetAndPlayerIds();
            if (playerPetId != -1 && playerId != -1)
            {
                CareHistoryRecorder.Instance.RecordFeedingHistory(playerPetId, playerId);
            }
        }
    }
    
    /// <summary>
    /// Extension method cho PetInfoUIManager để record playing history
    /// </summary>
    public static void RecordPlayingAction(this PetInfoUIManager petInfoManager)
    {
        if (CareHistoryRecorder.Instance != null && petInfoManager != null)
        {
            var (playerPetId, playerId) = petInfoManager.GetCurrentPetAndPlayerIds();
            if (playerPetId != -1 && playerId != -1)
            {
                CareHistoryRecorder.Instance.RecordPlayingHistory(playerPetId, playerId);
            }
        }
    }
    
    /// <summary>
    /// Extension method cho PetInfoUIManager để record sleeping history
    /// </summary>
    public static void RecordSleepingAction(this PetInfoUIManager petInfoManager)
    {
        if (CareHistoryRecorder.Instance != null && petInfoManager != null)
        {
            var (playerPetId, playerId) = petInfoManager.GetCurrentPetAndPlayerIds();
            if (playerPetId != -1 && playerId != -1)
            {
                CareHistoryRecorder.Instance.RecordSleepingHistory(playerPetId, playerId);
            }
        }
    }
    
    /// <summary>
    /// Helper method để lấy current pet và player IDs
    /// </summary>
    public static (int playerPetId, int playerId) GetCurrentPetAndPlayerIds(this PetInfoUIManager petInfoManager)
    {
        try
        {
            // Lấy current user
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser == null) return (-1, -1);
            
            // Lấy current pet (cần truy cập private field hoặc thêm public getter)
            // Vì currentPetDetails là private, chúng ta sẽ cần thêm public method
            return petInfoManager.GetCurrentPetAndPlayerId();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error getting current pet and player IDs: {ex.Message}");
            return (-1, -1);
        }
    }
}