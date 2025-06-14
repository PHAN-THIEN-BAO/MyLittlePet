using UnityEngine;

public class UserSessionManager : MonoBehaviour
{
    private static UserSessionManager _instance;
    public static UserSessionManager Instance { get { return _instance; } }

    // Current user session
    public UserData CurrentUser { get; private set; }
    public string CurrentToken { get; private set; }
    public bool IsLoggedIn { get { return CurrentUser != null && !string.IsNullOrEmpty(CurrentToken); } }

    // Player preferences keys
    private const string PREF_USER_TOKEN = "user_token";
    private const string PREF_USER_DATA = "user_data";

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Try to restore session from PlayerPrefs
        RestoreSession();
    }

    // Set the current user session
    public void SetUserSession(UserData userData, string token)
    {
        CurrentUser = userData;
        CurrentToken = token;

        // Update the token in APIManager
        if (APIManager.Instance != null)
        {
            APIManager.Instance.SetAuthToken(token);
        }

        // Save to PlayerPrefs
        SaveSession();

        Debug.Log($"User session set: {userData.UserName} (ID: {userData.ID})");
    }

    // Clear the current user session (logout)
    public void ClearSession()
    {
        CurrentUser = null;
        CurrentToken = null;

        // Clear token in APIManager
        if (APIManager.Instance != null)
        {
            APIManager.Instance.ClearAuthToken();
        }

        // Clear saved session
        PlayerPrefs.DeleteKey(PREF_USER_TOKEN);
        PlayerPrefs.DeleteKey(PREF_USER_DATA);
        PlayerPrefs.Save();

        Debug.Log("User session cleared");
    }

    // Save the current session to PlayerPrefs
    private void SaveSession()
    {
        if (CurrentUser != null && !string.IsNullOrEmpty(CurrentToken))
        {
            PlayerPrefs.SetString(PREF_USER_TOKEN, CurrentToken);
            
            // Serialize user data to JSON
            string userJson = JsonUtility.ToJson(CurrentUser);
            PlayerPrefs.SetString(PREF_USER_DATA, userJson);
            
            PlayerPrefs.Save();
        }
    }

    // Restore session from PlayerPrefs
    private void RestoreSession()
    {
        string savedToken = PlayerPrefs.GetString(PREF_USER_TOKEN, "");
        string savedUserJson = PlayerPrefs.GetString(PREF_USER_DATA, "");

        if (!string.IsNullOrEmpty(savedToken) && !string.IsNullOrEmpty(savedUserJson))
        {
            try
            {
                // Deserialize user data
                UserData userData = JsonUtility.FromJson<UserData>(savedUserJson);
                
                // Set session
                CurrentUser = userData;
                CurrentToken = savedToken;

                // Update APIManager
                if (APIManager.Instance != null)
                {
                    APIManager.Instance.SetAuthToken(savedToken);
                }

                Debug.Log($"Session restored: {userData.UserName} (ID: {userData.ID})");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error restoring session: {ex.Message}");
                
                // Clear invalid session data
                ClearSession();
            }
        }
    }

    // Validate token with the server
    public void ValidateSession(System.Action<bool> callback)
    {
        if (!IsLoggedIn)
        {
            callback?.Invoke(false);
            return;
        }

        if (APIManager.Instance == null)
        {
            Debug.LogWarning("APIManager not available for session validation");
            callback?.Invoke(true); // Assume valid in offline mode
            return;
        }

        // Call the API to validate the token
        StartCoroutine(APIManager.Instance.MakeGetRequest("Auth/me", (success, response) =>
        {
            if (success)
            {
                Debug.Log("Session validated with server");
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogWarning("Session validation failed - clearing session");
                ClearSession();
                callback?.Invoke(false);
            }
        }));
    }
}
