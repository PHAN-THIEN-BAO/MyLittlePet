using UnityEngine;

public class PlayerPositionSaver : MonoBehaviour
{
    [Header("Save Position Settings")]
    [SerializeField] private string savePointId;
    [SerializeField] private bool autoGenerateId = true;
    [SerializeField] private bool saveOnTriggerEnter = true;
    [SerializeField] private bool saveOnTriggerExit = false;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject saveIndicator;
    [SerializeField] private float indicatorDisplayTime = 2f;
    [SerializeField] private string saveMessage = "Position Saved!";

    [Header("Audio Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip saveSound;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider == null)
        {
            Debug.LogError($"PlayerPositionSaver on {gameObject.name} requires a Collider2D component!");
            enabled = false;
            return;
        }

        if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            if (enableDebugLogs)
                Debug.LogWarning($"Collider2D on {gameObject.name} was not set as trigger. Automatically set to trigger.");
        }

        if (autoGenerateId && string.IsNullOrEmpty(savePointId))
        {
            savePointId = gameObject.scene.name + "_" + gameObject.name + "_" + transform.GetSiblingIndex();
        }

        if (string.IsNullOrEmpty(savePointId))
        {
            Debug.LogError($"PlayerPositionSaver on {gameObject.name} requires a valid savePointId!");
            enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (saveOnTriggerEnter && other.CompareTag("Player"))
        {
            SavePlayerPosition(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (saveOnTriggerExit && other.CompareTag("Player"))
        {
            SavePlayerPosition(other.transform);
        }
    }

    public void SavePlayerPosition(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Cannot save position: Player transform is null!");
            return;
        }

        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser == null)
        {
            Debug.LogWarning("No user information found. Cannot save player position.");
            return;
        }

        string userId = currentUser.id.ToString();
        Vector3 position = playerTransform.position;

        PlayerPositionManager.SavePlayerPosition(userId, savePointId, position);

        ShowSaveIndicator();

        PlaySaveSound();

        if (enableDebugLogs)
        {
            Debug.Log($"Saved player position for user {userId} at save point {savePointId}: {position}");
        }
    }

    public bool LoadPlayerPosition(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Cannot load position: Player transform is null!");
            return false;
        }

        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser == null)
        {
            Debug.LogWarning("No user information found. Cannot load player position.");
            return false;
        }

        string userId = currentUser.id.ToString();

        if (PlayerPositionManager.HasSavedPosition(userId, savePointId))
        {
            Vector3 savedPosition = PlayerPositionManager.LoadPlayerPosition(userId, savePointId);
            playerTransform.position = savedPosition;

            if (enableDebugLogs)
            {
                Debug.Log($"Loaded player position for user {userId} from save point {savePointId}: {savedPosition}");
            }

            return true;
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.Log($"No saved position found for user {userId} at save point {savePointId}");
            }

            return false;
        }
    }

    private void ShowSaveIndicator()
    {
        if (saveIndicator != null)
        {
            saveIndicator.SetActive(true);

            var textComp = saveIndicator.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComp != null) textComp.text = saveMessage;

            var tmpComp = saveIndicator.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpComp != null) tmpComp.text = saveMessage;

            Invoke(nameof(HideSaveIndicator), indicatorDisplayTime);
        }
    }

    private void HideSaveIndicator()
    {
        if (saveIndicator != null)
        {
            saveIndicator.SetActive(false);
        }
    }

    private void PlaySaveSound()
    {
        if (audioSource != null && saveSound != null)
        {
            audioSource.PlayOneShot(saveSound);
        }
    }

    public void ManualSavePosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SavePlayerPosition(player.transform);
        }
        else
        {
            Debug.LogError("Cannot find player GameObject with tag 'Player'!");
        }
    }

    public bool ManualLoadPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            return LoadPlayerPosition(player.transform);
        }
        else
        {
            Debug.LogError("Cannot find player GameObject with tag 'Player'!");
            return false;
        }
    }

    public void ResetSavedPosition()
    {
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser != null)
        {
            string userId = currentUser.id.ToString();
            PlayerPositionManager.ClearSavedPosition(userId, savePointId);

            if (enableDebugLogs)
            {
                Debug.Log($"Reset saved position for user {userId} at save point {savePointId}");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawIcon(transform.position, "SaveIcon", true);

        if (triggerCollider != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            if (triggerCollider is BoxCollider2D box)
            {
                Gizmos.DrawWireCube(box.offset, box.size);
            }
            else if (triggerCollider is CircleCollider2D circle)
            {
                Gizmos.DrawWireSphere(circle.offset, circle.radius);
            }
        }
    }
}