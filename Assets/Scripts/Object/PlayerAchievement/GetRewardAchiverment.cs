using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GetRewardAchiverment : MonoBehaviour
{
    [SerializeField] public TMP_Text amountReward;
    [SerializeField] public TMP_Text achievementId;
    [SerializeField] public GameObject rewardPanel;

    /// <summary>
    /// Gets the achievement reward for a player
    /// </summary>
    /// <param name="playerId">The ID of the player</param>
    /// <param name="achievementId">The ID of the achievement</param>
    /// <param name="isCollected">Whether the achievement is collected</param>
    public void GetReward(int playerId, int achievementId, bool isCollected)
    {
        // Call API to update achievement collection status
        bool success = APIPlayerAchievement.UpdateAchievement(playerId, achievementId, true);

        if (success)
        {
            // Show reward panel
            if (rewardPanel != null)
            {
                rewardPanel.SetActive(true);
            }

            // Extract only the number from the format "x[Số]"
            string rewardText = amountReward.text;
            string numericValue = rewardText.StartsWith("x") ? rewardText.Substring(1) : rewardText;
            int rewardAmount = int.Parse(numericValue);

            // Update player information - add diamonds
            PlayerInformation.UpdatePlayerInfo(rewardAmount);

            // Update user information on server
            APIUser.UpdateUser();

            // Debug log
            Debug.Log($"Achievement {achievementId} collected successfully! Reward: {rewardAmount} diamonds");
        }
        else
        {
            Debug.LogError($"Failed to collect achievement {achievementId}");
        }
    }
}