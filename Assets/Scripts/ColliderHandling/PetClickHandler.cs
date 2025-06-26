using UnityEngine;

public class PetClickHandler : MonoBehaviour
{
    // Reference to the UI manager that will handle displaying pet info
    public PetInfoUIManager uiManager;

    private void OnMouseDown()
    {
        Debug.Log("Clicked pet: " + gameObject.name);
        
        var dataHolder = GetComponent<PetDataHolder>();
        if (dataHolder != null && uiManager != null)
        {
            // Use the petData to show info
            uiManager.ToggleInfoPanel(dataHolder.petData.playerPetID);
        }
        else
        {
            Debug.LogWarning("PetDataHolder or PetInfoUIManager is not assigned to " + gameObject.name);
        }
    }
}