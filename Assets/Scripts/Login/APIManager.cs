using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager Instance { get; private set; }
      [Header("API Configuration")]
    [SerializeField] private string baseURL = "http://localhost:5118/api"; // Updated with correct API URL
    [SerializeField] private int timeoutSeconds = 30;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Login(string username, string password, Action<LoginResponse> onComplete)
    {
        LoginRequest loginRequest = new LoginRequest
        {
            Username = username,
            Password = password
        };

        StartCoroutine(PostRequest<LoginResponse>("/Users/login", loginRequest, onComplete));
    }

    public void Register(RegisterRequest registerRequest, Action<LoginResponse> onComplete)
    {
        StartCoroutine(PostRequest<LoginResponse>("/Users/register", registerRequest, onComplete));
    }

    public void GetUser(int userId, Action<UserData> onComplete)
    {
        StartCoroutine(GetRequest<UserData>($"/Users/{userId}", onComplete));
    }

    private IEnumerator PostRequest<T>(string endpoint, object data, Action<T> onComplete) where T : class
    {
        string url = baseURL + endpoint;
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = timeoutSeconds;

            // Add authorization header if user is logged in
            if (UserSessionManager.Instance != null && UserSessionManager.Instance.IsLoggedIn)
            {
                string authHeader = UserSessionManager.Instance.GetAuthorizationHeader();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    request.SetRequestHeader("Authorization", authHeader);
                }
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    T response = JsonUtility.FromJson<T>(responseText);
                    onComplete?.Invoke(response);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing response: {e.Message}");
                    onComplete?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"API Request failed: {request.error}");
                Debug.LogError($"Response: {request.downloadHandler.text}");
                onComplete?.Invoke(null);
            }
        }
    }

    private IEnumerator GetRequest<T>(string endpoint, Action<T> onComplete) where T : class
    {
        string url = baseURL + endpoint;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = timeoutSeconds;

            // Add authorization header if user is logged in  
            if (UserSessionManager.Instance != null && UserSessionManager.Instance.IsLoggedIn)
            {
                string authHeader = UserSessionManager.Instance.GetAuthorizationHeader();
                if (!string.IsNullOrEmpty(authHeader))
                {
                    request.SetRequestHeader("Authorization", authHeader);
                }
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = request.downloadHandler.text;
                    T response = JsonUtility.FromJson<T>(responseText);
                    onComplete?.Invoke(response);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error parsing response: {e.Message}");
                    onComplete?.Invoke(null);
                }
            }
            else
            {
                Debug.LogError($"API Request failed: {request.error}");
                onComplete?.Invoke(null);
            }
        }
    }

    public void SetBaseURL(string url)
    {
        baseURL = url;
    }
}
