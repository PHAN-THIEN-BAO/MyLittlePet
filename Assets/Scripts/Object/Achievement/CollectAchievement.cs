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
        User user = PlayerInfomation.LoadPlayerInfo();


        if (user == null)
        {
            Debug.LogError("Failed to load player information.");
            return;
        }

        PlayerInfomation.UpdatePlayerInfo(u => u.diamond += 5);
        APIUser.UpdateUser();
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