using UnityEngine;
public class InteractablePet : MonoBehaviour, IInteractable
{
    [Header("Pet Interaction")]
    public PetInfoUIManager uiManager;
    public bool CanInteract()
    {
        return true;
    }
    public void Interact()
    {
        Debug.Log("Interacted with pet: " + gameObject.name);
        var dataHolder = GetComponent<PetDataHolder>();
        if (dataHolder != null && uiManager != null)
        {
            uiManager.ToggleInfoPanel(dataHolder.petData.playerPetID);
            Debug.Log($"Opened pet info panel for PlayerPetID: {dataHolder.petData.playerPetID}");
        }
        else
        {
            Debug.LogWarning("PetDataHolder or PetInfoUIManager is not assigned to " + gameObject.name);
        }
    }
    public void StopInteract()
    {
    }
    public int GetPlayerPetID()
    {
        var dataHolder = GetComponent<PetDataHolder>();
        if (dataHolder != null && dataHolder.petData != null)
        {
            return dataHolder.petData.playerPetID;
        }
        return -1;
    }
}