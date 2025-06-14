using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    private static APIManager _instance;
    public static APIManager Instance { get { return _instance; } }

    [Header("API Settings")]
    [SerializeField] private string baseUrl = "http://localhost:5187/api";
    [SerializeField] private float timeoutSeconds = 10f;
    [SerializeField] private bool logApiCalls = true;

    private string authToken = string.Empty;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetBaseURL(string url)
    {
        baseUrl = url;
        Debug.Log($"API base URL set to: {baseUrl}");
    }

    public string GetBaseURL()
    {
        return baseUrl;
    }

    public void SetAuthToken(string token)
    {
        authToken = token;
    }

    public string GetAuthToken()
    {
        return authToken;
    }

    public void ClearAuthToken()
    {
        authToken = string.Empty;
    }

    // Login method that matches your existing Login.cs implementation
    public void Login(string username, string password, Action<LoginResponse> callback)
    {
        // Create login data
        LoginRequest loginData = new LoginRequest
        {
            Email = username, // Note: API uses email for login, not username
            Password = password
        };

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Start coroutine for API call
        StartCoroutine(MakePostRequest("Auth/login", jsonData, (success, response) =>
        {
            if (success)
            {
                try
                {
                    // Parse response as AuthResponseDTO
                    AuthResponseDTO authResponse = JsonUtility.FromJson<AuthResponseDTO>(response);

                    // Create LoginResponse object
                    LoginResponse loginResponse = new LoginResponse
                    {
                        Success = true,
                        User = authResponse.User,
                        Token = authResponse.Token,
                        Message = "Login successful"
                    };

                    // Store the token for future requests
                    SetAuthToken(authResponse.Token);

                    // Call the callback
                    callback?.Invoke(loginResponse);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing login response: {ex.Message}");
                    callback?.Invoke(new LoginResponse
                    {
                        Success = false,
                        Message = "Error processing server response"
                    });
                }
            }
            else
            {
                // Parse error message if possible
                string errorMessage = "Unable to connect to server";
                
                try
                {
                    // Try to get a more specific error message from the response
                    if (!string.IsNullOrEmpty(response))
                    {
                        // Sometimes errors are returned as plain text
                        if (!response.StartsWith("{"))
                        {
                            errorMessage = response;
                        }
                        else
                        {
                            // Try to parse as JSON error
                            ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(response);
                            if (!string.IsNullOrEmpty(errorResponse.message))
                            {
                                errorMessage = errorResponse.message;
                            }
                        }
                    }
                }
                catch
                {
                    // If parsing fails, use generic message
                    errorMessage = "Login failed. Please check your credentials.";
                }

                callback?.Invoke(new LoginResponse
                {
                    Success = false,
                    Message = errorMessage
                });
            }
        }));
    }

    // Register method that matches your existing Login.cs implementation
    public void Register(RegisterRequest registerRequest, Action<LoginResponse> callback)
    {
        // Convert to JSON
        string jsonData = JsonUtility.ToJson(registerRequest);

        // Start coroutine for API call
        StartCoroutine(MakePostRequest("Auth/register", jsonData, (success, response) =>
        {
            if (success)
            {
                try
                {
                    // Parse response as AuthResponseDTO
                    AuthResponseDTO authResponse = JsonUtility.FromJson<AuthResponseDTO>(response);

                    // Create LoginResponse object (reusing it for registration response)
                    LoginResponse loginResponse = new LoginResponse
                    {
                        Success = true,
                        User = authResponse.User,
                        Token = authResponse.Token,
                        Message = "Registration successful"
                    };

                    // Store the token for future requests
                    SetAuthToken(authResponse.Token);

                    // Call the callback
                    callback?.Invoke(loginResponse);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error parsing register response: {ex.Message}");
                    callback?.Invoke(new LoginResponse
                    {
                        Success = false,
                        Message = "Error processing server response"
                    });
                }
            }
            else
            {
                // Parse error message if possible
                string errorMessage = "Unable to register. Please try again.";
                
                try
                {
                    // Try to get a more specific error message from the response
                    if (!string.IsNullOrEmpty(response))
                    {
                        // Sometimes errors are returned as plain text
                        if (!response.StartsWith("{"))
                        {
                            errorMessage = response;
                        }
                        else
                        {
                            // Try to parse as JSON error
                            ErrorResponse errorResponse = JsonUtility.FromJson<ErrorResponse>(response);
                            if (!string.IsNullOrEmpty(errorResponse.message))
                            {
                                errorMessage = errorResponse.message;
                            }
                        }
                    }
                }
                catch
                {
                    // If parsing fails, use generic message
                    errorMessage = "Registration failed. Please try again.";
                }

                callback?.Invoke(new LoginResponse
                {
                    Success = false,
                    Message = errorMessage
                });
            }
        }));
    }

    // Generic method to make a GET request
    public IEnumerator MakeGetRequest(string endpoint, Action<bool, string> callback)
    {
        string url = $"{baseUrl}/{endpoint}";
        
        if (logApiCalls)
            Debug.Log($"GET Request: {url}");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.timeout = Mathf.RoundToInt(timeoutSeconds);
            
            // Add auth token if available
            if (!string.IsNullOrEmpty(authToken))
            {
                www.SetRequestHeader("Authorization", $"Bearer {authToken}");
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                callback?.Invoke(true, response);
            }
            else
            {
                Debug.LogWarning($"API Error ({www.responseCode}): {www.error}");
                callback?.Invoke(false, www.downloadHandler.text);
            }
        }
    }

    // Generic method to make a POST request
    public IEnumerator MakePostRequest(string endpoint, string jsonData, Action<bool, string> callback)
    {
        string url = $"{baseUrl}/{endpoint}";
        
        if (logApiCalls)
            Debug.Log($"POST Request: {url}\nData: {jsonData}");

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.timeout = Mathf.RoundToInt(timeoutSeconds);
            
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // Add auth token if available
            if (!string.IsNullOrEmpty(authToken))
            {
                www.SetRequestHeader("Authorization", $"Bearer {authToken}");
            }

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string response = www.downloadHandler.text;
                callback?.Invoke(true, response);
            }
            else
            {
                Debug.LogWarning($"API Error ({www.responseCode}): {www.error}");
                callback?.Invoke(false, www.downloadHandler.text);
            }
        }
    }
}

// API Models that match the API's DTOs

[Serializable]
public class LoginRequest
{
    public string Email;
    public string Password;
}

[Serializable]
public class RegisterRequest
{
    public string UserName;
    public string Email;
    public string Password;
    public string Role = "Player";
}

[Serializable]
public class AuthResponseDTO
{
    public string Token;
    public UserData User;
}

[Serializable]
public class ErrorResponse
{
    public string message;
}

[Serializable]
public class LoginResponse
{
    public bool Success;
    public UserData User;
    public string Token;
    public string Message;
}

[Serializable]
public class UserData
{
    public int ID;
    public string Role;
    public string UserName;
    public string Email;
    public int Level;
    public int Coin;
    public int Diamond;
    public int Gem;
    public string JoinDate;
}
