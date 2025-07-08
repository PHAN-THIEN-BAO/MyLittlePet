using System.Collections.Generic;
using UnityEngine;

public class CollectAchievement : MonoBehaviour
{
    [SerializeField] public List<GameObject> achievementCollectButton;
    [SerializeField] public List<GameObject> achievementId;

    public void GetCollectAchievement()
    {
        // Get user information
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("Failed to load player information.");
            return;
        }
    }



}
