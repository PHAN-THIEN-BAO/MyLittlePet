using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("Scene Management")]
    [SerializeField] private string loginSceneName = "LoginScene";
    [SerializeField] private string mainSceneName = "MainScene";
    
    void Start()
    {
        // Check if user is already logged in when the game starts
        CheckLoginStatus();
    }
    
    void CheckLoginStatus()
    {
        if (UserSessionManager.Instance != null && UserSessionManager.Instance.IsLoggedIn)
        {
            Debug.Log($"User already logged in: {UserSessionManager.Instance.CurrentUser.UserName}");
            // User is already logged in, proceed to main scene
            if (SceneManager.GetActiveScene().name != mainSceneName)
            {
                SceneManager.LoadScene(mainSceneName);
            }
        }
        else
        {
            Debug.Log("No user session found, showing login screen");
            // User is not logged in, show login screen
            if (SceneManager.GetActiveScene().name != loginSceneName)
            {
                SceneManager.LoadScene(loginSceneName);
            }
        }
    }
    
    public void Logout()
    {
        if (UserSessionManager.Instance != null)
        {
            UserSessionManager.Instance.ClearUserSession();
        }
        
        // Return to login scene
        SceneManager.LoadScene(loginSceneName);
    }
    
    public void ShowUserProfile()
    {
        if (UserSessionManager.Instance != null && UserSessionManager.Instance.IsLoggedIn)
        {
            UserData user = UserSessionManager.Instance.CurrentUser;
            Debug.Log($"User Profile - Name: {user.UserName}, Level: {user.Level}, Coins: {user.Coin}");
        }
    }
}
