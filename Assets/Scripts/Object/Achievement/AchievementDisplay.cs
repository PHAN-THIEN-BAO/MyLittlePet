using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class AchievementDisplay : MonoBehaviour
{
    [SerializeField] public List<GameObject> achievementItems;

    public void SetAchievement()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        List<Achievement> achievements = APIAchievement.GetAllAchievements();
        List<PlayerAchievement> playerAchievements = APIPlayerAchievement.GetAchievementByIdPlayer(user.id);

        int count = Mathf.Min(achievementItems.Count, achievements.Count);

        for (int i = 0; i < count; i++)
        {
            TMP_Text achievementText = achievementItems[i].transform.Find("Achievements_Detail").GetComponent<TMP_Text>();
            TMP_Text achievement_Name = achievementItems[i].transform.Find("Achievement_Name").GetComponent<TMP_Text>();
            TMP_Text achievementIdText = achievementItems[i].transform.Find("Achievement_Id").GetComponent<TMP_Text>();
            GameObject readyCollectedBtn = achievementItems[i].transform.Find("Ready_Collected_Button").gameObject;
            GameObject collectedBtn = achievementItems[i].transform.Find("Collected_Button").gameObject;
            GameObject notCollectedBtn = achievementItems[i].transform.Find("Not_Collected_Button").gameObject;

            achievementText.text = achievements[i].description;
            achievement_Name.text = achievements[i].achievementName;
            achievementIdText.text = achievements[i].achievementID.ToString();

            PlayerAchievement playerAch = playerAchievements.Find(pa => pa.achievementID == achievements[i].achievementID);

            if (playerAch != null)
            {
                if (!playerAch.isCollected)
                {
                    readyCollectedBtn.SetActive(true);
                    collectedBtn.SetActive(false);
                    notCollectedBtn.SetActive(false);
                }
                else
                {
                    readyCollectedBtn.SetActive(false);
                    collectedBtn.SetActive(true);
                    notCollectedBtn.SetActive(false);
                }
            }
            else
            {
                readyCollectedBtn.SetActive(false);
                collectedBtn.SetActive(false);
                notCollectedBtn.SetActive(true);
            }
        }

        for (int i = count; i < achievementItems.Count; i++)
        {
            achievementItems[i].SetActive(false);
        }
    }
}