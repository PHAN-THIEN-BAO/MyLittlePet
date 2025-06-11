using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Direct demonstration of UnityWebRequest API calls for login and registration
/// This shows the actual HTTP requests being made to your API
/// </summary>
public class DirectUnityWebRequestDemo : MonoBehaviour
{
    [Header("API Configuration")]
    [SerializeField] private string apiBaseUrl = "http://localhost:5118/api";
    [SerializeField] private int timeoutSeconds = 30;

    [Header("Test Data")]
    [SerializeField] private string testUsername = "TestUser";
    [SerializeField] private string testPassword = "password123";
    [SerializeField] private string newUsername = "DirectTestUser";
    [SerializeField] private string newEmail = "directtest@example.com";
    [SerializeField] private string newPassword = "directpass123";

    void Start()
    {
        Debug.Log("DirectUnityWebRequestDemo Ready!");
        Debug.Log($"API Base URL: {apiBaseUrl}");
        Debug.Log("Use context menu or call methods directly to test API calls");
    }

    [ContextMenu("1. Test Login with Existing User")]
    public void TestLoginWithExistingUser()
    {
        StartCoroutine(LoginCoroutine(testUsername, testPassword));
    }

    [ContextMenu("2. Test Registration of New User")]
    public void TestRegistrationOfNewUser()
    {
        // Generate unique username to avoid conflicts
        string uniqueUsername = $"{newUsername}_{DateTime.Now.Ticks.ToString().Substring(10)}";
        string uniqueEmail = $"test{DateTime.Now.Ticks.ToString().Substring(10)}@example.com";
        
        StartCoroutine(RegisterCoroutine(uniqueUsername, uniqueEmail, newPassword));
    }

    [ContextMenu("3. Test Full Flow: Register then Login")]
    public void TestFullFlow()
    {
        StartCoroutine(FullFlowCoroutine());
    }

    /// <summary>
    /// Direct UnityWebRequest implementation for user login
    /// </summary>
    public IEnumerator LoginCoroutine(string username, string password)
    {
        Debug.Log("=== STARTING LOGIN REQUEST ===");
        
        // 1. Prepare the request data
        var loginData = new
        {
            Username = username,
            Password = password
        };
        
        string jsonData = JsonUtility.ToJson(loginData);
        Debug.Log($"📤 REQUEST URL: {apiBaseUrl}/Users/login");
        Debug.Log($"📤 REQUEST METHOD: POST");
        Debug.Log($"📤 REQUEST BODY: {jsonData}");
        
        // 2. Create UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/Users/login", "POST"))
        {
            // 3. Setup request body
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // 4. Set headers
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = timeoutSeconds;
            
            Debug.Log("📤 REQUEST HEADERS:");
            Debug.Log("   Content-Type: application/json");
            Debug.Log($"   Timeout: {timeoutSeconds} seconds");
            
            // 5. Send the request
            Debug.Log("🔄 SENDING REQUEST...");
            yield return request.SendWebRequest();
            
            // 6. Handle the response
            Debug.Log("=== RESPONSE RECEIVED ===");
            Debug.Log($"📥 RESPONSE CODE: {request.responseCode}");
            Debug.Log($"📥 RESPONSE RESULT: {request.result}");
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"📥 RESPONSE BODY: {request.downloadHandler.text}");
                
                try
                {
                    // Parse the response
                    var response = JsonUtility.FromJson<LoginResponseData>(request.downloadHandler.text);
                    
                    if (response.success)
                    {
                        Debug.Log("✅ LOGIN SUCCESS!");
                        Debug.Log($"   Welcome: {response.user.userName}");
                        Debug.Log($"   User ID: {response.user.id}");
                        Debug.Log($"   Email: {response.user.email}");
                        Debug.Log($"   Level: {response.user.level}");
                        Debug.Log($"   Coins: {response.user.coin}");
                        Debug.Log($"   Auth Token: {response.token.Substring(0, 20)}...");
                    }
                    else
                    {
                        Debug.LogWarning($"❌ LOGIN FAILED: {response.message}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ ERROR PARSING RESPONSE: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"❌ REQUEST FAILED: {request.error}");
                Debug.LogError($"❌ RESPONSE TEXT: {request.downloadHandler.text}");
            }
        }
        
        Debug.Log("=== LOGIN REQUEST COMPLETE ===\n");
    }

    /// <summary>
    /// Direct UnityWebRequest implementation for user registration
    /// </summary>
    public IEnumerator RegisterCoroutine(string username, string email, string password)
    {
        Debug.Log("=== STARTING REGISTRATION REQUEST ===");
        
        // 1. Prepare the request data
        var registerData = new
        {
            Role = "Player",
            UserName = username,
            Email = email,
            Password = password
        };
        
        string jsonData = JsonUtility.ToJson(registerData);
        Debug.Log($"📤 REQUEST URL: {apiBaseUrl}/Users/register");
        Debug.Log($"📤 REQUEST METHOD: POST");
        Debug.Log($"📤 REQUEST BODY: {jsonData}");
        
        // 2. Create UnityWebRequest
        using (UnityWebRequest request = new UnityWebRequest($"{apiBaseUrl}/Users/register", "POST"))
        {
            // 3. Setup request body
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // 4. Set headers
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = timeoutSeconds;
            
            Debug.Log("📤 REQUEST HEADERS:");
            Debug.Log("   Content-Type: application/json");
            Debug.Log($"   Timeout: {timeoutSeconds} seconds");
            
            // 5. Send the request
            Debug.Log("🔄 SENDING REQUEST...");
            yield return request.SendWebRequest();
            
            // 6. Handle the response
            Debug.Log("=== RESPONSE RECEIVED ===");
            Debug.Log($"📥 RESPONSE CODE: {request.responseCode}");
            Debug.Log($"📥 RESPONSE RESULT: {request.result}");
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"📥 RESPONSE BODY: {request.downloadHandler.text}");
                
                try
                {
                    // Parse the response
                    var response = JsonUtility.FromJson<LoginResponseData>(request.downloadHandler.text);
                    
                    if (response.success)
                    {
                        Debug.Log("✅ REGISTRATION SUCCESS!");
                        Debug.Log($"   New User: {response.user.userName}");
                        Debug.Log($"   User ID: {response.user.id}");
                        Debug.Log($"   Email: {response.user.email}");
                        Debug.Log($"   Starting Level: {response.user.level}");
                        Debug.Log($"   Starting Coins: {response.user.coin}");
                        Debug.Log($"   Auth Token: {response.token.Substring(0, 20)}...");
                    }
                    else
                    {
                        Debug.LogWarning($"❌ REGISTRATION FAILED: {response.message}");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ ERROR PARSING RESPONSE: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"❌ REQUEST FAILED: {request.error}");
                Debug.LogError($"❌ RESPONSE TEXT: {request.downloadHandler.text}");
            }
        }
        
        Debug.Log("=== REGISTRATION REQUEST COMPLETE ===\n");
    }

