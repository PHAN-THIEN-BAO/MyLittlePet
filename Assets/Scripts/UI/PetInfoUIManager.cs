using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PetInfoUIManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject petsInfoPanel;
    
    [Header("Pet Details UI")]
    public TMP_Text petNameText;
    public TMP_Text petLevelText;
    public TMP_Text petCustomNameText;
    public TMP_Text petAdoptedDateText;
    
    [Header("Status Bar Manager")]
    public PetStatusBarManager statusBarManager;
    
    [Header("Status Decay Settings")]
    [Tooltip("Time in seconds between status decay updates")]
    public float decayInterval = 300f; // 5 minutes by default
    [Tooltip("Amount to reduce hunger by on decay")]
    public int hungerDecayAmount = 5;
    [Tooltip("Amount to reduce happiness by on decay")]
    public int happinessDecayAmount = 5;
    [Tooltip("Amount to reduce energy by on decay")]
    public int energyDecayAmount = 5;
    [Tooltip("Minimum status value before decay stops")]
    public int minStatusValue = 10;
    
    [Header("Pet Care Settings")]
    [Tooltip("Amount to increase hunger when feeding pet")]
    public int feedIncreaseAmount = 15;
    [Tooltip("Amount to increase happiness when playing with pet")]
    public int playIncreaseAmount = 15;
    [Tooltip("Amount to increase energy when pet sleeps")]
    public int sleepIncreaseAmount = 15;
    [Tooltip("Maximum value for any status")]
    public int maxStatusValue = 100;
    
    // Track the current pet being displayed
    private int currentPetID = -1;
    
    // Store the current pet details
    private PlayerPet currentPetDetails;
    
    // Track if decay coroutine is running
    private Coroutine decayCoroutine;

    private void Start()
    {
        // Make sure we have a status bar manager
        if (statusBarManager == null)
        {
            // Try to find one on this GameObject
            statusBarManager = GetComponent<PetStatusBarManager>();
            
            // If still not found, look for one in children
            if (statusBarManager == null && petsInfoPanel != null)
            {
                statusBarManager = petsInfoPanel.GetComponentInChildren<PetStatusBarManager>();
            }
            
            if (statusBarManager == null)
            {
                Debug.LogWarning("No PetStatusBarManager found. Status bars will not be updated.");
            }
        }
        
        // Start the decay system if we have an active pet
        if (currentPetID != -1)
        {
            StartDecaySystem();
        }
    }
    
    private void OnDestroy()
    {
        // Stop the decay coroutine when this component is destroyed
        StopDecaySystem();
    }

    // Public method to toggle the panel and display pet details
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
    
    // Close the panel
    public void CloseInfoPanel()
    {
        petsInfoPanel.SetActive(false);
    }
    
    // Check if the panel is currently active
    public bool IsPanelActive()
    {
        return petsInfoPanel != null && petsInfoPanel.activeSelf;
    }
    
    // Display the pet details for the given pet ID
    private void DisplayPetDetails(int petID)
    {
        try
        {
            // Get the logged-in player information
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            
            if (currentUser != null)
            {
                // Fetch the pet details based on playerPetID
                PlayerPet petDetails = APIPlayerPet.GetPlayerPetById(petID);
                
                // Verify this pet belongs to the logged-in player
                if (petDetails != null && petDetails.playerID == currentUser.id)
                {
                    // Update UI elements with pet details
                    UpdatePetInfo(petDetails);
                    
                    // Log status values for debugging
                    Debug.Log($"Pet {petID} status: {petDetails.status}");
                }
                else
                {
                    Debug.LogWarning($"Pet does not belong to the current user or pet details not found. Pet ID: {petID}, User ID: {currentUser.id}");
                }
            }
            else
            {
                Debug.LogWarning("No user is currently logged in.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error displaying pet details: " + ex.Message);
        }
    }
    
    // Update the pet information UI elements
    private void UpdatePetInfo(PlayerPet petDetails)
    {
        if (petDetails == null) return;
        
        // Store the current pet details
        currentPetDetails = petDetails;
        
        // Update basic pet info text
        if (petNameText != null)
            petNameText.text = petDetails.petCustomName;
        
        if (petLevelText != null)
            petLevelText.text = "Level: " + petDetails.level.ToString();
        
        if (petAdoptedDateText != null)
            petAdoptedDateText.text = "Adopted: " + petDetails.adoptedAt.ToString("MM/dd/yyyy");
        
        if (petCustomNameText != null)
            petCustomNameText.text = "Custom Name: " + petDetails.petCustomName;
        
        // Update status bars using the status bar manager
        if (statusBarManager != null)
        {
            // Update level slider
            statusBarManager.UpdateLevelSlider(petDetails.level);
            
            // Update status bars and text
            statusBarManager.UpdatePetStatus(petDetails.status);
        }
        
        // Update UI button states
        UpdateCareButtonStates();
        
        // Start the decay system if it's not running
        StartDecaySystem();
    }
    
    // Coroutine to handle status decay over time
    private System.Collections.IEnumerator StatusDecayCoroutine()
    {
        // Wait for the initial delay
        yield return new WaitForSeconds(decayInterval);
        
        while (true)
        {
            // Check if the current pet details are available
            if (currentPetDetails != null)
            {
                // Decay each status value
                bool wasDecayed = false;
                
                // Parse the status string to get individual values
                string[] statusValues = currentPetDetails.status.Split('%');
                if (statusValues.Length >= 3)
                {
                    // Try to parse each status value
                    int hunger, happiness, energy;
                    if (int.TryParse(statusValues[0], out hunger) && 
                        int.TryParse(statusValues[1], out happiness) && 
                        int.TryParse(statusValues[2], out energy))
                    {
                        // Only decay if above the minimum value
                        if (hunger > minStatusValue)
                        {
                            hunger -= hungerDecayAmount;
                            wasDecayed = true;
                        }
                        
                        if (happiness > minStatusValue)
                        {
                            happiness -= happinessDecayAmount;
                            wasDecayed = true;
                        }
                        
                        if (energy > minStatusValue)
                        {
                            energy -= energyDecayAmount;
                            wasDecayed = true;
                        }
                        
                        // Update the status string if any value was decayed
                        if (wasDecayed)
                        {
                            // Create the new status string
                            string newStatus = $"{hunger}%{happiness}%{energy}";
                            currentPetDetails.status = newStatus;
                            
                            // Update the UI
                            if (statusBarManager != null)
                            {
                                statusBarManager.UpdatePetStatus(newStatus);
                            }
                            
                            // Update button states as statuses may have changed from maxed
                            UpdateCareButtonStates();
                            
                            // Update the database
                            StartCoroutine(UpdatePetInDatabase());
                            
                            Debug.Log($"Pet status decayed: {newStatus}");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to parse status values: " + currentPetDetails.status);
                    }
                }
                else
                {
                    Debug.LogError("Invalid status format: " + currentPetDetails.status);
                }
            }
            
            // Wait for the decay interval before next update
            yield return new WaitForSeconds(decayInterval);
        }
    }
    
    // Update the pet information in the database
    private System.Collections.IEnumerator UpdatePetInDatabase()
    {
        if (currentPetDetails != null)
        {
            // Call the API method to update the pet
            yield return APIPlayerPet.UpdatePlayerPetCoroutine(currentPetDetails, success => {
                if (success)
                {
                    Debug.Log($"Successfully updated pet {currentPetDetails.playerPetID} in database");
                }
                else
                {
                    Debug.LogError($"Failed to update pet {currentPetDetails.playerPetID} in database");
                }
            });
        }
    }
    
    // Start the decay system if it's not already running
    private void StartDecaySystem()
    {
        // Stop any existing decay coroutine first
        StopDecaySystem();
        
        // Start a new decay coroutine
        decayCoroutine = StartCoroutine(StatusDecayCoroutine());
        Debug.Log("Started pet status decay system");
    }
    
    // Stop the decay system
    private void StopDecaySystem()
    {
        if (decayCoroutine != null)
        {
            StopCoroutine(decayCoroutine);
            decayCoroutine = null;
            Debug.Log("Stopped pet status decay system");
        }
    }
    
    // --- Pet Care System ---
    
    // Feed the pet to increase hunger status
    public void FeedPet()
    {
        if (currentPetDetails == null) return;
        
        // Skip if already at max
        if (IsHungerAtMax())
        {
            Debug.Log($"Pet {currentPetDetails.playerPetID} is already full");
            return;
        }
        
        UpdatePetStatus(0, feedIncreaseAmount); // Index 0 is hunger
        Debug.Log($"Fed pet {currentPetDetails.playerPetID}, increasing hunger by {feedIncreaseAmount}");
    }
    
    // Play with the pet to increase happiness status
    public void PlayWithPet()
    {
        if (currentPetDetails == null) return;
        
        // Skip if already at max
        if (IsHappinessAtMax())
        {
            Debug.Log($"Pet {currentPetDetails.playerPetID} is already completely happy");
            return;
        }
        
        UpdatePetStatus(1, playIncreaseAmount); // Index 1 is happiness
        Debug.Log($"Played with pet {currentPetDetails.playerPetID}, increasing happiness by {playIncreaseAmount}");
    }
    
    // Let the pet sleep to increase energy status
    public void SleepPet()
    {
        if (currentPetDetails == null) return;
        
        // Skip if already at max
        if (IsEnergyAtMax())
        {
            Debug.Log($"Pet {currentPetDetails.playerPetID} is already full of energy");
            return;
        }
        
        UpdatePetStatus(2, sleepIncreaseAmount); // Index 2 is energy
        Debug.Log($"Pet {currentPetDetails.playerPetID} slept, increasing energy by {sleepIncreaseAmount}");
    }
    
    // Update a specific pet status value
    public void UpdatePetStatus(int statusIndex, int increaseAmount)
    {
        if (currentPetDetails == null) return;
        
        // Parse the current status string
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length < 3)
        {
            Debug.LogError($"Invalid status format: {currentPetDetails.status}");
            return;
        }
        
        // Parse the status value to update
        int statusValue;
        if (!int.TryParse(statusValues[statusIndex], out statusValue))
        {
            Debug.LogError($"Failed to parse status value: {statusValues[statusIndex]}");
            return;
        }
        
        // Increase the status value, capped at the maximum
        statusValue += increaseAmount;
        statusValue = Mathf.Min(statusValue, maxStatusValue);
        
        // Update the status array
        statusValues[statusIndex] = statusValue.ToString();
        
        // Create the new status string
        string newStatus = string.Join("%", statusValues);
        currentPetDetails.status = newStatus;
        
        // Update the UI
        if (statusBarManager != null)
        {
            statusBarManager.UpdatePetStatus(newStatus);
        }
        
        // Update button states if the status reached max
        UpdateCareButtonStates();
        
        // Update the database
        StartCoroutine(UpdatePetInDatabase());
        
        Debug.Log($"Updated pet status to: {newStatus}");
    }
    
    // --- UI Button Handlers ---
    
    // Handler for the Feed button
    public void OnFeedButtonClicked()
    {
        FeedPet();
    }
    
    // Handler for the Play button
    public void OnPlayButtonClicked()
    {
        PlayWithPet();
    }
    
    // Handler for the Sleep button
    public void OnSleepButtonClicked()
    {
        SleepPet();
    }
    
    // Care for all pet needs at once (for convenience)
    public void OnCareForAllButtonClicked()
    {
        if (currentPetDetails == null) return;
        
        FeedPet();
        PlayWithPet();
        SleepPet();
        
        Debug.Log($"Provided complete care for pet {currentPetDetails.playerPetID}");
    }
    
    // Update UI button interactable states based on pet status
    private void UpdateCareButtonStates()
    {
        // Find care buttons in the scene if they exist
        Button[] buttons = petsInfoPanel.GetComponentsInChildren<Button>(true);
        
        foreach (Button button in buttons)
        {
            // Check button name/tag to determine its function
            if (button.name.Contains("Feed") || button.tag == "FeedButton")
            {
                button.interactable = !IsHungerAtMax();
            }
            else if (button.name.Contains("Play") || button.tag == "PlayButton")
            {
                button.interactable = !IsHappinessAtMax();
            }
            else if (button.name.Contains("Sleep") || button.tag == "SleepButton")
            {
                button.interactable = !IsEnergyAtMax();
            }
            else if (button.name.Contains("CareAll") || button.tag == "CareAllButton")
            {
                button.interactable = !IsAllStatusAtMax();
            }
        }
    }
    
    // --- Status Check Methods ---
    
    // Check if hunger status is at maximum
    public bool IsHungerAtMax()
    {
        if (currentPetDetails == null) return false;
        
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length >= 1 && int.TryParse(statusValues[0], out int hunger))
        {
            return hunger >= maxStatusValue;
        }
        return false;
    }
    
    // Check if happiness status is at maximum
    public bool IsHappinessAtMax()
    {
        if (currentPetDetails == null) return false;
        
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length >= 2 && int.TryParse(statusValues[1], out int happiness))
        {
            return happiness >= maxStatusValue;
        }
        return false;
    }
    
    // Check if energy status is at maximum
    public bool IsEnergyAtMax()
    {
        if (currentPetDetails == null) return false;
        
        string[] statusValues = currentPetDetails.status.Split('%');
        if (statusValues.Length >= 3 && int.TryParse(statusValues[2], out int energy))
        {
            return energy >= maxStatusValue;
        }
        return false;
    }
    
    // Check if all statuses are at maximum
    public bool IsAllStatusAtMax()
    {
        return IsHungerAtMax() && IsHappinessAtMax() && IsEnergyAtMax();
    }
}