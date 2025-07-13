using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeScaleDebugger : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timeScaleText;
    public Button resetButton;
    
    [Header("Debug Settings")]
    public bool showOnGUI = true;
    public bool autoReset = true;
    
    private void Start()
    {
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(() => {
                Time.timeScale = 1f;
                Debug.Log("🔧 Time.timeScale manually reset to 1");
            });
        }
    }
    
    private void Update()
    {
        // Update UI text
        if (timeScaleText != null)
        {
            timeScaleText.text = $"TimeScale: {Time.timeScale:F2}";
            
            // Color coding
            if (Time.timeScale == 1f)
                timeScaleText.color = Color.green;
            else if (Time.timeScale == 0f)
                timeScaleText.color = Color.red;
            else
                timeScaleText.color = Color.yellow;
        }
        
        // Auto reset if needed
        if (autoReset && Time.timeScale != 1f)
        {
            Debug.LogWarning($"Auto-resetting Time.timeScale from {Time.timeScale} to 1");
            Time.timeScale = 1f;
        }
    }
    
    private void OnGUI()
    {
        if (!showOnGUI) return;
        
        GUILayout.BeginArea(new Rect(10, 10, 200, 100));
        GUILayout.Label($"Time Scale: {Time.timeScale:F2}");
        
        if (Time.timeScale != 1f)
        {
            GUI.color = Color.red;
            if (GUILayout.Button("Reset TimeScale"))
            {
                Time.timeScale = 1f;
            }
            GUI.color = Color.white;
        }
        
        GUILayout.EndArea();
    }
}