using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour
{
    // Prefabs for required managers
    [SerializeField] private GameObject apiManagerPrefab;
    [SerializeField] private GameObject userSessionManagerPrefab;
    
    // API Configuration
    [SerializeField] private string apiBaseUrl = "http://localhost:5187/api";
    [SerializeField] private bool useLocalAPI = true;
    [SerializeField] private string remoteApiUrl = "https://your-deployed-api.com/api";

    private void Awake()
    {
        // Make sure we have an APIManager
        if (APIManager.Instance == null && apiManagerPrefab != null)
        {
            GameObject apiManagerObj = Instantiate(apiManagerPrefab);
            apiManagerObj.name = "APIManager";
            
            // Configure the API URL
            APIManager apiManager = apiManagerObj.GetComponent<APIManager>();
            if (apiManager != null)
            {
                apiManager.SetBaseURL(useLocalAPI ? apiBaseUrl : remoteApiUrl);
            }
        }

        // Make sure we have a UserSessionManager
        if (UserSessionManager.Instance == null && userSessionManagerPrefab != null)
        {
            GameObject userSessionObj = Instantiate(userSessionManagerPrefab);
            userSessionObj.name = "UserSessionManager";
        }
    }
}
