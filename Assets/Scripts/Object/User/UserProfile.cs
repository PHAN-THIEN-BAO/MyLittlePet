using TMPro;
using UnityEngine;
using System.Collections.Generic;


public class UserProfile : MonoBehaviour
{
    [SerializeField] public TMP_Text namePlayer;
    [SerializeField] public TMP_Text levelPlayer;
    [SerializeField] public TMP_Text petOwned;
    [SerializeField] public TMP_Text Achievements;

    public void SetUserProfile()
    {
        // Load user data from PlayerInfomation and set the UI elements
        User user = PlayerInfomation.LoadPlayerInfo();
        List<Achievement> listAchievement = APIAchievement.GetAllAchievements();
        List<PlayerAchievement> playerAchievements = APIPlayerAchievement.GetAchievementByIdPlayer(user.id);
        if (user != null)
        {
            namePlayer.text = user.userName;
            levelPlayer.text = "Lv: " + user.level.ToString();
            petOwned.text = "Pets Owned:                              " + APIUser.GetPlayerPetCount(user.id.ToString()).ToString();
        }
        else
        {
            Debug.LogError("User data is null. Please ensure PlayerInfomation is set up correctly.");
        }
        // Set the number of achievements in the UI
        int countAchievement = listAchievement != null ? listAchievement.Count : 0;
        int countPlayerAchievement = playerAchievements != null ? playerAchievements.Count : 0;
        Achievements.text = countPlayerAchievement.ToString() + "/" + countAchievement.ToString();

    }



}
