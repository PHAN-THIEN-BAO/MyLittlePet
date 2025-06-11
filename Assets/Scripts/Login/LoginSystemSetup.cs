using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script helps set up the login system in your Unity project.
/// Attach this to a GameObject in your scene to initialize the login system.
/// </summary>
public class LoginSystemSetup : MonoBehaviour
{    [Header("Setup Configuration")]
    [SerializeField] private bool autoCreateManagers = true;
    [SerializeField] private string apiBaseUrl = "http://localhost:5118/api";
    
    [Header("Required Managers")]
    [SerializeField] private GameObject userSessionManagerPrefab;
    [SerializeField] private GameObject apiManagerPrefab;
    
    [Header("Scene Names")]
    [SerializeField] private string loginSceneName = "LoginScene";
    [SerializeField] private string mainSceneName = "MainScene";

    void Awake()
    {
        // This runs before any other Start() methods
        SetupLoginSystem();
    }

    void SetupLoginSystem()
    {
        // Ensure UserSessionManager exists
        if (UserSessionManager.Instance == null && autoCreateManagers)
        {
            CreateUserSessionManager();
        }

        // Ensure APIManager exists
        if (APIManager.Instance == null && autoCreateManagers)
        {
            CreateAPIManager();
        }

        Debug.Log("Login System Setup Complete");
    }

    private void CreateUserSessionManager()
    {
        GameObject sessionManagerObj;
        
        if (userSessionManagerPrefab != null)
        {
            sessionManagerObj = Instantiate(userSessionManagerPrefab);
        }
        else
        {
            sessionManagerObj = new GameObject("UserSessionManager");
            sessionManagerObj.AddComponent<UserSessionManager>();
        }
        
        sessionManagerObj.name = "UserSessionManager";
        DontDestroyOnLoad(sessionManagerObj);
        
        Debug.Log("UserSessionManager created");
    }

    private void CreateAPIManager()
    {
        GameObject apiManagerObj;
        
        if (apiManagerPrefab != null)
        {
            apiManagerObj = Instantiate(apiManagerPrefab);
        }
        else
        {
            apiManagerObj = new GameObject("APIManager");
            var apiManager = apiManagerObj.AddComponent<APIManager>();
            
            // Set the base URL using reflection or a public method
            if (!string.IsNullOrEmpty(apiBaseUrl))
            {
                apiManager.SetBaseURL(apiBaseUrl);
            }
        }
        
        apiManagerObj.name = "APIManager";
        DontDestroyOnLoad(apiManagerObj);
        
        Debug.Log("APIManager created");
    }

    [ContextMenu("Test Login System")]
    public void TestLoginSystem()
    {
        Debug.Log("=== Login System Test ===");
        
        // Check if UserSessionManager exists
        if (UserSessionManager.Instance != null)
        {
            Debug.Log("✓ UserSessionManager is present");
            Debug.Log($"  - Is Logged In: {UserSessionManager.Instance.IsLoggedIn}");
            
            if (UserSessionManager.Instance.IsLoggedIn)
            {
                var user = UserSessionManager.Instance.CurrentUser;
                Debug.Log($"  - Current User: {user.UserName} (Level {user.Level})");
            }
        }
        else
        {
            Debug.LogError("✗ UserSessionManager is missing");
        }
        
        // Check if APIManager exists
        if (APIManager.Instance != null)
        {
            Debug.Log("✓ APIManager is present");
        }
        else
        {
            Debug.LogError("✗ APIManager is missing");
        }
        
        Debug.Log("=== End Test ===");
    }

    // Called from Unity Inspector
    public void CreateRequiredManagers()
    {
        SetupLoginSystem();
    }

    // Helper method to navigate to login scene
    public void GoToLoginScene()
    {
        if (!string.IsNullOrEmpty(loginSceneName))
        {
            SceneManager.LoadScene(loginSceneName);
        }
        else
        {
            Debug.LogWarning("Login scene name is not set");
        }
    }

    // Helper method to navigate to main scene
    public void GoToMainScene()
    {
        if (!string.IsNullOrEmpty(mainSceneName))
        {
            SceneManager.LoadScene(mainSceneName);
        }
        else
        {
            Debug.LogWarning("Main scene name is not set");
        }
    }
}
