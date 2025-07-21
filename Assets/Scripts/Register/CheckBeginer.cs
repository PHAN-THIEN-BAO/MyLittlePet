using UnityEngine;
public class CheckBeginer : MonoBehaviour
{
    [SerializeField] public GameObject choosePetPannel;
    void Start()
    {
        Check();
    }
    public void Check()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (APIUser.GetPlayerPetCount(user.id.ToString()) == 0)
        {
            choosePetPannel.SetActive(true);
            AddDefaultAchievement();
        }
        else
        {
            choosePetPannel.SetActive(false);
        }
    }
    private void AddDefaultAchievement()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        APIPlayerAchievement.AddAchievement(1);
    }
}