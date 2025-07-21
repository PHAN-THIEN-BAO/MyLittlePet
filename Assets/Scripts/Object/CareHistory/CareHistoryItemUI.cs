using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class CareHistoryItemUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text dateTimeText;
    [SerializeField] private TMP_Text activityTypeText;
    [SerializeField] private TMP_Text petNameText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private Image activityIcon;
    [SerializeField] private Button detailsButton;
    [Header("Activity Icons")]
    [SerializeField] private Sprite feedingIcon;
    [SerializeField] private Sprite sleepIcon;
    [SerializeField] private Sprite playIcon;
    [SerializeField] private Sprite defaultIcon;
    private CareHistory historyData;
    private void Start()
    {
        if (detailsButton != null)
        {
            detailsButton.onClick.AddListener(ShowDetails);
        }
    }
    public void SetupHistoryItem(CareHistory history, bool showDetails = true)
    {
        historyData = history;
        if (dateTimeText != null)
        {
            dateTimeText.text = FormatDateTime(history.performedAt);
        }
        if (activityTypeText != null)
        {
            activityTypeText.text = GetActivityTypeText(history.activityId);
        }
        if (petNameText != null)
        {
            SetPetInfo(history.playerPetId);
        }
        if (playerNameText != null)
        {
            SetPlayerInfo(history.playerId);
        }
        if (activityIcon != null)
        {
            activityIcon.sprite = GetActivityIcon(history.activityId);
        }
        if (detailsButton != null)
        {
            detailsButton.gameObject.SetActive(showDetails);
        }
    }
    private string FormatDateTime(DateTime dateTime)
    {
        TimeSpan timeDiff = DateTime.Now - dateTime;
        if (timeDiff.TotalMinutes < 1)
            return "Just now";
        else if (timeDiff.TotalMinutes < 60)
            return $"{(int)timeDiff.TotalMinutes}m ago";
        else if (timeDiff.TotalHours < 24)
            return $"{(int)timeDiff.TotalHours}h ago";
        else if (timeDiff.TotalDays < 7)
            return $"{(int)timeDiff.TotalDays}d ago";
        else
            return dateTime.ToString("MMM dd, yyyy");
    }
    private string GetActivityTypeText(int activityId)
    {
        switch (activityId)
        {
            case 1: return "Feeding";
            case 2: return "Sleeping";
            case 3: return "Playing";
            default: return $"Activity {activityId}";
        }
    }
    private Sprite GetActivityIcon(int activityId)
    {
        switch (activityId)
        {
            case 1: return feedingIcon ?? defaultIcon;
            case 2: return sleepIcon ?? defaultIcon;
            case 3: return playIcon ?? defaultIcon;
            default: return defaultIcon;
        }
    }
    private void SetPetInfo(int playerPetId)
    {
        try
        {
            PlayerPet playerPet = APIPlayerPet.GetPlayerPetById(playerPetId);
            if (playerPet != null && !string.IsNullOrEmpty(playerPet.petCustomName))
            {
                petNameText.text = playerPet.petCustomName;
            }
            else
            {
                petNameText.text = $"Pet {playerPetId}";
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"CareHistoryItemUI: Could not load pet info for ID {playerPetId}: {ex.Message}");
            petNameText.text = $"Pet {playerPetId}";
        }
    }
    private void SetPlayerInfo(int playerId)
    {
        try
        {
            User user = APIUser.GetUserById(playerId);
            if (user != null && !string.IsNullOrEmpty(user.userName))
            {
                playerNameText.text = user.userName;
            }
            else
            {
                playerNameText.text = $"Player {playerId}";
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"CareHistoryItemUI: Could not load player info for ID {playerId}: {ex.Message}");
            playerNameText.text = $"Player {playerId}";
        }
    }
    private void ShowDetails()
    {
        if (historyData == null) return;
        string details = $"Care History Details:\n" +
                        $"Date: {historyData.performedAt:MMM dd, yyyy HH:mm:ss}\n" +
                        $"Activity: {GetActivityTypeText(historyData.activityId)}\n" +
                        $"Pet ID: {historyData.playerPetId}\n" +
                        $"Player ID: {historyData.playerId}\n" +
                        $"History ID: {historyData.careHistoryId}";
        Debug.Log(details);
    }
    public CareHistory GetHistoryData()
    {
        return historyData;
    }
}