    /// <summary>
    /// Demonstrates complete flow: register new user, then login with that user
    /// </summary>
    private IEnumerator FullFlowCoroutine()
    {
        Debug.Log("🚀 STARTING FULL FLOW TEST: REGISTER + LOGIN");
        
        // Generate unique credentials
        string uniqueId = DateTime.Now.Ticks.ToString().Substring(10);
        string username = $"FlowTest{uniqueId}";
        string email = $"flowtest{uniqueId}@example.com";
        string password = "flowtest123";
        
        Debug.Log($"Generated test credentials: {username} / {email} / {password}");
        
        // Step 1: Register the user
        Debug.Log("\n🔄 STEP 1: REGISTERING NEW USER...");
        yield return StartCoroutine(RegisterCoroutine(username, email, password));
        
        // Wait a moment between requests
        yield return new WaitForSeconds(1f);
        
        // Step 2: Login with the new user
        Debug.Log("\n🔄 STEP 2: LOGGING IN WITH NEW USER...");
        yield return StartCoroutine(LoginCoroutine(username, password));
        
        Debug.Log("🎉 FULL FLOW TEST COMPLETE!");
    }

    [ContextMenu("Test API Server Connection")]
    public void TestAPIConnection()
    {
        StartCoroutine(TestConnectionCoroutine());
    }

    private IEnumerator TestConnectionCoroutine()
    {
        Debug.Log("🔍 TESTING API SERVER CONNECTION...");
        
        using (UnityWebRequest request = UnityWebRequest.Get($"{apiBaseUrl}/Users"))
        {
            request.timeout = 5; // Short timeout for connection test
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ API SERVER IS REACHABLE!");
                Debug.Log($"   Response Code: {request.responseCode}");
                Debug.Log($"   Server responded with user data");
            }
            else
            {
                Debug.LogError("❌ API SERVER CONNECTION FAILED!");
                Debug.LogError($"   Error: {request.error}");
                Debug.LogError($"   Make sure API server is running at: {apiBaseUrl}");
            }
        }
    }
}

/// <summary>
/// Data structure for parsing API responses
/// </summary>
[System.Serializable]
public class LoginResponseData
{
    public bool success;
    public string message;
    public UserResponseData user;
    public string token;
}

[System.Serializable]
public class UserResponseData
{
    public int id;
    public string role;
    public string userName;
    public string email;
    public string password; // Will be empty in response for security
    public int level;
    public int? coin;
    public int diamond;
    public int gem;
    public string joinDate;
}
