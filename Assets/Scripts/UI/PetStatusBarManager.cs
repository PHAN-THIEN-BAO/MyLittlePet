using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PetStatusBarManager : MonoBehaviour
{    [Header("Status Text UI")]
    public TMP_Text petHungerStatusText;
    public TMP_Text petHappinessStatusText;
    public TMP_Text petEnergyStatusText;
    [Header("Status Progress Bars")]
    public Slider hungerSlider;
    public Slider happinessSlider;
    public Slider energySlider;
    [Header("Level Progress Bar")]
    public Slider levelSlider;
    public int maxPetLevel = 100;
    [Header("Status Value Settings")]
    public float maxStatusValue = 100f;
    public bool normalizeStatusValues = true;
    private void Start()
    {
        InitializeSliders();
    }
    public void InitializeSliders()
    {
        if (hungerSlider != null) hungerSlider.maxValue = maxStatusValue;
        if (happinessSlider != null) happinessSlider.maxValue = maxStatusValue;
        if (energySlider != null) energySlider.maxValue = maxStatusValue;
        if (levelSlider != null) levelSlider.maxValue = maxPetLevel;
    }
    public void UpdateLevelSlider(int level)
    {
        if (levelSlider != null)
        {
            levelSlider.value = Mathf.Clamp(level, 0, maxPetLevel);
        }
    }
    public void UpdatePetStatus(string statusString)
    {
        if (string.IsNullOrEmpty(statusString))
        {
            HandleInvalidStatusFormat("Status string is empty");
            return;
        }
        try
        {
            string[] statuses = statusString.Split('%');
            if (statuses.Length >= 3)
            {
                if (petHungerStatusText != null)
                    petHungerStatusText.text = statuses[0] + "%";
                if (petHappinessStatusText != null)
                    petHappinessStatusText.text = statuses[1] + "%";
                if (petEnergyStatusText != null)
                    petEnergyStatusText.text = statuses[2] + "%";
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
    private void UpdateStatusBars(string[] statuses)
    {
        try
        {
            if (statuses.Length >= 3)
            {
                bool hunger = float.TryParse(statuses[0], out float hungerValue);
                bool happiness = float.TryParse(statuses[1], out float happinessValue);
                bool energy = float.TryParse(statuses[2], out float energyValue);
                Debug.Log($"Parsed status values - Hunger: {hungerValue}, Happiness: {happinessValue}, Energy: {energyValue}");
                if (hunger && hungerSlider != null)
                {
                    hungerSlider.value = Mathf.Clamp(hungerValue, 0, maxStatusValue);
                }
                if (happiness && happinessSlider != null)
                {
                    happinessSlider.value = Mathf.Clamp(happinessValue, 0, maxStatusValue);
                }
                if (energy && energySlider != null)
                {
                    energySlider.value = Mathf.Clamp(energyValue, 0, maxStatusValue);
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
    private void HandleInvalidStatusFormat(string statusString)
    {
        Debug.LogWarning("Status string does not contain 3 values: " + statusString);
        if (petHungerStatusText != null)
            petHungerStatusText.text = "Status: " + statusString;
        if (petHappinessStatusText != null)
            petHappinessStatusText.text = "";
        if (petEnergyStatusText != null)
            petEnergyStatusText.text = "";
        ResetAllSliders();
    }
    public void ResetAllSliders()
    {
        if (hungerSlider != null)
            hungerSlider.value = 0;
        if (happinessSlider != null)
            happinessSlider.value = 0;
        if (energySlider != null)
            energySlider.value = 0;
        if (levelSlider != null)
            levelSlider.value = 0;
    }
}