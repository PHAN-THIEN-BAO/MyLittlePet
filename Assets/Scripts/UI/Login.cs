using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public TMP_InputField usernameField;
    [SerializeField] public TMP_InputField passwordField;
    [SerializeField] public Button loginButton;
    [SerializeField] public TextMeshProUGUI errorText;
    
    [Header("Settings")]
    [SerializeField] public string mainSceneName = "MainScene";
    [SerializeField] public string correctUsername = "admin";
    [SerializeField] public string correctPassword = "password";

    void Start()
    {
        // Clear any error messages
        if (errorText != null)
            errorText.text = "";
            
        // Add listener to login button
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    public void OnLoginButtonClick()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        
        // Simple validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please enter both username and password.");
            return;
        }
        
        // Check credentials (replace with your actual authentication logic)
        if (username == correctUsername && password == correctPassword)
        {
            // Login successful
            LoginSuccessful();
        }
        else
        {
            // Login failed
            ShowError("Invalid username or password.");
        }
    }
    
    private void ShowError(string message)
    {
        if (errorText != null)
            errorText.text = message;
    }
    
    private void LoginSuccessful()
    {
        // You might want to save login state or user data here
        Debug.Log("Login successful! Navigating to main scene.");
        
        // Load the main scene
        SceneManager.LoadScene(mainSceneName);
    }
}
