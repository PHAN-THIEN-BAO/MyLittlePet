using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] public Image expBarFill;
    [SerializeField] public TMP_Text levelText;
    [SerializeField] public TMP_Text expText;
    [SerializeField] public GameObject nextLevelPanel;
    [SerializeField] public TMP_Text coinReward;
    [SerializeField] public TMP_Text diamonReward;
    [SerializeField] public TMP_Text gemReward;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip rewardSound;


    private User currentUser;

    private void Start()
    {
        LoadPlayerInfo();
        UpdateUI();
    }

    public void LoadPlayerInfo()
    {
        currentUser = PlayerInfomation.LoadPlayerInfo();

        if (currentUser == null)
            Debug.LogError("Không th? load thông tin ngu?i choi!");
        else
        {
            if (currentUser.level == 0)
                currentUser.level = 1;

            if (!currentUser.exp.HasValue)
                currentUser.exp = 0;
        }
    }

    public void UpdateUI()
    {
        if (currentUser == null)
        {
            LoadPlayerInfo();
            if (currentUser == null) return;
        }

        int level = currentUser.level > 0 ? currentUser.level : 1;
        int exp = currentUser.exp.GetValueOrDefault(0);

        if (levelText != null)
            levelText.text = "Lv: " + level;

        int totalExpNeeded = CalculateTotalExp(level);

        if (expText != null)
            expText.text = exp + "/" + totalExpNeeded + " exp";

        if (expBarFill != null)
        {
            float fillAmount = (float)exp / totalExpNeeded;
            expBarFill.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }

    public int CalculateTotalExp(int level)
    {
        return 50 * level * level;
    }

    public void AddExp(int expAmount)
    {
        if (currentUser == null)
        {
            LoadPlayerInfo();
            if (currentUser == null) return;
        }

        if (!currentUser.exp.HasValue)
            currentUser.exp = 0;

        int totalExpNeeded = CalculateTotalExp(currentUser.level);

        currentUser.exp += expAmount;

        Debug.Log($"Ðã thêm {expAmount} exp. Hi?n t?i có {currentUser.exp} exp / {totalExpNeeded} exp.");

        CheckLevelUp();

        SavePlayerInfo();

        UpdateUI();
    }

    private void CheckLevelUp()
    {
        if (currentUser == null) return;

        if (!currentUser.exp.HasValue)
            currentUser.exp = 0;

        int totalExpNeeded = CalculateTotalExp(currentUser.level);

        if (currentUser.exp.Value >= totalExpNeeded)
        {
            int oldLevel = currentUser.level;

            currentUser.exp -= totalExpNeeded;

            currentUser.level++;

            Debug.Log($"Lên c?p! T? level {oldLevel} lên level {currentUser.level}");

            ShowLevelUpRewards(oldLevel);
            PlayerInfomation.SavePlayerInfo(currentUser);

            CheckLevelUp();
        }
        PlayerInfomation.SavePlayerInfo(currentUser);
        Debug.Log("currentUser save exp: " + currentUser.exp);
        APIUser.UpdateUser();
    }

    private void ShowLevelUpRewards(int oldLevel)
    {
        if (nextLevelPanel == null) return;

        int coinRewardAmount = oldLevel * 100;
        int diamondRewardAmount = oldLevel * 2;
        int gemRewardAmount = (oldLevel <= 5) ? oldLevel : oldLevel + 5;

        if (coinReward != null)
            coinReward.text = coinRewardAmount.ToString();

        if (diamonReward != null)
            diamonReward.text = diamondRewardAmount.ToString();

        if (gemReward != null)
            gemReward.text = gemRewardAmount.ToString();

        currentUser.coin += coinRewardAmount;
        currentUser.diamond += diamondRewardAmount;
        currentUser.gem += gemRewardAmount;

        nextLevelPanel.SetActive(true);
        nextLevelPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(nextLevelPanel, new Vector3(1, 1, 1), 1f)
            .setDelay(0)
            .setEase(LeanTweenType.easeOutElastic);

        if (audioSource != null && rewardSound != null)
            audioSource.PlayOneShot(rewardSound);


        SavePlayerInfo();
    }

    private void SavePlayerInfo()
    {
        PlayerInfomation.SavePlayerInfo(currentUser);
        APIUser.UpdateUser();
    }

    public void CloseNextLevelPanel()
    {
        if (nextLevelPanel != null)
            nextLevelPanel.SetActive(false);
    }
}