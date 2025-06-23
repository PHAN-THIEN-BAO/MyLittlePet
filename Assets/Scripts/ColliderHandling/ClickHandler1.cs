using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic; // Add this namespace for List<>

public class IdleObjectClickHandler : MonoBehaviour
{
    public GameObject petsInfoPanel; // Reference to the panel to toggle
    
    // UI Components to display pet details
    public TMP_Text petNameText;
    public TMP_Text petLevelText;
    
    // New UI components for the separate statuses
    public TMP_Text petHungerStatusText;
    public TMP_Text petHappinessStatusText; 
    public TMP_Text petHealthStatusText;
    
    public TMP_Text petAdoptedDateText;
    public TMP_Text petCustomNameText;
    
    // Reference to the pet's PlayerPetID
    public int playerPetID;

    private void OnMouseDown()
    {
        // Check if PetsInfoPanel is assigned
        if (petsInfoPanel != null)
        {
            // Toggle the panel's visibility
            bool isActive = petsInfoPanel.activeSelf;
            petsInfoPanel.SetActive(!isActive);
            
            // If we're showing the panel, load and display the pet information
            if (!isActive)
            {
                DisplayPetDetails();
            }
        }
        else
        {
            Debug.LogWarning("PetsInfoPanel is not assigned in the Inspector.");
        }
    }

    private void DisplayPetDetails()
    {
        try
        {
            // Get the logged-in player information
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            
            if (currentUser != null)
            {
                // Fetch the pet details based on playerPetID
                PlayerPet petDetails = APIPlayerPet.GetPlayerPetById(playerPetID);
                
                // Verify this pet belongs to the logged-in player
                if (petDetails != null && petDetails.playerID == currentUser.id)
                {
                    // Update UI elements with pet details
                    if (petNameText != null)
                        petNameText.text = petDetails.petCustomName;
                    
                    if (petLevelText != null)
                        petLevelText.text = "Level: " + petDetails.level.ToString();
                    
                    // Parse and display the separate statuses
                    ParseAndDisplayStatuses(petDetails.status);
                    
                    if (petAdoptedDateText != null)
                        petAdoptedDateText.text = "Adopted: " + petDetails.adoptedAt.ToString("MM/dd/yyyy");
                    
                    if (petCustomNameText != null)
                        petCustomNameText.text = "Custom Name: " + petDetails.petCustomName;
                }
                else
                {
                    Debug.LogWarning("Pet does not belong to the current user or pet details not found.");
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
    
    private void ParseAndDisplayStatuses(string statusString)
    {
        try
        {
            // Split the status string by the '%' delimiter
            string[] statuses = statusString.Split('%');
            
            // Check if we have 3 status values
            if (statuses.Length >= 3)
            {
                // Display hunger status
                if (petHungerStatusText != null)
                    petHungerStatusText.text = "Hunger: " + statuses[0];
                
                // Display happiness status
                if (petHappinessStatusText != null)
                    petHappinessStatusText.text = "Happiness: " + statuses[1];
                
                // Display health status
                if (petHealthStatusText != null)
                    petHealthStatusText.text = "Health: " + statuses[2];
            }
            else
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
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing pet status: " + ex.Message);
            
            // Display raw status as fallback
            if (petHungerStatusText != null)
                petHungerStatusText.text = "Status: " + statusString;
        }
    }

    private void Update()
    {
        // Check for mouse click to close panel when clicking elsewhere
        if (Input.GetMouseButtonDown(0) && petsInfoPanel != null && petsInfoPanel.activeSelf)
        {
            // Check if the click is over a UI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Perform a 2D raycast to detect clicks on colliders
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                // If the raycast misses or hits something that is NOT the current GameObject, close the panel
                if (hit.collider == null || hit.collider.gameObject != this.gameObject)
                {
                    petsInfoPanel.SetActive(false);
                }
            }
        }
    }
}