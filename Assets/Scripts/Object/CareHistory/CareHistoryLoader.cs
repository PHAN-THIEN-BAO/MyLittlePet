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

    [Header("Sort Options")]
    [SerializeField] private TMP_Dropdown sortDropdown;

    [Header("Sort/Filter Dropdowns")]
    [SerializeField] private TMP_Dropdown petNameDropdown;
    [SerializeField] private TMP_Dropdown activityTypeDropdown;
    [SerializeField] private TMP_Dropdown mostRecentDropdown;

    private List<CareHistory> currentCareHistory = new List<CareHistory>();
    private List<CareHistory> allCareHistory = new List<CareHistory>();
    private User currentUser;
    private int selectedPetIndex = 0;
    private int selectedActivityIndex = 0;

    private void Start()
    {
        SetupPetNameDropdown();
        SetupActivityTypeDropdown();
        SetupMostRecentDropdown();
        LoadCareHistoryData();
    }

    private void SetupSortDropdown()
    {
        if (sortDropdown != null)
        {
            sortDropdown.ClearOptions();
            sortDropdown.AddOptions(new List<string> {
                "Most Recent",
                "Pet Name (A-Z)",
                "Activity Type (A-Z)"
            });
            sortDropdown.onValueChanged.AddListener(OnSortDropdownChanged);
        }
    }

    private void SetupPetNameDropdown()
    {
        if (petNameDropdown != null)
        {
            var playerPets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
            var petNames = playerPets.Select(p => p.petCustomName).OrderBy(n => n).ToList();
            petNames.Insert(0, "All Pets");
            petNameDropdown.ClearOptions();
            petNameDropdown.AddOptions(petNames);
            petNameDropdown.onValueChanged.AddListener(OnPetNameDropdownChanged);
        }
    }

    private void SetupActivityTypeDropdown()
    {
        if (activityTypeDropdown != null)
        {
            var activities = APICareActivity.GetAllCareActivities();
            var activityTypes = activities.Select(a => a.activityType).OrderBy(t => t).ToList();
            activityTypes.Insert(0, "All Activities");
            activityTypeDropdown.ClearOptions();
            activityTypeDropdown.AddOptions(activityTypes);
            activityTypeDropdown.onValueChanged.AddListener(OnActivityTypeDropdownChanged);
        }
    }

    private void SetupMostRecentDropdown()
    {
        if (mostRecentDropdown != null)
        {
            mostRecentDropdown.ClearOptions();
            mostRecentDropdown.AddOptions(new List<string> { "Most Recent", "Oldest First" });
            mostRecentDropdown.onValueChanged.AddListener(OnMostRecentDropdownChanged);
        }
    }

    private void OnSortDropdownChanged(int option)
    {
        switch (option)
        {
            case 0:
                sortByMostRecent = true;
                ProcessAndDisplayHistory(currentCareHistory);
                break;
            case 1:
                sortByMostRecent = false;
                var playerPets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
                currentCareHistory = currentCareHistory
                    .OrderBy(h => {
                        var pet = playerPets.FirstOrDefault(p => p.playerPetID == h.playerPetId);
                        return pet != null ? pet.petCustomName : "";
                    })
                    .ToList();
                DisplayCareHistory();
                break;
                //case 2: // Activity Type (A-Z)
        }
    }

    private void OnPetNameDropdownChanged(int index)
    {
        selectedPetIndex = index;
        ApplyCombinedFilter();
    }

    private void OnActivityTypeDropdownChanged(int index)
    {
        selectedActivityIndex = index;
        ApplyCombinedFilter();
    }

    private void OnMostRecentDropdownChanged(int index)
    {
        sortByMostRecent = (index == 0);
        ProcessAndDisplayHistory(currentCareHistory);
    }

    private void ApplyCombinedFilter()
    {
        var playerPets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
        var activities = APICareActivity.GetAllCareActivities();

        IEnumerable<CareHistory> filtered = allCareHistory;

        if (selectedPetIndex > 0)
        {
            var selectedPet = playerPets.OrderBy(p => p.petCustomName).ElementAt(selectedPetIndex - 1);
            filtered = filtered.Where(h => h.playerPetId == selectedPet.playerPetID);
        }

        if (selectedActivityIndex > 0)
        {
            var selectedType = activities.OrderBy(a => a.activityType).ElementAt(selectedActivityIndex - 1).activityType;
            filtered = filtered.Where(h =>
            {
                var activity = activities.FirstOrDefault(a => a.activityId == h.activityId);
                return activity != null && activity.activityType == selectedType;
            });
        }

        currentCareHistory = filtered.ToList();
        DisplayCareHistory();
    }

    public void LoadCareHistoryData()
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(true);

        if (noHistoryText != null)
            noHistoryText.gameObject.SetActive(false);

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
            if (loadingIndicator != null)
                loadingIndicator.SetActive(false);
        }
    }

    private List<CareHistory> LoadAllCareHistory()
    {
        Debug.Log("CareHistoryLoader: Loading all care history...");
        return APICareHistory.GetAllCareHistory();
    }

    private List<CareHistory> LoadPlayerCareHistory(int playerId)
    {
        Debug.Log($"CareHistoryLoader: Loading care history for player {playerId}...");
        return APICareHistory.GetCareHistoryByPlayerId(playerId);
    }

    private List<CareHistory> LoadPetCareHistory(int playerPetId)
    {
        Debug.Log($"CareHistoryLoader: Loading care history for pet {playerPetId}...");
        return APICareHistory.GetCareHistoryByPlayerPetId(playerPetId);
    }

    private PlayerPet GetCurrentActivePet()
    {
        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(currentUser.id);
        if (playerPets != null && playerPets.Count > 0)
        {
            return playerPets[0];
        }
        return null;
    }

    private void ProcessAndDisplayHistory(List<CareHistory> historyData)
    {
        allCareHistory = historyData;
        currentCareHistory = historyData;

        if (sortByMostRecent)
        {
            currentCareHistory = currentCareHistory
                .OrderByDescending(h => h.performedAt)
                .ToList();
        }

        if (maxHistoryItems > 0 && currentCareHistory.Count > maxHistoryItems)
        {
            currentCareHistory = currentCareHistory.Take(maxHistoryItems).ToList();
        }

        DisplayCareHistory();
    }

    private void DisplayCareHistory()
    {
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

    private void CreateHistoryItem(CareHistory history)
    {
        if (careHistoryItemPrefab == null || contentParent == null)
        {
            Debug.LogWarning("CareHistoryLoader: Missing prefab or content parent for displaying history items");
            return;
        }

        GameObject historyItem = Instantiate(careHistoryItemPrefab, contentParent);
        
        CareHistoryItemUI itemUI = historyItem.GetComponent<CareHistoryItemUI>();
        if (itemUI != null)
        {
            itemUI.SetupHistoryItem(history, showActivityDetails);
        }
        else
        {
            SetupHistoryItemManually(historyItem, history);
        }
    }

    private void SetupHistoryItemManually(GameObject historyItem, CareHistory history)
    {
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

    public void RefreshCareHistory()
    {
        LoadCareHistoryData();
    }

    public void SetLoadAllHistory()
    {
        loadAllHistory = true;
        loadCurrentPlayerHistory = false;
        loadCurrentPetHistory = false;
        LoadCareHistoryData();
    }

    public void SetLoadPlayerHistory()
    {
        loadAllHistory = false;
        loadCurrentPlayerHistory = true;
        loadCurrentPetHistory = false;
        LoadCareHistoryData();
    }

    public void SetLoadPetHistory(int playerPetId = -1)
    {
        loadAllHistory = false;
        loadCurrentPlayerHistory = false;
        loadCurrentPetHistory = true;
        
        if (playerPetId > 0)
            specificPlayerPetId = playerPetId;
            
        LoadCareHistoryData();
    }

    public void SetMaxHistoryItems(int maxItems)
    {
        maxHistoryItems = maxItems;
    }

    public void SetSortByMostRecent(bool sortRecent)
    {
        sortByMostRecent = sortRecent;
        if (currentCareHistory != null && currentCareHistory.Count > 0)
        {
            ProcessAndDisplayHistory(currentCareHistory);
        }
    }

    public List<CareHistory> GetCurrentCareHistory()
    {
        return currentCareHistory;
    }

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