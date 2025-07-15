using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class CareHistoryLoader : MonoBehaviour
{
    [Header("Care History Display Settings")]
    [SerializeField] private GameObject careHistoryItemPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TMP_Text noHistoryText;
    [SerializeField] private GameObject loadingIndicator;

    [Header("Filter Options")]
    [SerializeField] private bool loadAllHistory = false;
    [SerializeField] private bool loadCurrentPlayerHistory = true;
    [SerializeField] private bool loadCurrentPetHistory = false;
    [SerializeField] private int specificPlayerPetId = -1;

    [Header("Display Options")]
    [SerializeField] private int maxHistoryItems = 50;
    [SerializeField] private bool sortByMostRecent = true;
    [SerializeField] private bool showActivityDetails = true;

    private List<CareHistory> currentCareHistory = new List<CareHistory>();
    private User currentUser;

    private void Start()
    {
        LoadCareHistoryData();
    }

    /// <summary>
    /// Main method to load care history based on current settings
    /// </summary>
    public void LoadCareHistoryData()
    {
        // Show loading indicator
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);

        // Hide no history text initially
        if (noHistoryText != null)
            noHistoryText.gameObject.SetActive(false);

        // Get current user information
        currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser == null)
        {
            Debug.LogError("CareHistoryLoader: Failed to load player information.");
            ShowNoHistoryMessage("Failed to load player information");
            return;
        }

        try
        {
            List<CareHistory> historyData = null;

            if (loadAllHistory)
            {
                historyData = LoadAllCareHistory();
            }
            else if (loadCurrentPlayerHistory)
            {
                historyData = LoadPlayerCareHistory(currentUser.id);
            }
            else if (loadCurrentPetHistory && specificPlayerPetId > 0)
            {
                historyData = LoadPetCareHistory(specificPlayerPetId);
            }
            else if (loadCurrentPetHistory)
            {
                // Load history for the current active pet
                PlayerPet activePet = GetCurrentActivePet();
                if (activePet != null)
                {
                    historyData = LoadPetCareHistory(activePet.playerPetID);
                }
                else
                {
                    ShowNoHistoryMessage("No active pet found");
                    return;
                }
            }

            if (historyData != null && historyData.Count > 0)
            {
                ProcessAndDisplayHistory(historyData);
            }
            else
            {
                ShowNoHistoryMessage("No care history found");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"CareHistoryLoader: Error loading care history - {ex.Message}");
            ShowNoHistoryMessage("Error loading care history");
        }
        finally
        {
            // Hide loading indicator
            if (loadingIndicator != null)
                loadingIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Load all care history records
    /// </summary>
    private List<CareHistory> LoadAllCareHistory()
    {
        Debug.Log("CareHistoryLoader: Loading all care history...");
        return APICareHistory.GetAllCareHistory();
    }

    /// <summary>
    /// Load care history for a specific player
    /// </summary>
    private List<CareHistory> LoadPlayerCareHistory(int playerId)
    {
        Debug.Log($"CareHistoryLoader: Loading care history for player {playerId}...");
        return APICareHistory.GetCareHistoryByPlayerId(playerId);
    }

    /// <summary>
    /// Load care history for a specific pet
    /// </summary>
    private List<CareHistory> LoadPetCareHistory(int playerPetId)
    {
        Debug.Log($"CareHistoryLoader: Loading care history for pet {playerPetId}...");
        return APICareHistory.GetCareHistoryByPlayerPetId(playerPetId);
    }

    /// <summary>
    /// Get the current active pet for the player
    /// </summary>
    private PlayerPet GetCurrentActivePet()
    {
        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
        if (playerPets != null && playerPets.Count > 0)
        {
            // Return the first pet or implement logic to get the currently selected pet
            return playerPets[0];
        }
        return null;
    }

    /// <summary>
    /// Process the loaded history data and display it
    /// </summary>
    private void ProcessAndDisplayHistory(List<CareHistory> historyData)
    {
        currentCareHistory = historyData;

        // Sort by most recent if enabled
        if (sortByMostRecent)
        {
            currentCareHistory = currentCareHistory.OrderByDescending(h => h.performedAt).ToList();
        }

        // Limit the number of items
        if (maxHistoryItems > 0 && currentCareHistory.Count > maxHistoryItems)
        {
            currentCareHistory = currentCareHistory.Take(maxHistoryItems).ToList();
        }

        DisplayCareHistory();
    }

    /// <summary>
    /// Display the care history in the UI
    /// </summary>
    private void DisplayCareHistory()
    {
        // Clear existing items
        ClearHistoryDisplay();

        if (currentCareHistory == null || currentCareHistory.Count == 0)
        {
            ShowNoHistoryMessage("No care history to display");
            return;
        }

        foreach (CareHistory history in currentCareHistory)
        {
            CreateHistoryItem(history);
        }

        Debug.Log($"CareHistoryLoader: Displayed {currentCareHistory.Count} care history items");
    }

    /// <summary>
    /// Create a single history item in the UI
    /// </summary>
    private void CreateHistoryItem(CareHistory history)
    {
        if (careHistoryItemPrefab == null || contentParent == null)
        {
            Debug.LogWarning("CareHistoryLoader: Missing prefab or content parent for displaying history items");
            return;
        }

        GameObject historyItem = Instantiate(careHistoryItemPrefab, contentParent);
        
        // Set up the history item data
        CareHistoryItemUI itemUI = historyItem.GetComponent<CareHistoryItemUI>();
        if (itemUI != null)
        {
            itemUI.SetupHistoryItem(history, showActivityDetails);
        }
        else
        {
            // Fallback: try to set up manually if no CareHistoryItemUI component
            SetupHistoryItemManually(historyItem, history);
        }
    }

    /// <summary>
    /// Manually set up history item if no CareHistoryItemUI component exists
    /// </summary>
    private void SetupHistoryItemManually(GameObject historyItem, CareHistory history)
    {
        // Try to find common text components and set their values
        TMP_Text dateText = historyItem.transform.Find("DateText")?.GetComponent<TMP_Text>();
        if (dateText != null)
            dateText.text = history.performedAt.ToString("MMM dd, yyyy HH:mm");

        TMP_Text activityText = historyItem.transform.Find("ActivityText")?.GetComponent<TMP_Text>();
        if (activityText != null)
            activityText.text = $"Activity ID: {history.activityId}";

        TMP_Text petText = historyItem.transform.Find("PetText")?.GetComponent<TMP_Text>();
        if (petText != null)
            petText.text = $"Pet ID: {history.playerPetId}";
    }

    /// <summary>
    /// Clear all existing history items from display
    /// </summary>
    private void ClearHistoryDisplay()
    {
        if (contentParent != null)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// Show message when no history is available
    /// </summary>
    private void ShowNoHistoryMessage(string message)
    {
        ClearHistoryDisplay();
        
        if (noHistoryText != null)
        {
            noHistoryText.text = message;
            noHistoryText.gameObject.SetActive(true);
        }
        
        Debug.Log($"CareHistoryLoader: {message}");
    }

    /// <summary>
    /// Refresh the care history display
    /// </summary>
    public void RefreshCareHistory()
    {
        LoadCareHistoryData();
    }

    /// <summary>
    /// Set filter to load all history
    /// </summary>
    public void SetLoadAllHistory()
    {
        loadAllHistory = true;
        loadCurrentPlayerHistory = false;
        loadCurrentPetHistory = false;
        LoadCareHistoryData();
    }

    /// <summary>
    /// Set filter to load current player's history
    /// </summary>
    public void SetLoadPlayerHistory()
    {
        loadAllHistory = false;
        loadCurrentPlayerHistory = true;
        loadCurrentPetHistory = false;
        LoadCareHistoryData();
    }

    /// <summary>
    /// Set filter to load specific pet's history
    /// </summary>
    public void SetLoadPetHistory(int playerPetId = -1)
    {
        loadAllHistory = false;
        loadCurrentPlayerHistory = false;
        loadCurrentPetHistory = true;
        
        if (playerPetId > 0)
            specificPlayerPetId = playerPetId;
            
        LoadCareHistoryData();
    }

    /// <summary>
    /// Set the maximum number of history items to display
    /// </summary>
    public void SetMaxHistoryItems(int maxItems)
    {
        maxHistoryItems = maxItems;
    }

    /// <summary>
    /// Toggle sorting by most recent
    /// </summary>
    public void SetSortByMostRecent(bool sortRecent)
    {
        sortByMostRecent = sortRecent;
        if (currentCareHistory != null && currentCareHistory.Count > 0)
        {
            ProcessAndDisplayHistory(currentCareHistory);
        }
    }

    /// <summary>
    /// Get the currently loaded care history data
    /// </summary>
    public List<CareHistory> GetCurrentCareHistory()
    {
        return currentCareHistory;
    }

    /// <summary>
    /// Get care history count for current player
    /// </summary>
    public int GetPlayerCareHistoryCount()
    {
        if (currentUser == null)
        {
            currentUser = PlayerInfomation.LoadPlayerInfo();
            if (currentUser == null) return 0;
        }

        List<CareHistory> playerHistory = APICareHistory.GetCareHistoryByPlayerId(currentUser.id);
        return playerHistory != null ? playerHistory.Count : 0;
    }
}