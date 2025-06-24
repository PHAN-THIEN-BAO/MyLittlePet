using UnityEngine;

public class CheckBeginer : MonoBehaviour
{
    [SerializeField] public GameObject choosePetPannel;

    void Start()
    {
        // Check if the player is a beginner when the script starts
        Check();
    }

    public void Check()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        // Check if the player is a beginner
        if (APIUser.GetPlayerPetCount(user.id.ToString()) == 0)
        {
            // If the player is a beginner, show the choose pet panel
            choosePetPannel.SetActive(true);
        }
        else
        {
            // If the player is not a beginner, hide the choose pet panel
            choosePetPannel.SetActive(false);
        }
    }
}
