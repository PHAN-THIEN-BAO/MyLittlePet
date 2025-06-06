using UnityEngine;
using UnityEngine.EventSystems;

public class IdleObjectClickHandler : MonoBehaviour
{
    public GameObject petsInfoPanel; // Reference to the panel to toggle

    private void OnMouseDown()
    {
        // Check if PetsInfoPanel is assigned
        if (petsInfoPanel != null)
        {
            // Toggle the panel's visibility
            bool isActive = petsInfoPanel.activeSelf;
            petsInfoPanel.SetActive(!isActive);
        }
        else
        {
            Debug.LogWarning("PetsInfoPanel is not assigned in the Inspector.");
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