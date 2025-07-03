using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;

public class PlayableOnceController : MonoBehaviour
{
    [Header("Playable Settings")]
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private string playableId; // ID duy nhất cho playable này
    
    [Header("Trigger Settings")]
    [SerializeField] private bool autoPlayOnStart = false;
    [SerializeField] private bool triggerOnPlayerEnter = true;
    
    private static HashSet<string> playedPlayables = new HashSet<string>();
    private bool hasPlayerInTrigger = false;
    
    void Start()
    {
        // Tự động lấy PlayableDirector nếu chưa được gán
        if (playableDirector == null)
        {
            playableDirector = GetComponent<PlayableDirector>();
        }
        
        // Tạo ID tự động nếu chưa có
        if (string.IsNullOrEmpty(playableId))
        {
            playableId = gameObject.name + "_" + transform.position.ToString();
        }
        
        // Load trạng thái đã phát từ PlayerPrefs
        LoadPlayableState();
        
        // Auto play nếu được bật và chưa phát
        if (autoPlayOnStart && !HasBeenPlayed())
        {
            PlayOnce();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnPlayerEnter && other.CompareTag("Player"))
        {
            hasPlayerInTrigger = true;
            if (!HasBeenPlayed())
            {
                PlayOnce();
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hasPlayerInTrigger = false;
        }
    }
    
    /// <summary>
    /// Phát playable một lần duy nhất
    /// </summary>
    public void PlayOnce()
    {
        if (HasBeenPlayed())
        {
            Debug.Log($"Playable {playableId} đã được phát trước đó. Bỏ qua.");
            return;
        }
        
        if (playableDirector == null)
        {
            Debug.LogError("PlayableDirector không được gán!");
            return;
        }
        
        // Phát playable
        playableDirector.Play();
        
        // Đánh dấu đã phát
        MarkAsPlayed();
        
        Debug.Log($"Đã phát playable {playableId} lần đầu tiên.");
    }
    
    /// <summary>
    /// Kiểm tra xem playable đã được phát chưa
    /// </summary>
    public bool HasBeenPlayed()
    {
        return playedPlayables.Contains(playableId);
    }
    
    /// <summary>
    /// Đánh dấu playable đã được phát
    /// </summary>
    private void MarkAsPlayed()
    {
        playedPlayables.Add(playableId);
        SavePlayableState();
    }
    
    /// <summary>
    /// Lưu trạng thái playable vào PlayerPrefs
    /// </summary>
    private void SavePlayableState()
    {
        List<string> playedList = new List<string>(playedPlayables);
        string jsonData = JsonUtility.ToJson(new SerializableList<string>(playedList));
        PlayerPrefs.SetString("PlayedPlayables", jsonData);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Load trạng thái playable từ PlayerPrefs
    /// </summary>
    private void LoadPlayableState()
    {
        if (PlayerPrefs.HasKey("PlayedPlayables"))
        {
            string jsonData = PlayerPrefs.GetString("PlayedPlayables");
            SerializableList<string> loadedList = JsonUtility.FromJson<SerializableList<string>>(jsonData);
            playedPlayables = new HashSet<string>(loadedList.items);
        }
    }
    
    /// <summary>
    /// Reset trạng thái - cho phép phát lại (dùng cho testing)
    /// </summary>
    [ContextMenu("Reset Playable State")]
    public void ResetPlayableState()
    {
        playedPlayables.Remove(playableId);
        SavePlayableState();
        Debug.Log($"Đã reset trạng thái cho playable {playableId}");
    }
    
    /// <summary>
    /// Reset tất cả playable states (dùng cho testing)
    /// </summary>
    public static void ResetAllPlayableStates()
    {
        playedPlayables.Clear();
        PlayerPrefs.DeleteKey("PlayedPlayables");
        PlayerPrefs.Save();
        Debug.Log("Đã reset tất cả trạng thái playable");
    }
    
    /// <summary>
    /// Phương thức public để trigger từ bên ngoài
    /// </summary>
    public void TriggerPlayable()
    {
        PlayOnce();
    }
}

[System.Serializable]
public class SerializableList<T>
{
    public List<T> items;
    
    public SerializableList(List<T> items)
    {
        this.items = items;
    }
}