using UnityEngine;

public class CareHistoryManager : MonoBehaviour
{
    [Header("Care History Panel")]
    [SerializeField] private GameObject careHistoryPanel;
    [SerializeField] private CareHistoryLoader careHistoryLoader;
    
    [Header("Animation Settings")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private float animationDuration = 0.3f;
    
    private bool isPanelOpen = false;

    private void Awake()
    {
        // Đảm bảo panel bị ẩn khi khởi động
        if (careHistoryPanel != null)
        {
            careHistoryPanel.SetActive(false);
            isPanelOpen = false;
        }
    }

    /// <summary>
    /// Mở Care History Panel
    /// </summary>
    public void OpenCareHistoryPanel()
    {
        if (careHistoryPanel == null)
        {
            Debug.LogError("CareHistoryManager: Care History Panel is not assigned!");
            return;
        }

        if (isPanelOpen) return;

        // Kích hoạt panel
        careHistoryPanel.SetActive(true);
        isPanelOpen = true;

        // Refresh dữ liệu khi mở panel
        if (careHistoryLoader != null)
        {
            careHistoryLoader.RefreshCareHistory();
        }

        // Animation mở panel
        if (useAnimation)
        {
            AnimateOpenPanel();
        }

        Debug.Log("Care History Panel opened");
    }

    /// <summary>
    /// Đóng Care History Panel
    /// </summary>
    public void CloseCareHistoryPanel()
    {
        if (careHistoryPanel == null || !isPanelOpen) return;

        if (useAnimation)
        {
            AnimateClosePanel();
        }
        else
        {
            careHistoryPanel.SetActive(false);
            isPanelOpen = false;
        }

        Debug.Log("Care History Panel closed");
    }

    /// <summary>
    /// Toggle Care History Panel (mở/đóng)
    /// </summary>
    public void ToggleCareHistoryPanel()
    {
        if (isPanelOpen)
        {
            CloseCareHistoryPanel();
        }
        else
        {
            OpenCareHistoryPanel();
        }
    }

    /// <summary>
    /// Animation mở panel
    /// </summary>
    private void AnimateOpenPanel()
    {
        if (careHistoryPanel == null) return;

        // Scale từ 0 lên 1
        careHistoryPanel.transform.localScale = Vector3.zero;
        
        LeanTween.scale(careHistoryPanel, Vector3.one, animationDuration)
            .setEase(LeanTweenType.easeOutBack);
    }

    /// <summary>
    /// Animation đóng panel
    /// </summary>
    private void AnimateClosePanel()
    {
        if (careHistoryPanel == null) return;

        LeanTween.scale(careHistoryPanel, Vector3.zero, animationDuration)
            .setEase(LeanTweenType.easeInBack)
            .setOnComplete(() => {
                careHistoryPanel.SetActive(false);
                isPanelOpen = false;
            });
    }

    /// <summary>
    /// Set filter và mở panel
    /// </summary>
    public void OpenWithPlayerHistory()
    {
        OpenCareHistoryPanel();
        if (careHistoryLoader != null)
        {
            careHistoryLoader.SetLoadPlayerHistory();
        }
    }

    /// <summary>
    /// Set filter pet và mở panel
    /// </summary>
    public void OpenWithPetHistory(int petId = -1)
    {
        OpenCareHistoryPanel();
        if (careHistoryLoader != null)
        {
            careHistoryLoader.SetLoadPetHistory(petId);
        }
    }

    /// <summary>
    /// Set filter tất cả và mở panel
    /// </summary>
    public void OpenWithAllHistory()
    {
        OpenCareHistoryPanel();
        if (careHistoryLoader != null)
        {
            careHistoryLoader.SetLoadAllHistory();
        }
    }

    /// <summary>
    /// Kiểm tra panel có đang mở không
    /// </summary>
    public bool IsPanelOpen()
    {
        return isPanelOpen;
    }

    /// <summary>
    /// Refresh dữ liệu nếu panel đang mở
    /// </summary>
    public void RefreshIfOpen()
    {
        if (isPanelOpen && careHistoryLoader != null)
        {
            careHistoryLoader.RefreshCareHistory();
        }
    }
}