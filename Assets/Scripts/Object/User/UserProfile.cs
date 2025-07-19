//using TMPro;
//using UnityEngine;
//using System.Collections.Generic;


//public class UserProfile : MonoBehaviour
//{
//    [SerializeField] public TMP_Text namePlayer;
//    [SerializeField] public TMP_Text levelPlayer;
//    [SerializeField] public TMP_Text petOwned;
//    [SerializeField] public TMP_Text Achievements;

//    public void SetUserProfile()
//    {
//        // Load user data from PlayerInfomation and set the UI elements
//        User user = PlayerInfomation.LoadPlayerInfo();
//        List<Achievement> listAchievement = APIAchievement.GetAllAchievements();
//        List<PlayerAchievement> playerAchievements = APIPlayerAchievement.GetAchievementByIdPlayer(user.id);
//        if (user != null)
//        {
//            namePlayer.text = user.userName;
//            levelPlayer.text = "Lv: " + user.level.ToString();
//            petOwned.text = "Pets Owned:             " + APIUser.GetPlayerPetCount(user.id.ToString()).ToString();
//        }
//        else
//        {
//            Debug.LogError("User data is null. Please ensure PlayerInfomation is set up correctly.");
//        }
//        // Set the number of achievements in the UI
//        int countAchievement = listAchievement != null ? listAchievement.Count : 0;
//        int countPlayerAchievement = playerAchievements != null ? playerAchievements.Count : 0;
//        Achievements.text = countPlayerAchievement.ToString() + "/" + countAchievement.ToString();

//    }



//}


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
        // Khởi tạo hệ thống đổi tên
        if (renameButton != null)
            renameButton.onClick.AddListener(OpenRenamePanel);

        if (submitRenameButton != null)
            submitRenameButton.onClick.AddListener(SubmitNewName);

        if (cancelRenameButton != null)
            cancelRenameButton.onClick.AddListener(CloseRenamePanel);

        // Ẩn panel đổi tên ban đầu
        if (renamePanel != null)
            renamePanel.SetActive(false);

        // Load thông tin người dùng
        SetUserProfile();
    }

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
            petOwned.text = "Pets Owned:             " + APIUser.GetPlayerPetCount(user.id.ToString()).ToString();
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

    // Mở panel đổi tên
    public void OpenRenamePanel()
    {
        if (renamePanel != null)
        {
            renamePanel.SetActive(true);

            // Lấy tên hiện tại của người dùng để hiển thị trong input field
            User user = PlayerInfomation.LoadPlayerInfo();
            if (user != null && renameInput != null)
            {
                renameInput.text = user.userName;
                renameInput.Select();
                renameInput.ActivateInputField();
            }
        }
    }

    // Đóng panel đổi tên
    public void CloseRenamePanel()
    {
        if (renamePanel != null)
            renamePanel.SetActive(false);
    }

    // Lưu tên mới
    public void SubmitNewName()
    {
        if (renameInput == null || string.IsNullOrWhiteSpace(renameInput.text))
        {
            Debug.LogWarning("Tên người dùng không được để trống!");
            return;
        }

        string newName = renameInput.text.Trim();

        // Cập nhật tên người dùng
        PlayerInfomation.UpdatePlayerInfo(user => {
            user.userName = newName;
        });

        // Cập nhật UI
        namePlayer.text = newName;

        // Đóng panel đổi tên
        CloseRenamePanel();

        Debug.Log("Đã đổi tên thành: " + newName);
    }
}