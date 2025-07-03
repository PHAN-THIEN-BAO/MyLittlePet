using UnityEngine;

public class PlayerPositionSaver : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string savePointId; // ID duy nhất cho save point này
    [SerializeField] private bool saveOnEnter = true; // Lưu khi player vào trigger
    [SerializeField] private bool saveOnExit = false; // Lưu khi player rời trigger
    [SerializeField] private bool showDebugMessage = true;
    
    [Header("Visual Feedback (Optional)")]
    [SerializeField] private GameObject saveEffectPrefab; // Effect khi lưu
    [SerializeField] private AudioClip saveSound; // Âm thanh khi lưu
    
    private AudioSource audioSource;
    
    void Start()
    {
        // Tạo ID tự động nếu chưa có
        if (string.IsNullOrEmpty(savePointId))
        {
            savePointId = gameObject.name + "_" + transform.position.ToString();
        }
        
        // Lấy AudioSource component
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (saveOnEnter && other.CompareTag("Player"))
        {
            SavePlayerPosition(other.transform);
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (saveOnExit && other.CompareTag("Player"))
        {
            SavePlayerPosition(other.transform);
        }
    }
    
    /// <summary>
    /// Lưu vị trí người chơi
    /// </summary>
    /// <param name="playerTransform">Transform của player</param>
    private void SavePlayerPosition(Transform playerTransform)
    {
        Vector3 playerPosition = playerTransform.position;
        
        // Lưu vị trí vào PlayerPrefs với key duy nhất
        string positionKey = "PlayerPosition_" + savePointId;
        string positionData = JsonUtility.ToJson(new SerializableVector3(playerPosition));
        PlayerPrefs.SetString(positionKey, positionData);
        
        // Lưu vị trí chung cho toàn bộ game
        PlayerPrefs.SetString("LastPlayerPosition", positionData);
        PlayerPrefs.SetString("LastSavePointId", savePointId);
        PlayerPrefs.Save();
        
        if (showDebugMessage)
        {
            Debug.Log($"Đã lưu vị trí player tại {savePointId}: {playerPosition}");
        }
        
        // Hiệu ứng và âm thanh
        PlaySaveEffects(playerPosition);
    }
    
    /// <summary>
    /// Phát hiệu ứng khi lưu
    /// </summary>
    private void PlaySaveEffects(Vector3 position)
    {
        // Hiệu ứng visual
        if (saveEffectPrefab != null)
        {
            GameObject effect = Instantiate(saveEffectPrefab, position, Quaternion.identity);
            // Tự động xóa effect sau 2 giây
            Destroy(effect, 2f);
        }
        
        // Âm thanh
        if (saveSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(saveSound);
        }
    }
    
    /// <summary>
    /// Load vị trí đã lưu của player
    /// </summary>
    /// <param name="savePointId">ID của save point (null để lấy vị trí cuối cùng)</param>
    /// <returns>Vị trí đã lưu hoặc Vector3.zero nếu không có</returns>
    public static Vector3 LoadPlayerPosition(string savePointId = null)
    {
        string positionKey;
        
        if (string.IsNullOrEmpty(savePointId))
        {
            // Lấy vị trí cuối cùng
            positionKey = "LastPlayerPosition";
        }
        else
        {
            // Lấy vị trí của save point cụ thể
            positionKey = "PlayerPosition_" + savePointId;
        }
        
        if (PlayerPrefs.HasKey(positionKey))
        {
            string positionData = PlayerPrefs.GetString(positionKey);
            SerializableVector3 savedPosition = JsonUtility.FromJson<SerializableVector3>(positionData);
            return savedPosition.ToVector3();
        }
        
        return Vector3.zero;
    }
    
    /// <summary>
    /// Lấy ID của save point cuối cùng
    /// </summary>
    public static string GetLastSavePointId()
    {
        return PlayerPrefs.GetString("LastSavePointId", "");
    }
    
    /// <summary>
    /// Kiểm tra xem có vị trí đã lưu không
    /// </summary>
    public static bool HasSavedPosition(string savePointId = null)
    {
        string positionKey = string.IsNullOrEmpty(savePointId) ? 
            "LastPlayerPosition" : 
            "PlayerPosition_" + savePointId;
        
        return PlayerPrefs.HasKey(positionKey);
    }
    
    /// <summary>
    /// Xóa vị trí đã lưu
    /// </summary>
    public static void ClearSavedPosition(string savePointId = null)
    {
        if (string.IsNullOrEmpty(savePointId))
        {
            // Xóa tất cả
            PlayerPrefs.DeleteKey("LastPlayerPosition");
            PlayerPrefs.DeleteKey("LastSavePointId");
        }
        else
        {
            // Xóa save point cụ thể
            PlayerPrefs.DeleteKey("PlayerPosition_" + savePointId);
        }
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Lưu vị trí thủ công từ bên ngoài
    /// </summary>
    public void ManualSave()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            SavePlayerPosition(player.transform);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy GameObject với tag 'Player'");
        }
    }
}

[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;
    
    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }
    
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}