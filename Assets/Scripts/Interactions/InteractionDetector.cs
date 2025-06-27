using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null ;
    public GameObject InteractionIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InteractionIcon.SetActive(false);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (interactableInRange is NPC npc)
            {
                npc.playerPetID = PetSelectionManager.CurrentPlayerPetID;
                npc.Interact();
            }
            else
            {
                interactableInRange?.Interact();
            }
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            InteractionIcon.SetActive(true);

            // Nếu là pet, lưu luôn playerPetID
            if (interactable is PetClickHandler petHandler)
            {
                var dataHolder = petHandler.GetComponent<PetDataHolder>();
                if (dataHolder != null)
                    PetSelectionManager.CurrentPlayerPetID = dataHolder.petData.playerPetID;
            }
            // Nếu là NPC, có thể giữ nguyên logic cũ
        }
    }
   private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            InteractionIcon.SetActive(false);
        }
    }
    public void Interact()
    {
        if(interactableInRange != null)
        {
            interactableInRange.Interact();
        }
    }
    public void StopInteract()
    {
        if(interactableInRange != null)
        {
            interactableInRange.StopInteract();
        }
    }

    // Assuming you have a reference to the NPC (npcInstance)
    public void InteractWithNPC(NPC npcInstance)
    {
        npcInstance.playerPetID = PetSelectionManager.CurrentPlayerPetID;
        npcInstance.Interact();
    }
}
