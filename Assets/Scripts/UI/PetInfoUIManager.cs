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
    
    // Track the current pet being displayed
    private int currentPetID = -1;

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
    }
}