using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CollectAchievement : MonoBehaviour
{
    [SerializeField] public TMP_Text achievementId;
    [SerializeField] public GameObject successPannel;
    public void GetCollectAchievement()
    {
        // Get user information
        User user = PlayerInfomation.LoadPlayerInfo();


        if (user == null)
        {
            Debug.LogError("Failed to load player information.");
            return;
        }

        // add 5 diamond for user
        PlayerInfomation.UpdatePlayerInfo(u => u.diamond += 5);
        // Update the user information in PlayerInfomation
        APIUser.UpdateUser();
        // Get the achievements for the user
        if (APIPlayerAchievement.UpdatePlayerAchievement(int.Parse(achievementId.text)))
        {
            Debug.Log("Achievement updated successfully.");
            successPannel.SetActive(true);
        }
        else
        {
            Debug.LogError("Failed to update achievement.");
        }

    }

}
