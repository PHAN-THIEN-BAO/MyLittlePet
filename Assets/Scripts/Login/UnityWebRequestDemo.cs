using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Comprehensive test script that demonstrates UnityWebRequest API calls
/// for login and registration functionality
/// </summary>
public class UnityWebRequestDemo : MonoBehaviour
{
    [Header("Test UI Elements")]
    [SerializeField] private TMP_InputField testUsernameField;
    [SerializeField] private TMP_InputField testPasswordField;
    [SerializeField] private TMP_InputField testEmailField;
    [SerializeField] private Button testLoginButton;
    [SerializeField] private Button testRegisterButton;
    [SerializeField] private Button createNewUserButton;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private ScrollRect logScrollRect;
    [SerializeField] private TextMeshProUGUI logText;

    [Header("Test Data")]
    [SerializeField] private string existingUsername = "TestUser";
    [SerializeField] private string existingPassword = "password123";
    
    private string logContent = "";

    void Start()
    {
        SetupUI();
        LogMessage("UnityWebRequest Demo Ready!");
        LogMessage($"API Base URL: {(APIManager.Instance != null ? "http://localhost:5118/api" : "APIManager not found")}");
    }

    private void SetupUI()
    {
        if (testLoginButton != null)
            testLoginButton.onClick.AddListener(TestExistingUserLogin);
            
        if (testRegisterButton != null)
            testRegisterButton.onClick.AddListener(TestUserRegistration);
            
        if (createNewUserButton != null)
            createNewUserButton.onClick.AddListener(CreateAndTestNewUser);

        // Set default values
        if (testUsernameField != null)
            testUsernameField.text = existingUsername;
        if (testPasswordField != null)
            testPasswordField.text = existingPassword;
        if (testEmailField != null)
            testEmailField.text = "newuser@example.com";
    }

    [ContextMenu("Test Existing User Login")]
    public void TestExistingUserLogin()
    {
        string username = testUsernameField != null ? testUsernameField.text : existingUsername;
        string password = testPasswordField != null ? testPasswordField.text : existingPassword;
        
        LogMessage($"🔄 Testing login with existing user: {username}");
        
        if (APIManager.Instance == null)
        {
            LogMessage("❌ ERROR: APIManager not found! Make sure to create it first.");
            return;
        }

        StartCoroutine(TestLoginCoroutine(username, password));
    }

    [ContextMenu("Test User Registration")]
    public void TestUserRegistration()
    {
        string username = testUsernameField != null ? testUsernameField.text : "NewTestUser";
        string password = testPasswordField != null ? testPasswordField.text : "newpassword123";
        string email = testEmailField != null ? testEmailField.text : "newuser@example.com";
        
        LogMessage($"🔄 Testing registration for new user: {username}");
        
        if (APIManager.Instance == null)
        {
            LogMessage("❌ ERROR: APIManager not found! Make sure to create it first.");
            return;
        }

        StartCoroutine(TestRegistrationCoroutine(username, email, password));
    }

    [ContextMenu("Create And Test New User")]
    public void CreateAndTestNewUser()
    {
        string randomId = System.DateTime.Now.Ticks.ToString().Substring(10);
        string username = $"TestUser{randomId}";
        string email = $"test{randomId}@example.com";
        string password = "testpass123";
        
        LogMessage($"🔄 Creating and testing completely new user: {username}");
        
        if (APIManager.Instance == null)
        {
            LogMessage("❌ ERROR: APIManager not found! Make sure to create it first.");
            return;
        }

        StartCoroutine(CreateAndTestNewUserCoroutine(username, email, password));
    }

    private IEnumerator TestLoginCoroutine(string username, string password)
    {
        LogMessage($"📤 Sending UnityWebRequest POST to /Users/login");
        LogMessage($"   Username: {username}");
        LogMessage($"   Password: {password}");
        
        bool responseReceived = false;
        LoginResponse loginResponse = null;

        APIManager.Instance.Login(username, password, (response) =>
        {
            loginResponse = response;
            responseReceived = true;
        });

        // Wait for response
        yield return new WaitUntil(() => responseReceived);

        if (loginResponse == null)
        {
            LogMessage("❌ Login FAILED: No response from server");
            LogMessage("   Possible issues:");
            LogMessage("   - API server not running");
            LogMessage("   - Network connection problem");
            LogMessage("   - Invalid API URL");
        }
        else if (loginResponse.Success)
        {
            LogMessage("✅ Login SUCCESS!");
            LogMessage($"   User ID: {loginResponse.User.ID}");
            LogMessage($"   Username: {loginResponse.User.UserName}");
            LogMessage($"   Email: {loginResponse.User.Email}");
            LogMessage($"   Level: {loginResponse.User.Level}");
            LogMessage($"   Coins: {loginResponse.User.Coin}");
            LogMessage($"   Token: {loginResponse.Token.Substring(0, 20)}...");
            
            UpdateResultText($"Login Success! Welcome {loginResponse.User.UserName}");
        }
        else
        {
            LogMessage($"❌ Login FAILED: {loginResponse.Message}");
            UpdateResultText($"Login Failed: {loginResponse.Message}");
        }
    }

    private IEnumerator TestRegistrationCoroutine(string username, string email, string password)
    {
        LogMessage($"📤 Sending UnityWebRequest POST to /Users/register");
        LogMessage($"   Username: {username}");
        LogMessage($"   Email: {email}");
        LogMessage($"   Password: {password}");
        
        bool responseReceived = false;
        LoginResponse registerResponse = null;

        RegisterRequest request = new RegisterRequest
        {
            UserName = username,
            Email = email,
            Password = password,
            Role = "Player"
        };

        APIManager.Instance.Register(request, (response) =>
        {
            registerResponse = response;
            responseReceived = true;
        });

        // Wait for response
        yield return new WaitUntil(() => responseReceived);

        if (registerResponse == null)
        {
            LogMessage("❌ Registration FAILED: No response from server");
        }
        else if (registerResponse.Success)
        {
            LogMessage("✅ Registration SUCCESS!");
            LogMessage($"   New User ID: {registerResponse.User.ID}");
            LogMessage($"   Username: {registerResponse.User.UserName}");
            LogMessage($"   Email: {registerResponse.User.Email}");
            LogMessage($"   Starting Coins: {registerResponse.User.Coin}");
            
            UpdateResultText($"Registration Success! Welcome {registerResponse.User.UserName}");
        }
        else
        {
            LogMessage($"❌ Registration FAILED: {registerResponse.Message}");
            UpdateResultText($"Registration Failed: {registerResponse.Message}");
        }
    }

    private IEnumerator CreateAndTestNewUserCoroutine(string username, string email, string password)
    {
        // First, register the user
        yield return StartCoroutine(TestRegistrationCoroutine(username, email, password));
        
        // Wait a moment
        yield return new WaitForSeconds(1f);
        
        // Then, test login with the new user
        LogMessage("🔄 Now testing login with the newly created user...");
        yield return StartCoroutine(TestLoginCoroutine(username, password));
    }

    private void LogMessage(string message)
    {
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string logEntry = $"[{timestamp}] {message}";
        
        logContent += logEntry + "\n";
        
        if (logText != null)
        {
            logText.text = logContent;
            
            // Auto-scroll to bottom
            if (logScrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                logScrollRect.verticalNormalizedPosition = 0f;
            }
        }
        
        Debug.Log($"UnityWebRequestDemo: {message}");
    }

    private void UpdateResultText(string message)
    {
        if (resultText != null)
        {
            resultText.text = message;
            resultText.color = message.Contains("Success") ? Color.green : Color.red;
        }
    }

    [ContextMenu("Clear Log")]
    public void ClearLog()
    {
        logContent = "";
        if (logText != null)
            logText.text = "";
        LogMessage("Log cleared - UnityWebRequest Demo Ready!");
    }

    [ContextMenu("Test API Connection")]
    public void TestAPIConnection()
    {
        LogMessage("🔄 Testing basic API connection...");
        
        if (APIManager.Instance == null)
        {
            LogMessage("❌ APIManager Instance is NULL");
            LogMessage("   Create a GameObject with APIManager script");
            LogMessage("   Or use LoginSystemSetup to create managers automatically");
            return;
        }
        
        LogMessage("✅ APIManager Instance found");
        LogMessage($"   API Base URL: http://localhost:5118/api");
        
        // Test with dummy credentials to check server response
        StartCoroutine(TestLoginCoroutine("dummy", "dummy"));
    }
}
