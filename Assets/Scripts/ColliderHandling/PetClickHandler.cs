using UnityEngine;
using UnityEngine.EventSystems;

public class PetClickHandler : MonoBehaviour
{
    // Reference to the pet's PlayerPetID
    public int playerPetID;
    
    // Reference to the UI manager that will handle displaying pet info
    public PetInfoUIManager uiManager;

    private void OnMouseDown()
    {
        // Check if UI manager is assigned
        if (uiManager != null)
        {
            // Tell the UI manager to toggle the panel and display pet details
            uiManager.ToggleInfoPanel(playerPetID);
        }
        else
        {
            Debug.LogWarning("PetInfoUIManager is not assigned to " + gameObject.name);
        }
    }

    private void Update()
    {
        // Check for mouse click to close panel when clicking elsewhere
        if (Input.GetMouseButtonDown(0) && uiManager != null && uiManager.IsPanelActive())
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
                    uiManager.CloseInfoPanel();
                }
            }
        }
    }
}