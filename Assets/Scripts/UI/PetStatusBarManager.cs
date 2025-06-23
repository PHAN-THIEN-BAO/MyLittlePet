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
    public TMP_Text petHealthStatusText;
    
    [Header("Status Progress Bars")]
    public Slider hungerSlider;
    public Slider happinessSlider;
    public Slider healthSlider;
    
    [Header("Level Progress Bar")]
    public Slider levelSlider;
    public int maxPetLevel = 100; // Maximum possible pet level for slider scale
    
    [Header("Status Value Settings")]
    public float maxStatusValue = 100f; // Maximum value for status sliders
    public bool normalizeStatusValues = true; // Set to true if status values need normalization

    private void Start()
    {
        // Initialize sliders with the proper max values
        InitializeSliders();
    }
    
    // Initialize all sliders with their proper max values
    public void InitializeSliders()
    {
        if (hungerSlider != null) hungerSlider.maxValue = maxStatusValue;
        if (happinessSlider != null) happinessSlider.maxValue = maxStatusValue;
        if (healthSlider != null) healthSlider.maxValue = maxStatusValue;
        if (levelSlider != null) levelSlider.maxValue = maxPetLevel;
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
                
                // Display health status
                if (petHealthStatusText != null)
                    petHealthStatusText.text = statuses[2] + "%";
                
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
                bool health = float.TryParse(statuses[2], out float healthValue);
                
                // Log the parsed values for debugging
                Debug.Log($"Parsed status values - Hunger: {hungerValue}, Happiness: {happinessValue}, Health: {healthValue}");
                
                // Update sliders if parsing succeeded
                if (hunger && hungerSlider != null)
                {
                    hungerSlider.value = Mathf.Clamp(hungerValue, 0, maxStatusValue);
                }
                
                if (happiness && happinessSlider != null)
                {
                    happinessSlider.value = Mathf.Clamp(happinessValue, 0, maxStatusValue);
                }
                
                if (health && healthSlider != null)
                {
                    healthSlider.value = Mathf.Clamp(healthValue, 0, maxStatusValue);
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
        
        if (petHealthStatusText != null)
            petHealthStatusText.text = "";
        
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
            
        if (healthSlider != null)
            healthSlider.value = 0;
            
        if (levelSlider != null)
            levelSlider.value = 0;
    }
}