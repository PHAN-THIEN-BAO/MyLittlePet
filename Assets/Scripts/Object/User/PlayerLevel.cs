using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] public Image expBarFill;
    [SerializeField] public TMP_Text levelText;// Lv: __
    [SerializeField] public TMP_Text expText;   // currentExp/totalExp
    [SerializeField] public GameObject nextLevelPanel; // Panel hiển thị khi lên cấp
    [SerializeField] public TMP_Text coinReward;
    [SerializeField] public TMP_Text diamonReward;
    [SerializeField] public TMP_Text gemReward;

    private User currentUser;

    private void Start()
    {
        // Load thông tin người chơi
        LoadPlayerInfo();
        UpdateUI();
    }

    // Hàm load thông tin người chơi từ PlayerInfomation
    public void LoadPlayerInfo()
    {
        currentUser = PlayerInfomation.LoadPlayerInfo();

        // Nếu currentUser là null, hiện log lỗi
        if (currentUser == null)
            Debug.LogError("Không thể load thông tin người chơi!");
        else
        {
            // Đảm bảo level và exp không null
            if (currentUser.level == 0)
                currentUser.level = 1; // Level mặc định là 1 thay vì 0

            if (!currentUser.exp.HasValue)
                currentUser.exp = 0;
        }
    }

    // Hàm cập nhật UI hiển thị level và exp
    public void UpdateUI()
    {
        if (currentUser == null)
        {
            LoadPlayerInfo();
            if (currentUser == null) return;
        }

        // Đảm bảo level và exp không null
        int level = currentUser.level > 0 ? currentUser.level : 1;
        int exp = currentUser.exp.GetValueOrDefault(0);

        // Cập nhật text hiển thị level
        if (levelText != null)
            levelText.text = "Lv: " + level;

        // Tính toán tổng exp cần để lên level tiếp theo
        int totalExpNeeded = CalculateTotalExp(level);

        // Cập nhật text hiển thị exp
        if (expText != null)
            expText.text = exp + "/" + totalExpNeeded + " exp";

        // Cập nhật thanh exp
        if (expBarFill != null)
        {
            float fillAmount = (float)exp / totalExpNeeded;
            expBarFill.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }

    // Hàm tính tổng exp cần cho mỗi level
    public int CalculateTotalExp(int level)
    {
        return 50 * level * level;
    }

    // Hàm cộng exp và kiểm tra lên level
    public void AddExp(int expAmount)
    {
        if (currentUser == null)
        {
            LoadPlayerInfo();
            if (currentUser == null) return;
        }

        // Đảm bảo exp không null
        if (!currentUser.exp.HasValue)
            currentUser.exp = 0;

        // Tính toán tổng exp cần để lên level hiện tại
        int totalExpNeeded = CalculateTotalExp(currentUser.level);

        // Cộng exp vào exp hiện tại
        currentUser.exp += expAmount;

        Debug.Log($"Đã thêm {expAmount} exp. Hiện tại có {currentUser.exp} exp / {totalExpNeeded} exp.");

        // Kiểm tra xem đã đủ exp để lên level chưa
        CheckLevelUp();

        // Lưu thông tin người chơi
        SavePlayerInfo();

        // Cập nhật UI
        UpdateUI();
    }

    // Hàm kiểm tra và xử lý lên level
    private void CheckLevelUp()
    {
        if (currentUser == null) return;

        // Đảm bảo exp không null
        if (!currentUser.exp.HasValue)
            currentUser.exp = 0;

        int totalExpNeeded = CalculateTotalExp(currentUser.level);

        // Nếu exp hiện tại >= exp cần thiết để lên level
        if (currentUser.exp.Value >= totalExpNeeded)
        {
            // Lưu lại level cũ để tính phần thưởng
            int oldLevel = currentUser.level;

            // Trừ exp đã dùng để lên level
            currentUser.exp -= totalExpNeeded;

            // Tăng level
            currentUser.level++;

            Debug.Log($"Lên cấp! Từ level {oldLevel} lên level {currentUser.level}");

            // Hiển thị panel lên cấp và phần thưởng
            ShowLevelUpRewards(oldLevel);
            PlayerInfomation.SavePlayerInfo(currentUser);

            // Kiểm tra tiếp nếu đủ exp để lên thêm level nữa
            CheckLevelUp();
        }
        PlayerInfomation.SavePlayerInfo(currentUser);
        Debug.Log("currentUser save exp: " + currentUser.exp);
        APIUser.UpdateUser();
    }

    // Hàm hiển thị phần thưởng lên cấp
    private void ShowLevelUpRewards(int oldLevel)
    {
        if (nextLevelPanel == null) return;

        // Tính phần thưởng
        int coinRewardAmount = oldLevel * 100;
        int diamondRewardAmount = oldLevel * 2;
        int gemRewardAmount = (oldLevel <= 5) ? oldLevel : oldLevel + 5;

        // Cập nhật text hiển thị phần thưởng
        if (coinReward != null)
            coinReward.text = coinRewardAmount.ToString();

        if (diamonReward != null)
            diamonReward.text = diamondRewardAmount.ToString();

        if (gemReward != null)
            gemReward.text = gemRewardAmount.ToString();

        // Cộng phần thưởng vào tài khoản người chơi
        currentUser.coin += coinRewardAmount;
        currentUser.diamond += diamondRewardAmount;
        currentUser.gem += gemRewardAmount;

        // Hiển thị panel lên cấp
        nextLevelPanel.SetActive(true);
        nextLevelPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(nextLevelPanel, new Vector3(1, 1, 1), 1f)
            .setDelay(0)
            .setEase(LeanTweenType.easeOutElastic);
        // Lưu thông tin người chơi sau khi cập nhật phần thưởng
        SavePlayerInfo();
    }

    // Hàm lưu thông tin người chơi
    private void SavePlayerInfo()
    {
        PlayerInfomation.SavePlayerInfo(currentUser);
        APIUser.UpdateUser();
    }

    // Hàm đóng panel lên cấp (có thể gọi từ nút trong panel)
    public void CloseNextLevelPanel()
    {
        if (nextLevelPanel != null)
            nextLevelPanel.SetActive(false);
    }
}





// how to use AddExp method
/*PlayerLevel playerLevel = GameObject.Find("Player").GetComponent<PlayerLevel>();
if (playerLevel != null)
{
    playerLevel.AddExp(100); // Thêm 100 exp
}*/
