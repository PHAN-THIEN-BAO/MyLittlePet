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
        if (careHistoryPanel != null)
        {
            careHistoryPanel.SetActive(false);
            isPanelOpen = false;
        }
    }
    public void OpenCareHistoryPanel()
    {
        if (careHistoryPanel == null)
        {
            Debug.LogError("CareHistoryManager: Care History Panel is not assigned!");
            return;
        }
        if (isPanelOpen) return;
        careHistoryPanel.SetActive(true);
        isPanelOpen = true;
        if (careHistoryLoader != null)
        {
            careHistoryLoader.RefreshCareHistory();
        }
        if (useAnimation)
        {
            AnimateOpenPanel();
        }
        Debug.Log("Care History Panel opened");
    }
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
    private void AnimateOpenPanel()
    {
        if (careHistoryPanel == null) return;
        careHistoryPanel.transform.localScale = Vector3.zero;
        LeanTween.scale(careHistoryPanel, Vector3.one, animationDuration)
            .setEase(LeanTweenType.easeOutBack);
    }
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
    public void OpenWithPlayerHistory()
    {
        OpenCareHistoryPanel();
        if (careHistoryLoader != null)
        {
            careHistoryLoader.SetLoadPlayerHistory();
        }
    }
    public void OpenWithPetHistory(int petId = -1)
    {
        OpenCareHistoryPanel();
        if (careHistoryLoader != null)
        {
            careHistoryLoader.SetLoadPetHistory(petId);
        }
    }
    public void OpenWithAllHistory()
    {
        OpenCareHistoryPanel();
        if (careHistoryLoader != null)
        {
            careHistoryLoader.SetLoadAllHistory();
        }
    }
    public bool IsPanelOpen()
    {
        return isPanelOpen;
    }
    public void RefreshIfOpen()
    {
        if (isPanelOpen && careHistoryLoader != null)
        {
            careHistoryLoader.RefreshCareHistory();
        }
    }
}