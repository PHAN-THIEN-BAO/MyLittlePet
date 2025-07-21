using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class UserProfile : MonoBehaviour
{
    [SerializeField] public TMP_Text namePlayer;
    [SerializeField] public TMP_Text levelPlayer;
    [SerializeField] public TMP_Text petOwned;
    [SerializeField] public TMP_Text Achievements;
    [Header("Rename System")]
    [SerializeField] private GameObject renamePanel;
    [SerializeField] private TMP_InputField renameInput;
    [SerializeField] private Button renameButton;
    [SerializeField] private Button submitRenameButton;
    [SerializeField] private Button cancelRenameButton;
    private void Start()
    {
        if (renameButton != null)
            renameButton.onClick.AddListener(OpenRenamePanel);
        if (submitRenameButton != null)
            submitRenameButton.onClick.AddListener(SubmitNewName);
        if (cancelRenameButton != null)
            cancelRenameButton.onClick.AddListener(CloseRenamePanel);
        if (renamePanel != null)
            renamePanel.SetActive(false);
        SetUserProfile();
    }
    public void SetUserProfile()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        List<Achievement> listAchievement = APIAchievement.GetAllAchievements();
        List<PlayerAchievement> playerAchievements = APIPlayerAchievement.GetAchievementByIdPlayer(user.id);
        if (user != null)
        {
            namePlayer.text = user.userName;
            levelPlayer.text = "Lv: " + user.level.ToString();
            petOwned.text = "Pets Owned:             " + APIUser.GetPlayerPetCount(user.id.ToString()).ToString();
        }
        else
        {
            Debug.LogError("User data is null. Please ensure PlayerInfomation is set up correctly.");
        }
        int countAchievement = listAchievement != null ? listAchievement.Count : 0;
        int countPlayerAchievement = playerAchievements != null ? playerAchievements.Count : 0;
        Achievements.text = countPlayerAchievement.ToString() + "/" + countAchievement.ToString();
    }
    public void OpenRenamePanel()
    {
        if (renamePanel != null)
        {
            renamePanel.SetActive(true);
            User user = PlayerInfomation.LoadPlayerInfo();
            if (user != null && renameInput != null)
            {
                renameInput.text = user.userName;
                renameInput.Select();
                renameInput.ActivateInputField();
            }
        }
    }
    public void CloseRenamePanel()
    {
        if (renamePanel != null)
            renamePanel.SetActive(false);
    }
    public void SubmitNewName()
    {
        if (renameInput == null || string.IsNullOrWhiteSpace(renameInput.text))
        {
            Debug.LogWarning("Tên ngu?i dùng không du?c d? tr?ng!");
            return;
        }
        string newName = renameInput.text.Trim();
        PlayerInfomation.UpdatePlayerInfo(user => {
            user.userName = newName;
        });
        namePlayer.text = newName;
        bool updateSuccess = APIUser.UpdateUser();
        if (updateSuccess)
        {
            Debug.Log("Ðã c?p nh?t tên ngu?i dùng lên database thành công!");
        }
        else
        {
            Debug.LogWarning("C?p nh?t tên ngu?i dùng lên database không thành công. Thay d?i ch? du?c luu c?c b?.");
        }
        CloseRenamePanel();
        Debug.Log("Ðã d?i tên thành: " + newName);
    }
}