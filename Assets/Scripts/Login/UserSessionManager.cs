using UnityEngine;

public class UserSessionManager : MonoBehaviour
{
    public static UserSessionManager Instance { get; private set; }
    
    [Header("User Session Data")]
    public UserData CurrentUser { get; private set; }
    public string AuthToken { get; private set; }
    public bool IsLoggedIn { get; private set; }
    
    [Header("Events")]
    public System.Action<UserData> OnUserLoggedIn;
    public System.Action OnUserLoggedOut;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUserSession();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUserSession(UserData user, string token)
    {
        CurrentUser = user;
        AuthToken = token;
        IsLoggedIn = true;
        
        // Save to PlayerPrefs for persistence
        SaveUserSession();
        
        // Notify listeners
        OnUserLoggedIn?.Invoke(user);
        
        Debug.Log($"User session set for: {user.UserName}");
    }

    public void ClearUserSession()
    {
        CurrentUser = null;
        AuthToken = null;
        IsLoggedIn = false;
        
        // Clear from PlayerPrefs
        PlayerPrefs.DeleteKey("UserSession");
        PlayerPrefs.DeleteKey("AuthToken");
        PlayerPrefs.Save();
        
        // Notify listeners
        OnUserLoggedOut?.Invoke();
        
        Debug.Log("User session cleared");
    }

    public void UpdateUserData(UserData updatedUser)
    {
        if (IsLoggedIn && CurrentUser != null)
        {
            CurrentUser = updatedUser;
            SaveUserSession();
        }
    }

    private void SaveUserSession()
    {
        if (CurrentUser != null && !string.IsNullOrEmpty(AuthToken))
        {
            string userJson = JsonUtility.ToJson(CurrentUser);
            PlayerPrefs.SetString("UserSession", userJson);
            PlayerPrefs.SetString("AuthToken", AuthToken);
            PlayerPrefs.Save();
        }
    }

    private void LoadUserSession()
    {
        if (PlayerPrefs.HasKey("UserSession") && PlayerPrefs.HasKey("AuthToken"))
        {
            try
            {
                string userJson = PlayerPrefs.GetString("UserSession");
                string token = PlayerPrefs.GetString("AuthToken");
                
                CurrentUser = JsonUtility.FromJson<UserData>(userJson);
                AuthToken = token;
                IsLoggedIn = true;
                
                Debug.Log($"Loaded user session for: {CurrentUser.UserName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load user session: {e.Message}");
                ClearUserSession();
            }
        }
    }

    public string GetAuthorizationHeader()
    {
        return IsLoggedIn && !string.IsNullOrEmpty(AuthToken) ? $"Bearer {AuthToken}" : "";
    }
}
