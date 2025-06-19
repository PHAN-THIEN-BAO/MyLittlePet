using TMPro;
using UnityEngine;


public class UserProfile : MonoBehaviour
{
    [SerializeField] public TMP_Text namePlayer;
    [SerializeField] public TMP_Text levelPlayer;
    [SerializeField] public TMP_Text petOwned;

    public void SetUserProfile()
    {
        // Load user data from PlayerInfomation and set the UI elements
        User user = PlayerInfomation.LoadPlayerInfo();
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
    }



}
