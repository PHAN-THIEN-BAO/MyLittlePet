using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Complete login system demonstration script
/// This shows how to use all the login components together
/// </summary>
public class LoginDemo : MonoBehaviour
{
    [Header("Demo UI")]
    [SerializeField] private Button createUIButton;
    [SerializeField] private Button testAPIButton;
    [SerializeField] private Button createTestUserButton;
    [SerializeField] private Button showUserInfoButton;
    [SerializeField] private Button logoutButton;
    [SerializeField] private TextMeshProUGUI demoStatusText;
    
    [Header("Demo Settings")]
    [SerializeField] private string testUsername = "testuser";
    [SerializeField] private string testPassword = "testpass123";
    [SerializeField] private string testEmail = "test@example.com";

    void Start()
    {
        SetupDemoButtons();
        UpdateStatus("Login Demo Ready - Use buttons to test functionality");
    }

    private void SetupDemoButtons()
    {
        if (createUIButton != null)
            createUIButton.onClick.AddListener(CreateLoginUI);
            
        if (testAPIButton != null)
            testAPIButton.onClick.AddListener(TestAPIConnection);
            
        if (createTestUserButton != null)
            createTestUserButton.onClick.AddListener(CreateTestUser);
            
        if (showUserInfoButton != null)
            showUserInfoButton.onClick.AddListener(ShowCurrentUserInfo);
            
        if (logoutButton != null)
            logoutButton.onClick.AddListener(LogoutUser);
    }

    public void CreateLoginUI()
    {
        var uiCreator = FindObjectOfType<LoginUICreator>();
        if (uiCreator == null)
        {
            GameObject creatorObj = new GameObject("LoginUICreator");
            uiCreator = creatorObj.AddComponent<LoginUICreator>();
        }
        
        uiCreator.CreateLoginUI();
        UpdateStatus("Login UI created! Check your Canvas for the new login interface.");
    }

    public void TestAPIConnection()
    {
        UpdateStatus("Testing API connection...");
        
        if (APIManager.Instance == null)
        {
            UpdateStatus("ERROR: APIManager not found. Create managers first!");
            return;
        }

        // Test with dummy credentials to check if API responds
        APIManager.Instance.Login("dummy", "dummy", OnTestLoginResult);
    }

    private void OnTestLoginResult(LoginResponse response)
    {
        if (response == null)
        {
            UpdateStatus("API Connection FAILED - Check if API is running at https://localhost:7024");
        }
        else
        {
            UpdateStatus($"API Connection SUCCESS - Server responded: {response.Message}");
        }
    }

    public void CreateTestUser()
    {
        UpdateStatus("Creating test user...");
        
        if (APIManager.Instance == null)
        {
            UpdateStatus("ERROR: APIManager not found!");
            return;
        }

        RegisterRequest request = new RegisterRequest
        {
            UserName = testUsername,
            Email = testEmail,
            Password = testPassword,
            Role = "Player"
        };

        APIManager.Instance.Register(request, OnTestUserCreated);
    }

    private void OnTestUserCreated(LoginResponse response)
    {
        if (response == null)
        {
            UpdateStatus("Failed to create test user - API connection issue");
        }
        else if (response.Success)
        {
            UpdateStatus($"Test user created successfully! Username: {testUsername}, Password: {testPassword}");
        }
        else
        {
            UpdateStatus($"Test user creation failed: {response.Message}");
        }
    }

    public void ShowCurrentUserInfo()
    {
        if (UserSessionManager.Instance == null)
        {
            UpdateStatus("ERROR: UserSessionManager not found!");
            return;
        }

        if (UserSessionManager.Instance.IsLoggedIn)
        {
            var user = UserSessionManager.Instance.CurrentUser;
            UpdateStatus($"Current User: {user.UserName} | Level: {user.Level} | Coins: {user.Coin} | Diamonds: {user.Diamond}");
        }
        else
        {
            UpdateStatus("No user is currently logged in");
        }
    }

    public void LogoutUser()
    {
        if (UserSessionManager.Instance != null)
        {
            UserSessionManager.Instance.ClearUserSession();
            UpdateStatus("User logged out successfully");
        }
        else
        {
            UpdateStatus("ERROR: UserSessionManager not found!");
        }
    }

    private void UpdateStatus(string message)
    {
        if (demoStatusText != null)
        {
            demoStatusText.text = $"[{System.DateTime.Now:HH:mm:ss}] {message}";
        }
        
        Debug.Log($"LoginDemo: {message}");
    }

    // Context menu commands for easy testing
    [ContextMenu("Setup Login System")]
    public void SetupLoginSystem()
    {
        var setupScript = FindObjectOfType<LoginSystemSetup>();
        if (setupScript == null)
        {
            GameObject setupObj = new GameObject("LoginSystemSetup");
            setupScript = setupObj.AddComponent<LoginSystemSetup>();
        }
        
        setupScript.CreateRequiredManagers();
        UpdateStatus("Login system managers created");
    }

    [ContextMenu("Quick Test Full Flow")]
    public void QuickTestFullFlow()
    {
        StartCoroutine(TestFullLoginFlow());
    }

    private IEnumerator TestFullLoginFlow()
    {
        UpdateStatus("Starting full login flow test...");
        
        // 1. Setup system
        SetupLoginSystem();
        yield return new WaitForSeconds(1);
        
        // 2. Test API
        TestAPIConnection();
        yield return new WaitForSeconds(2);
        
        // 3. Create test user
        CreateTestUser();
        yield return new WaitForSeconds(3);
        
        // 4. Try login (simulate)
        if (APIManager.Instance != null)
        {
            APIManager.Instance.Login(testUsername, testPassword, OnFullTestLoginResult);
        }
        
        yield return new WaitForSeconds(2);
        
        // 5. Show user info
        ShowCurrentUserInfo();
        
        UpdateStatus("Full login flow test completed!");
    }

    private void OnFullTestLoginResult(LoginResponse response)
    {
        if (response != null && response.Success)
        {
            UpdateStatus("Full test LOGIN SUCCESS - User can now access the game!");
        }
        else
        {
            UpdateStatus("Full test login failed - Check API and credentials");
        }
    }
}
