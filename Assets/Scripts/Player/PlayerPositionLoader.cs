using UnityEngine;
using System.Collections;

public class PlayerPositionLoader : MonoBehaviour
{
    [Header("Loading Settings")]
    [SerializeField] private bool loadOnStart = true;
    [SerializeField] private string specificSavePointId = ""; // Để trống để load vị trí cuối cùng
    [SerializeField] private float loadDelay = 0.1f; // Delay để đảm bảo player đã được khởi tạo
    
    [Header("Fallback Settings")]
    [SerializeField] private Vector3 defaultPosition = Vector3.zero;
    [SerializeField] private bool useDefaultIfNoSave = true;
    
    void Start()
    {
        if (loadOnStart)
        {
            StartCoroutine(LoadPlayerPositionDelayed());
        }
    }
    
    IEnumerator LoadPlayerPositionDelayed()
    {
        yield return new WaitForSeconds(loadDelay);
        
        LoadPlayerPosition();
    }
    
    /// <summary>
    /// Load và áp dụng vị trí đã lưu cho player
    /// </summary>
    public void LoadPlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Không tìm thấy GameObject với tag 'Player'");
            return;
        }
        
        Vector3 savedPosition = PlayerPositionSaver.LoadPlayerPosition(specificSavePointId);
        
        if (savedPosition != Vector3.zero)
        {
            // Có vị trí đã lưu
            player.transform.position = savedPosition;
            Debug.Log($"Đã load vị trí player: {savedPosition} từ save point: {PlayerPositionSaver.GetLastSavePointId()}");
        }
        else if (useDefaultIfNoSave)
        {
            // Không có vị trí đã lưu, dùng vị trí mặc định
            player.transform.position = defaultPosition;
            Debug.Log($"Không có vị trí đã lưu, sử dụng vị trí mặc định: {defaultPosition}");
        }
        else
        {
            Debug.Log("Không có vị trí đã lưu và không sử dụng vị trí mặc định");
        }
    }
    
    /// <summary>
    /// Load vị trí từ save point cụ thể
    /// </summary>
    public void LoadFromSavePoint(string savePointId)
    {
        specificSavePointId = savePointId;
        LoadPlayerPosition();
    }
    
    /// <summary>
    /// Đặt vị trí mặc định mới
    /// </summary>
    public void SetDefaultPosition(Vector3 newPosition)
    {
        defaultPosition = newPosition;
    }
}