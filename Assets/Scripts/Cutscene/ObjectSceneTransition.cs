using UnityEngine;

public class ObjectSceneTransition : MonoBehaviour
{
    [Header("Scene Transition Settings")]
    [SerializeField] private string targetSceneName = "";
    [SerializeField] private Vector3 playerSpawnPosition = Vector3.zero;
    
    [Header("Interaction Method")]
    [SerializeField] private TransitionMethod transitionMethod = TransitionMethod.OnTrigger;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Header("UI Feedback")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private string promptMessage = "Press E to enter";
    
    [Header("Advanced Settings")]
    [SerializeField] private bool useOnce = false;
    [SerializeField] private bool destroyAfterUse = false;
    
    private bool playerInRange = false;
    private bool hasBeenUsed = false;

    public enum TransitionMethod
    {
        OnTrigger,
        KeyPress,
        OnClick
    }

    void Update()
    {
        if (transitionMethod == TransitionMethod.KeyPress && 
            playerInRange && 
            Input.GetKeyDown(interactKey) && 
            CanUseTransition())
        {
            TriggerTransition();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            if (transitionMethod == TransitionMethod.OnTrigger && CanUseTransition())
            {
                TriggerTransition();
            }
            else if (transitionMethod == TransitionMethod.KeyPress)
            {
                ShowInteractionPrompt();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            HideInteractionPrompt();
        }
    }

    private void OnMouseDown()
    {
        if (transitionMethod == TransitionMethod.OnClick && CanUseTransition())
        {
            TriggerTransition();
        }
    }

    private bool CanUseTransition()
    {
        return !useOnce || !hasBeenUsed;
    }

    private void TriggerTransition()
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError($"Target scene name is not set on {gameObject.name}!");
            return;
        }

        if (useOnce) hasBeenUsed = true;
        HideInteractionPrompt();

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName, playerSpawnPosition);
        }
        else
        {
            Debug.LogError("SceneTransitionManager not found!");
        }

        if (destroyAfterUse)
        {
            Destroy(gameObject, 0.1f);
        }

        Debug.Log($"Transitioning to: {targetSceneName} at position: {playerSpawnPosition}");
    }

    private void ShowInteractionPrompt()
    {
        if (interactionPrompt != null && CanUseTransition())
        {
            interactionPrompt.SetActive(true);
            
            var textComp = interactionPrompt.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComp != null) textComp.text = promptMessage;
            
            var tmpComp = interactionPrompt.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpComp != null) tmpComp.text = promptMessage;
        }
    }

    private void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    public void ManualTrigger() => TriggerTransition();
    public void ResetUsage() => hasBeenUsed = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(playerSpawnPosition, Vector3.one);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && col.isTrigger)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            
            if (col is BoxCollider2D box)
                Gizmos.DrawWireCube(box.offset, box.size);
            else if (col is CircleCollider2D circle)
                Gizmos.DrawWireSphere(circle.offset, circle.radius);
        }
    }
}