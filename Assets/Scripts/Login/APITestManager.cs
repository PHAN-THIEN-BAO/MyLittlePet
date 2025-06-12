using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class APITestManager : MonoBehaviour
{
    [Header("Test UI")]
    [SerializeField] private Button testConnectionButton;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TMP_InputField apiUrlField;
    
    void Start()
    {
        if (testConnectionButton != null)
            testConnectionButton.onClick.AddListener(TestAPIConnection);
            
        // Set default API URL
        if (apiUrlField != null)
            apiUrlField.text = "https://localhost:7024/api";
    }
    
    public void TestAPIConnection()
    {
        if (resultText != null)
            resultText.text = "Testing connection...";
            
        if (APIManager.Instance == null)
        {
            ShowResult("APIManager not found in scene!", Color.red);
            return;
        }
        
        // Update API URL if changed
        if (apiUrlField != null && !string.IsNullOrEmpty(apiUrlField.text))
        {
            APIManager.Instance.SetBaseURL(apiUrlField.text);
        }
        
        // Test login with dummy credentials
        APIManager.Instance.Login("test", "test", OnTestLoginResult);
    }
    
    private void OnTestLoginResult(LoginResponse response)
    {
        if (response == null)
        {
            ShowResult("Connection failed - Check API URL and ensure API is running", Color.red);
        }
        else if (response.Success)
        {
            ShowResult("Connection successful - Test user logged in", Color.green);
        }
        else
        {
            ShowResult($"Connection successful - API responded: {response.Message}", Color.yellow);
        }
    }
    
    private void ShowResult(string message, Color color)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.color = color;
        }
        Debug.Log($"API Test: {message}");
    }
}
