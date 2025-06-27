using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PetInfoUIManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject petsInfoPanel;
    public GameObject feedingPanel; // Panel that appears when feeding the pet

    [Header("Pet Details UI")]
    public TMP_Text petNameText;
    public TMP_Text petLevelText;
    public TMP_Text petCustomNameText;
    public TMP_Text petAdoptedDateText;
    public Image petImage; // 

    [Header("Pet Sprites")]
    public Sprite[] petSprites; // 

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

        StartDecaySystem();
    }

    private void OnDestroy()
    {
        // Stop the decay coroutine when this component is destroyed
        StopDecaySystem();
    }

    // Public method to toggle the panel and display pet details
    public void ToggleInfoPanel(int petID)
    {
        Debug.Log("ToggleInfoPanel called with petID: " + petID);

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
            petLevelText.text = "Lv. " + petDetails.level.ToString();

        if (petAdoptedDateText != null)
            petAdoptedDateText.text = "Adopted: " + petDetails.adoptedAt.ToString("MM/dd/yyyy");

        if (petCustomNameText != null)
            petCustomNameText.text = "Custom Name: " + petDetails.petCustomName;

        
        if (petImage != null && petSprites != null && petDetails.petID >= 0 && petDetails.petID < petSprites.Length)
        {
            petImage.sprite = petSprites[petDetails.petID];
        }

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
            yield return APIPlayerPet.UpdatePlayerPetCoroutine(currentPetDetails, success =>
            {
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

    public void FeedPet(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        if (IsHungerAtMax(playerPetID)) return;
        UpdatePetStatus(0, feedIncreaseAmount, playerPetID);
    }

    public void PlayWithPet(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        if (IsHappinessAtMax(playerPetID)) return;
        UpdatePetStatus(1, playIncreaseAmount, playerPetID);
    }

    public void SleepPet(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        if (IsEnergyAtMax(playerPetID)) return;
        UpdatePetStatus(2, sleepIncreaseAmount, playerPetID);
    }

    public void UpdatePetStatus(int statusIndex, int increaseAmount, int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length < 3) return;
        if (!int.TryParse(statusValues[statusIndex], out int statusValue)) return;
        statusValue += increaseAmount;
        statusValue = Mathf.Min(statusValue, maxStatusValue);
        statusValues[statusIndex] = statusValue.ToString();
        string newStatus = string.Join("%", statusValues);
        pet.status = newStatus;
        // Cập nhật database
        StartCoroutine(APIPlayerPet.UpdatePlayerPetCoroutine(pet, null));
    }

    // --- UI Button Handlers ---

    public void OnFeedButtonClicked()
    {
        // If the feeding panel is active, use the pending feed amount
        if (feedingPanel != null && feedingPanel.activeSelf)
        {
            // Use the pending feed amount to update hunger
            UpdatePetStatus(0, pendingFeedAmount, currentPetID); // 0 is hunger index

            // Hide the panel after feeding
            feedingPanel.SetActive(false);

            Debug.Log($"Pet fed with {pendingFeedAmount} amount of food");
        }
        else
        {
            // Default behavior when button is clicked outside of the feeding panel flow
            FeedPet(currentPetID);
        }
    }

    public void OnPlayButtonClicked()
    {
        PlayWithPet(currentPetID);
    }

    public void OnSleepButtonClicked()
    {
        SleepPet(currentPetID);
    }

    public void OnCareForAllButtonClicked()
    {
        if (currentPetDetails == null) return;

        FeedPet(currentPetID);
        PlayWithPet(currentPetID);
        SleepPet(currentPetID);

        Debug.Log($"Provided complete care for pet {currentPetDetails.playerPetID}");
    }

    public void OnCareForAllButtonClicked(int playerPetID)
    {
        FeedPet(playerPetID);
        PlayWithPet(playerPetID);
        SleepPet(playerPetID);
        Debug.Log($"Provided complete care for pet {playerPetID}");
    }

    private void UpdateCareButtonStates()
    {
        // Find care buttons in the scene if they exist
        Button[] buttons = petsInfoPanel.GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            // Check button name/tag to determine its function
            if (button.name.Contains("Feed") || button.tag == "FeedButton")
            {
                button.interactable = !IsHungerAtMax(currentPetID);
            }
            else if (button.name.Contains("Play") || button.tag == "PlayButton")
            {
                button.interactable = !IsHappinessAtMax(currentPetID);
            }
            else if (button.name.Contains("Sleep") || button.tag == "SleepButton")
            {
                button.interactable = !IsEnergyAtMax(currentPetID);
            }
            else if (button.name.Contains("CareAll") || button.tag == "CareAllButton")
            {
                button.interactable = !IsAllStatusAtMax(currentPetID);
            }
        }
    }

    // --- Status Check Methods ---

    public bool IsHungerAtMax(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return false;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length >= 1 && int.TryParse(statusValues[0], out int hunger))
        {
            return hunger >= maxStatusValue;
        }
        return false;
    }

    public bool IsHappinessAtMax(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return false;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length >= 2 && int.TryParse(statusValues[1], out int happiness))
        {
            return happiness >= maxStatusValue;
        }
        return false;
    }

    public bool IsEnergyAtMax(int playerPetID)
    {
        PlayerPet pet = APIPlayerPet.GetPlayerPetById(playerPetID);
        if (pet == null) return false;
        string[] statusValues = pet.status.Split('%');
        if (statusValues.Length >= 3 && int.TryParse(statusValues[2], out int energy))
        {
            return energy >= maxStatusValue;
        }
        return false;
    }

    public bool IsAllStatusAtMax(int playerPetID)
    {
        return IsHungerAtMax(playerPetID) && IsHappinessAtMax(playerPetID) && IsEnergyAtMax(playerPetID);
    }

    // Store the care amount to be used by the feeding panel
    [HideInInspector]
    public int pendingFeedAmount = 0;

    public void ShowFeedingPanel(int customCareAmount = 0)
    {
        if (feedingPanel != null)
        {
            // Store the care amount for use by buttons in the feeding panel
            pendingFeedAmount = customCareAmount > 0 ? customCareAmount : feedIncreaseAmount;

            // Activate the feeding panel
            feedingPanel.SetActive(true);

            // Optional: Update UI elements in the feeding panel to show available food options
            UpdateFeedingPanelUI();
        }
        else
        {
            Debug.LogWarning("Feeding panel is not assigned in the inspector!");
        }
    }

    private void UpdateFeedingPanelUI()
    {
        // This method can be implemented to update food option buttons, 
        // display current hunger level, etc.
    }

    private System.Collections.IEnumerator HideFeedingPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedingPanel != null)
        {
            feedingPanel.SetActive(false);
        }
    }
}