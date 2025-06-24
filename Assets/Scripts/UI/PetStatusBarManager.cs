using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles the display and updates of pet status progress bars
/// </summary>
public class PetStatusBarManager : MonoBehaviour
{
    [Header("Status Text UI")]
    public TMP_Text petHungerStatusText;
    public TMP_Text petHappinessStatusText; 
    public TMP_Text petEnergyStatusText; // Changed from petHealthStatusText
    
    [Header("Status Progress Bars")]
    public Slider hungerSlider;
    public Slider happinessSlider;
    public Slider energySlider; // Changed from healthSlider
    
    [Header("Level Progress Bar")]
    public Slider levelSlider;
    public int maxPetLevel = 100; // Maximum possible pet level for slider scale
    
    [Header("Status Value Settings")]
    public float maxStatusValue = 100f; // Maximum value for status sliders
    public bool normalizeStatusValues = true; // Set to true if status values need normalization

    [Header("Status Icons")]
    public GameObject lowHungerWarning;
    public GameObject lowHappinessWarning;
    public GameObject lowEnergyWarning;

    [Header("Critical Thresholds")]
    [Range(0, 100)]
    public float warningThreshold = 25f;

    private void Start()
    {
        // Initialize sliders with the proper max values
        InitializeSliders();
        
        // Try to find PetStatusManager and subscribe to events
        // Changed from FindObjectOfType to FindAnyObjectByType
        PetStatusManager statusManager = Object.FindAnyObjectByType<PetStatusManager>();
        if (statusManager != null)
        {
            statusManager.OnPetStatusUpdated += OnPetStatusUpdated;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        // Changed from FindObjectOfType to FindAnyObjectByType
        PetStatusManager statusManager = Object.FindAnyObjectByType<PetStatusManager>();
        if (statusManager != null)
        {
            statusManager.OnPetStatusUpdated -= OnPetStatusUpdated;
        }
    }
    
    private void OnPetStatusUpdated(PlayerPet pet)
    {
        // Only update UI if this status manager is currently displaying this pet
        int currentPetID = GetCurrentDisplayedPetID();
        if (currentPetID == pet.playerPetID)
        {
            UpdatePetStatus(pet.status);
        }
    }
    
    private int GetCurrentDisplayedPetID()
    {
        // Get the current pet ID from the parent PetInfoUIManager
        PetInfoUIManager infoManager = GetComponentInParent<PetInfoUIManager>();
        return infoManager != null ? infoManager.CurrentPetID : -1;
    }
    
    // Initialize all sliders with their proper max values
    public void InitializeSliders()
    {
        if (hungerSlider != null) hungerSlider.maxValue = maxStatusValue;
        if (happinessSlider != null) happinessSlider.maxValue = maxStatusValue;
        if (energySlider != null) energySlider.maxValue = maxStatusValue; // Changed from healthSlider
        if (levelSlider != null) levelSlider.maxValue = maxPetLevel;
        
        // Initialize warning icons
        UpdateWarningIcons(maxStatusValue, maxStatusValue, maxStatusValue);
    }
    
    // Update the level slider based on the pet's level
    public void UpdateLevelSlider(int level)
    {
        if (levelSlider != null)
        {
            // Set the current value to the pet's level
            levelSlider.value = Mathf.Clamp(level, 0, maxPetLevel);
        }
    }
    
    // Update all status bars and text based on the pet's status string
    public void UpdatePetStatus(string statusString)
    {
        if (string.IsNullOrEmpty(statusString))
        {
            HandleInvalidStatusFormat("Status string is empty");
            return;
        }
        
        try
        {
            // Split the status string by the '%' delimiter
            string[] statuses = statusString.Split('%');
            
            // Check if we have 3 status values
            if (statuses.Length >= 3)
            {
                // Display hunger status
                if (petHungerStatusText != null)
                    petHungerStatusText.text = statuses[0] + "%";
                
                // Display happiness status
                if (petHappinessStatusText != null)
                    petHappinessStatusText.text = statuses[1] + "%";
                
                // Display energy status (changed from health)
                if (petEnergyStatusText != null)
                    petEnergyStatusText.text = statuses[2] + "%";
                
                // Update progress bars if they exist
                UpdateStatusBars(statuses);
            }
            else
            {
                HandleInvalidStatusFormat(statusString);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing pet status: " + ex.Message);
            HandleInvalidStatusFormat(statusString);
        }
    }
    
    // Update the status progress bars if they're available
    private void UpdateStatusBars(string[] statuses)
    {
        try
        {
            // Parse status values and update sliders
            if (statuses.Length >= 3)
            {
                // Parse the values with better error handling
                bool hunger = float.TryParse(statuses[0], out float hungerValue);
                bool happiness = float.TryParse(statuses[1], out float happinessValue);
                bool energy = float.TryParse(statuses[2], out float energyValue); // Changed from health
                
                // Log the parsed values for debugging
                Debug.Log($"Parsed status values - Hunger: {hungerValue}, Happiness: {happinessValue}, Energy: {energyValue}"); // Changed Health to Energy
                
                // Update sliders if parsing succeeded
                if (hunger && hungerSlider != null)
                {
                    hungerSlider.value = Mathf.Clamp(hungerValue, 0, maxStatusValue);
                }
                
                if (happiness && happinessSlider != null)
                {
                    happinessSlider.value = Mathf.Clamp(happinessValue, 0, maxStatusValue);
                }
                
                if (energy && energySlider != null) // Changed from health
                {
                    energySlider.value = Mathf.Clamp(energyValue, 0, maxStatusValue); // Changed from healthSlider
                }
                
                // Update warning icons
                if (hunger && happiness && energy)
                {
                    UpdateWarningIcons(hungerValue, happinessValue, energyValue);
                }
            }
            else
            {
                Debug.LogWarning("Not enough status values to update progress bars");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error updating status bars: " + ex.Message);
        }
    }
    
    private void UpdateWarningIcons(float hunger, float happiness, float energy)
    {
        if (lowHungerWarning != null)
            lowHungerWarning.SetActive(hunger < warningThreshold);
            
        if (lowHappinessWarning != null)
            lowHappinessWarning.SetActive(happiness < warningThreshold);
            
        if (lowEnergyWarning != null)
            lowEnergyWarning.SetActive(energy < warningThreshold);
    }
    
    // Handle cases where the status format is invalid
    private void HandleInvalidStatusFormat(string statusString)
    {
        Debug.LogWarning("Status string does not contain 3 values: " + statusString);
        
        // Display raw status as fallback
        if (petHungerStatusText != null)
            petHungerStatusText.text = "Status: " + statusString;
        
        // Hide or clear other status texts
        if (petHappinessStatusText != null)
            petHappinessStatusText.text = "";
        
        if (petEnergyStatusText != null) // Changed from petHealthStatusText
            petEnergyStatusText.text = "";
        
        // Reset sliders
        ResetAllSliders();
    }
    
    // Reset all sliders to zero
    public void ResetAllSliders()
    {
        if (hungerSlider != null)
            hungerSlider.value = 0;
            
        if (happinessSlider != null)
            happinessSlider.value = 0;
            
        if (energySlider != null) // Changed from healthSlider
            energySlider.value = 0;
            
        if (levelSlider != null)
            levelSlider.value = 0;
            
        // Reset warning icons
        UpdateWarningIcons(100, 100, 100);
    }
}