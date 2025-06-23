
public interface IInteractable
{
    /// <summary>
    /// Called when the player interacts with the object.
    /// </summary>
    void Interact();
    /// <summary>
    /// Called when the player stops interacting with the object.
    /// </summary>
    void StopInteract();
    /// <summary>
    /// Returns true if the object can be interacted with.
    /// </summary>
    bool CanInteract();
}