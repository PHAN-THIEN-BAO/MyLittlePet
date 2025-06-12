using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Login : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public TMP_InputField usernameField;
    [SerializeField] public TMP_InputField passwordField;
    [SerializeField] public Button loginButton;
    [SerializeField] public Button registerButton;
    [SerializeField] public TextMeshProUGUI errorText;
    [SerializeField] public TextMeshProUGUI statusText;
    [SerializeField] public GameObject loadingPanel;
    
    [Header("Settings")]
    [SerializeField] public string mainSceneName = "MainScene";
    [SerializeField] public bool enableOfflineMode = true;
    [SerializeField] public string offlineUsername = "admin";
    [SerializeField] public string offlinePassword = "password";

    [Header("Registration Fields")]
    [SerializeField] public GameObject registrationPanel;
    [SerializeField] public TMP_InputField regUsernameField;
    [SerializeField] public TMP_InputField regEmailField;
    [SerializeField] public TMP_InputField regPasswordField;
    [SerializeField] public TMP_InputField regConfirmPasswordField;
    [SerializeField] public Button submitRegisterButton;
    [SerializeField] public Button cancelRegisterButton;

    private bool isLoading = false;

    void Start()
    {
        InitializeUI();
        SetupEventListeners();
        
        // Check if user is already logged in
        if (UserSessionManager.Instance != null && UserSessionManager.Instance.IsLoggedIn)
        {
            ShowStatus($"Welcome back, {UserSessionManager.Instance.CurrentUser.UserName}!");
            StartCoroutine(DelayedSceneLoad());
        }
    }

    private void InitializeUI()
    {
        // Clear any messages
        if (errorText != null) errorText.text = "";
        if (statusText != null) statusText.text = "";
        
        // Hide loading panel
        if (loadingPanel != null) loadingPanel.SetActive(false);
        
        // Hide registration panel
        if (registrationPanel != null) registrationPanel.SetActive(false);
    }

    private void SetupEventListeners()
    {
        // Login button
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginButtonClick);
            
        // Register button
        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterButtonClick);
            
        // Registration form buttons
        if (submitRegisterButton != null)
            submitRegisterButton.onClick.AddListener(OnSubmitRegisterClick);
            
        if (cancelRegisterButton != null)
            cancelRegisterButton.onClick.AddListener(OnCancelRegisterClick);
    }

    public void OnLoginButtonClick()
    {
        if (isLoading) return;

        string username = usernameField.text.Trim();
        string password = passwordField.text;
        
        // Input validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please enter both username and password.");
            return;
        }

        StartLogin(username, password);
    }

    public void OnRegisterButtonClick()
    {
        if (registrationPanel != null)
        {
            registrationPanel.SetActive(true);
            ClearMessages();
        }
    }

    public void OnSubmitRegisterClick()
    {
        if (isLoading) return;

        string username = regUsernameField.text.Trim();
        string email = regEmailField.text.Trim();
        string password = regPasswordField.text;
        string confirmPassword = regConfirmPasswordField.text;

        // Validation
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowError("Please fill in all fields.");
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Passwords do not match.");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters long.");
            return;
        }

        if (!IsValidEmail(email))
        {
            ShowError("Please enter a valid email address.");
            return;
        }

        StartRegistration(username, email, password);
    }

    public void OnCancelRegisterClick()
    {
        if (registrationPanel != null)
        {
            registrationPanel.SetActive(false);
            ClearRegistrationFields();
            ClearMessages();
        }
    }

    private void StartLogin(string username, string password)
    {
        SetLoading(true);
        ShowStatus("Logging in...");

        // Check if API Manager is available
        if (APIManager.Instance == null)
        {
            Debug.LogWarning("APIManager not found. Using offline mode.");
            HandleOfflineLogin(username, password);
            return;
        }

        // Attempt API login
        APIManager.Instance.Login(username, password, OnLoginResponse);
    }

    private void StartRegistration(string username, string email, string password)
    {
        SetLoading(true);
        ShowStatus("Creating account...");

        if (APIManager.Instance == null)
        {
            SetLoading(false);
            ShowError("Registration requires an internet connection.");
            return;
        }

        RegisterRequest registerRequest = new RegisterRequest
        {
            UserName = username,
            Email = email,
            Password = password,
            Role = "Player" // Default role
        };

        APIManager.Instance.Register(registerRequest, OnRegisterResponse);
    }

    private void OnLoginResponse(LoginResponse response)
    {
        SetLoading(false);

        if (response == null)
        {
            ShowError("Unable to connect to server. Check your internet connection.");
            
            // Try offline mode if enabled
            if (enableOfflineMode)
            {
                ShowStatus("Attempting offline login...");
                string username = usernameField.text.Trim();
                string password = passwordField.text;
                HandleOfflineLogin(username, password);
            }
            return;
        }

        if (response.Success)
        {
            // Save user session
            if (UserSessionManager.Instance != null)
            {
                UserSessionManager.Instance.SetUserSession(response.User, response.Token);
            }

            ShowStatus($"Welcome, {response.User.UserName}!");
            LoginSuccessful();
        }
        else
        {
            ShowError(response.Message);
        }
    }

    private void OnRegisterResponse(LoginResponse response)
    {
        SetLoading(false);

        if (response == null)
        {
            ShowError("Unable to connect to server. Please try again later.");
            return;
        }

        if (response.Success)
        {
            // Save user session
            if (UserSessionManager.Instance != null)
            {
                UserSessionManager.Instance.SetUserSession(response.User, response.Token);
            }

            ShowStatus($"Account created successfully! Welcome, {response.User.UserName}!");
            
            // Close registration panel
            if (registrationPanel != null)
                registrationPanel.SetActive(false);
                
            LoginSuccessful();
        }
        else
        {
            ShowError(response.Message);
        }
    }

    private void HandleOfflineLogin(string username, string password)
    {
        if (!enableOfflineMode)
        {
            ShowError("Offline mode is disabled.");
            return;
        }

        if (username == offlineUsername && password == offlinePassword)
        {
            // Create offline user data
            UserData offlineUser = new UserData
            {
                ID = -1,
                UserName = username,
                Email = "offline@local.com",
                Role = "Player",
                Level = 1,
                Coin = 1000,
                Diamond = 0,
                Gem = 0,
                JoinDate = System.DateTime.Now
            };

            if (UserSessionManager.Instance != null)
            {
                UserSessionManager.Instance.SetUserSession(offlineUser, "offline_token");
            }

            ShowStatus("Logged in offline mode");
            LoginSuccessful();
        }
        else
        {
            ShowError("Invalid credentials for offline mode.");
        }
    }

    private void LoginSuccessful()
    {
        Debug.Log("Login successful! Navigating to main scene.");
        StartCoroutine(DelayedSceneLoad());
    }

    private IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(mainSceneName);
    }

    private void SetLoading(bool loading)
    {
        isLoading = loading;
        
        if (loadingPanel != null)
            loadingPanel.SetActive(loading);
            
        if (loginButton != null)
            loginButton.interactable = !loading;
            
        if (registerButton != null)
            registerButton.interactable = !loading;
    }

    private void ShowError(string message)
    {
        if (errorText != null)
        {
            errorText.text = message;
            errorText.color = Color.red;
        }
        
        if (statusText != null)
            statusText.text = "";
            
        Debug.LogWarning($"Login Error: {message}");
    }
    
    private void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = Color.green;
        }
        
        if (errorText != null)
            errorText.text = "";
            
        Debug.Log($"Login Status: {message}");
    }

    private void ClearMessages()
    {
        if (errorText != null) errorText.text = "";
        if (statusText != null) statusText.text = "";
    }

    private void ClearRegistrationFields()
    {
        if (regUsernameField != null) regUsernameField.text = "";
        if (regEmailField != null) regEmailField.text = "";
        if (regPasswordField != null) regPasswordField.text = "";
        if (regConfirmPasswordField != null) regConfirmPasswordField.text = "";
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
