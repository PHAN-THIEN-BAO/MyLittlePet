using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IdleObjectClickHandler : MonoBehaviour
{
    public GameObject petsInfoPanel;
    
    public TMP_Text petNameText;
    public TMP_Text petLevelText;
    
    public TMP_Text petHungerStatusText;
    public TMP_Text petHappinessStatusText; 
    public TMP_Text petHealthStatusText;
    
    public TMP_Text petAdoptedDateText;
    public TMP_Text petCustomNameText;
    
    public int playerPetID;

    private void OnMouseDown()
    {
        if (petsInfoPanel != null)
        {
            bool isActive = petsInfoPanel.activeSelf;
            petsInfoPanel.SetActive(!isActive);
            
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
            User currentUser = PlayerInfomation.LoadPlayerInfo();
            
            if (currentUser != null)
            {
                PlayerPet petDetails = APIPlayerPet.GetPlayerPetById(playerPetID);
                
                if (petDetails != null && petDetails.playerID == currentUser.id)
                {
                    if (petNameText != null)
                        petNameText.text = petDetails.petCustomName;
                    
                    //if (petLevelText != null)
                    
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
            string[] statuses = statusString.Split('%');
            
            if (statuses.Length >= 3)
            {
                if (petHungerStatusText != null)
                    petHungerStatusText.text = "Hunger: " + statuses[0];
                
                if (petHappinessStatusText != null)
                    petHappinessStatusText.text = "Happiness: " + statuses[1];
                
                if (petHealthStatusText != null)
                    petHealthStatusText.text = "Health: " + statuses[2];
            }
            else
            {
                Debug.LogWarning("Status string does not contain 3 values: " + statusString);
                
                if (petHungerStatusText != null)
                    petHungerStatusText.text = "Status: " + statusString;
                
                if (petHappinessStatusText != null)
                    petHappinessStatusText.text = "";
                
                if (petHealthStatusText != null)
                    petHealthStatusText.text = "";
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error parsing pet status: " + ex.Message);
            
            if (petHungerStatusText != null)
                petHungerStatusText.text = "Status: " + statusString;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && petsInfoPanel != null && petsInfoPanel.activeSelf)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider == null || hit.collider.gameObject != this.gameObject)
                {
                    petsInfoPanel.SetActive(false);
                }
            }
        }
    }
}