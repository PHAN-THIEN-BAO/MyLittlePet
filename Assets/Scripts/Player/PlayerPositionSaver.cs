using UnityEngine;

/// <summary>
/// Component để save/load vị trí player thông qua trigger collider 2D
/// </summary>
public class PlayerPositionSaver : MonoBehaviour
{
    [Header("Save Position Settings")]
    [SerializeField] private string savePointId; // ID duy nhất cho save point này
    [SerializeField] private bool autoGenerateId = true; // Tự động tạo ID
    [SerializeField] private bool saveOnTriggerEnter = true; // Save khi player vào trigger
    [SerializeField] private bool saveOnTriggerExit = false; // Save khi player rời trigger

    [Header("Visual Feedback")]
    [SerializeField] private GameObject saveIndicator; // UI hiển thị khi save
    [SerializeField] private float indicatorDisplayTime = 2f; // Thời gian hiển thị indicator
    [SerializeField] private string saveMessage = "Position Saved!";

    [Header("Audio Feedback")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip saveSound;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private Collider2D triggerCollider;

    private void Awake()
    {
        // Lấy collider component
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider == null)
        {
            Debug.LogError($"PlayerPositionSaver on {gameObject.name} requires a Collider2D component!");
            enabled = false;
            return;
        }

        // Đảm bảo collider là trigger
        if (!triggerCollider.isTrigger)
        {
            triggerCollider.isTrigger = true;
            if (enableDebugLogs)
                Debug.LogWarning($"Collider2D on {gameObject.name} was not set as trigger. Automatically set to trigger.");
        }

        // Tự động tạo ID nếu cần
        if (autoGenerateId && string.IsNullOrEmpty(savePointId))
        {
            savePointId = gameObject.scene.name + "_" + gameObject.name + "_" + transform.GetSiblingIndex();
        }

        // Validate save point ID
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

    /// <summary>
    /// Save vị trí player hiện tại
    /// </summary>
    public void SavePlayerPosition(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Cannot save position: Player transform is null!");
            return;
        }

        // Lấy thông tin user hiện tại
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser == null)
        {
            Debug.LogWarning("No user information found. Cannot save player position.");
            return;
        }

        string userId = currentUser.id.ToString();
        Vector3 position = playerTransform.position;

        // Save position
        PlayerPositionManager.SavePlayerPosition(userId, savePointId, position);

        // Visual feedback
        ShowSaveIndicator();

        // Audio feedback
        PlaySaveSound();

        if (enableDebugLogs)
        {
            Debug.Log($"Saved player position for user {userId} at save point {savePointId}: {position}");
        }
    }

    /// <summary>
    /// Load vị trí player đã save (nếu có)
    /// </summary>
    public bool LoadPlayerPosition(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            Debug.LogError("Cannot load position: Player transform is null!");
            return false;
        }

        // Lấy thông tin user hiện tại
        User currentUser = PlayerInfomation.LoadPlayerInfo();
        if (currentUser == null)
        {
            Debug.LogWarning("No user information found. Cannot load player position.");
            return false;
        }

        string userId = currentUser.id.ToString();

        // Kiểm tra xem có saved position không
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

    /// <summary>
    /// Hiển thị indicator khi save
    /// </summary>
    private void ShowSaveIndicator()
    {
        if (saveIndicator != null)
        {
            saveIndicator.SetActive(true);

            // Tìm text component và cập nhật message
            var textComp = saveIndicator.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComp != null) textComp.text = saveMessage;

            var tmpComp = saveIndicator.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmpComp != null) tmpComp.text = saveMessage;

            // Tự động ẩn sau một thời gian
            Invoke(nameof(HideSaveIndicator), indicatorDisplayTime);
        }
    }

    /// <summary>
    /// Ẩn save indicator
    /// </summary>
    private void HideSaveIndicator()
    {
        if (saveIndicator != null)
        {
            saveIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Phát âm thanh khi save
    /// </summary>
    private void PlaySaveSound()
    {
        if (audioSource != null && saveSound != null)
        {
            audioSource.PlayOneShot(saveSound);
        }
    }

    /// <summary>
    /// Public method để save position từ script khác
    /// </summary>
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

    /// <summary>
    /// Public method để load position từ script khác
    /// </summary>
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

    /// <summary>
    /// Reset saved position cho save point này
    /// </summary>
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

    // Gizmos để visualize save point trong editor
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