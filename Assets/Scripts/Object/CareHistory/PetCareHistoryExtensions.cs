using UnityEngine;
public static class PetCareHistoryExtensions
{
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
    public static (int playerPetId, int playerId) GetCurrentPetAndPlayerIds(this PetInfoUIManager petInfoManager)
    {
        try
        {
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser == null) return (-1, -1);
            return petInfoManager.GetCurrentPetAndPlayerId();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error getting current pet and player IDs: {ex.Message}");
            return (-1, -1);
        }
    }
}