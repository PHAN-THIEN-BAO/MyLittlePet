using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AchievementDisplay : MonoBehaviour
{
    [SerializeField] public List<TMP_Text> achievementTexts; // pulls the TMP_Text fields from the scene
    [SerializeField] public List<GameObject> readyCollected;
    [SerializeField] public List<GameObject> collected;
    [SerializeField] public List<GameObject> notCollected;
    /// <summary>
    /// Sets the achievement descriptions in the TMP_Text fields.
    /// </summary>
    public void SetAchievement()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        List<Achievement> achievements = APIAchievement.GetAllAchievements();
        List<PlayerAchievement> playerAchievements = APIPlayerAchievement.GetAchievementByIdPlayer(user.id);

        int count = Mathf.Min(achievementTexts.Count, achievements.Count);

        for (int i = 0; i < count; i++)
        {
            achievementTexts[i].text = achievements[i].description;

            // find the corresponding PlayerAchievement for the current achievement
            PlayerAchievement playerAch = playerAchievements.Find(pa => pa.achievementID == achievements[i].achievementID);

            if (playerAch != null)
            {
                if (!playerAch.isCollected)
                {
                    // got achievement not collected
                    readyCollected[i].SetActive(true);
                    collected[i].SetActive(false);
                    notCollected[i].SetActive(false);
                }
                else
                {
                    // collected
                    readyCollected[i].SetActive(false);
                    collected[i].SetActive(true);
                    notCollected[i].SetActive(false);
                }
            }
            else
            {
                // not achieved
                readyCollected[i].SetActive(false);
                collected[i].SetActive(false);
                notCollected[i].SetActive(true);
            }
        }

        // Clear remaining achievement texts and states if there are more texts than achievements
        for (int i = count; i < achievementTexts.Count; i++)
        {
            achievementTexts[i].text = "";
            readyCollected[i].SetActive(false);
            collected[i].SetActive(false);
            notCollected[i].SetActive(false);
        }
    }
}
