using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class PetInfoUIManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject petsInfoPanel;
    
    [Header("Pet Details UI")]
    public TMP_Text petNameText;
    public TMP_Text petLevelText;
    public TMP_Text petCustomNameText;
    public TMP_Text petAdoptedDateText;
    
    [Header("Status Components")]
    public PetStatusBarManager statusBarManager;
    
    [Header("Status Indicators")]
    public GameObject statusCriticalWarning;
    public float criticalThreshold = 15f;
    
    // Track the current pet being displayed
    private int currentPetID = -1;
    
    // Property to access current pet ID
    public int CurrentPetID => currentPetID;
    
    // Define the player ID value
    private const int PLAYER_ID = 3;
    
    // Default PlayerPetID to use
    public int defaultPlayerPetID = 1;
    
    private void Start()
    {
        // Find required components
        if (statusBarManager == null)
        {
            statusBarManager = GetComponentInChildren<PetStatusBarManager>();
            
            if (statusBarManager == null && petsInfoPanel != null)
            {
                statusBarManager = petsInfoPanel.GetComponentInChildren<PetStatusBarManager>();
            }
            
            if (statusBarManager == null)
            {
                Debug.LogWarning("No PetStatusBarManager found. Status bars will not be updated.");
            }
        }
        
        // Hide critical warning by default
        if (statusCriticalWarning != null)
        {
            statusCriticalWarning.SetActive(false);
        }
        
        // Try to subscribe to pet status updates
        PetStatusManager statusManager = Object.FindAnyObjectByType<PetStatusManager>();
        if (statusManager != null)
        {
            statusManager.OnPetStatusUpdated += OnPetStatusUpdated;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        PetStatusManager statusManager = Object.FindAnyObjectByType<PetStatusManager>();
        if (statusManager != null)
        {
            statusManager.OnPetStatusUpdated -= OnPetStatusUpdated;
        }
    }
    
    private void OnPetStatusUpdated(PlayerPet pet)
    {
        // Only update if this is the pet we're currently displaying
        if (pet.playerPetID == currentPetID)
        {
            UpdatePetInfo(pet);
            CheckCriticalStatus(pet.status);
        }
    }
    
    public void ToggleInfoPanel(int petID)
    {
        bool isActive = petsInfoPanel.activeSelf;
        
        // If we're already showing this pet's info, just toggle visibility
        if (isActive && currentPetID == petID)
        {
            petsInfoPanel.SetActive(false);
            return;
        }
        
        // Store the current pet ID
        currentPetID = petID;
        
        // Show the panel and load the pet details
        petsInfoPanel.SetActive(true);
        DisplayPetDetails(petID);
    }
    
    public void CloseInfoPanel()
    {
        petsInfoPanel.SetActive(false);
    }
    
    public bool IsPanelActive()
    {
        return petsInfoPanel != null && petsInfoPanel.activeSelf;
    }
    
    private void DisplayPetDetails(int petID)
    {
        try
        {
            // Use the hardcoded PlayerID
            User currentUser = new User { id = PLAYER_ID };
            
            PlayerPet petDetails = APIPlayerPet.GetPlayerPetById(petID);
            
            if (petDetails != null && petDetails.playerID == PLAYER_ID)
            {
                UpdatePetInfo(petDetails);
                CheckCriticalStatus(petDetails.status);
            }
            else
            {
                Debug.LogWarning($"Pet does not belong to the player or pet details not found. Pet ID: {petID}, Player ID: {PLAYER_ID}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error displaying pet details: " + ex.Message);
        }
    }
    
    private void UpdatePetInfo(PlayerPet petDetails)
    {
        if (petDetails == null) return;
        
        // Update basic pet info text
        if (petNameText != null)
            petNameText.text = petDetails.petCustomName;
        
        if (petLevelText != null)
            petLevelText.text = "Level: " + petDetails.level.ToString();
        
        if (petAdoptedDateText != null)
            petAdoptedDateText.text = "Adopted: " + petDetails.adoptedAt.ToString("MM/dd/yyyy");
        
        if (petCustomNameText != null)
            petCustomNameText.text = "Custom Name: " + petDetails.petCustomName;
        
        // Update status bars
        if (statusBarManager != null)
        {
            statusBarManager.UpdateLevelSlider(petDetails.level);
            statusBarManager.UpdatePetStatus(petDetails.status);
        }
    }
    
    private void CheckCriticalStatus(string statusString)
    {
        if (string.IsNullOrEmpty(statusString) || statusCriticalWarning == null)
            return;
            
        try
        {
            string[] statuses = statusString.Split('%');
            
            if (statuses.Length >= 3)
            {
                float hungerValue = float.Parse(statuses[0]);
                float happinessValue = float.Parse(statuses[1]);
                float energyValue = float.Parse(statuses[2]);
                
                // Show warning if any stat is below critical threshold
                bool isCritical = hungerValue < criticalThreshold || 
                                  happinessValue < criticalThreshold || 
                                  energyValue < criticalThreshold;
                                  
                statusCriticalWarning.SetActive(isCritical);
                
                // Pulse animation if critical
                if (isCritical)
                {
                    Animator animator = statusCriticalWarning.GetComponent<Animator>();
                    if (animator != null)
                        animator.SetTrigger("Pulse");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error checking critical status: " + ex.Message);
        }
    }
    
    public void RefreshPetInfo()
    {
        if (currentPetID > 0 && petsInfoPanel.activeSelf)
        {
            DisplayPetDetails(currentPetID);
        }
    }
    
    public void TestPlayerWithID(int playerID)
    {
        Debug.Log($"Testing player with ID: {playerID}");
        
        try
        {
            // Always use the hardcoded PLAYER_ID instead of the parameter
            List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(PLAYER_ID);
            
            if (playerPets != null && playerPets.Count > 0)
            {
                Debug.Log($"Found {playerPets.Count} pets for Player ID: {PLAYER_ID}");
                
                // Display the first pet's info
                int firstPetID = playerPets[0].playerPetID;
                ToggleInfoPanel(firstPetID);
                
                Debug.Log($"Displaying pet ID: {firstPetID}, Name: {playerPets[0].petCustomName}");
                Debug.Log($"Status: {playerPets[0].status}");
            }
            else
            {
                Debug.LogWarning($"No pets found for Player ID: {PLAYER_ID}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error testing Player ID {PLAYER_ID}: {ex.Message}");
        }
    }
}