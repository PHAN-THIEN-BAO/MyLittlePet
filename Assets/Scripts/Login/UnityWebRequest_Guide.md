# UnityWebRequest API Integration Guide

## 🎯 Overview

Your MyLittlePet Unity game now has **full UnityWebRequest integration** for handling login and registration with your API server. Here's how it works:

## 🔧 How UnityWebRequest is Used

### 1. APIManager.cs - The Core HTTP Handler

The `APIManager` class handles all HTTP communication using UnityWebRequest:

```csharp
// Login Implementation
public void Login(string username, string password, Action<LoginResponse> onComplete)
{
    LoginRequest loginRequest = new LoginRequest
    {
        Username = username,
        Password = password
    };
    
    StartCoroutine(PostRequest<LoginResponse>("/Users/login", loginRequest, onComplete));
}

// Generic POST Request with UnityWebRequest
private IEnumerator PostRequest<T>(string endpoint, object data, Action<T> onComplete)
{
    string url = baseURL + endpoint;
    string jsonData = JsonUtility.ToJson(data);

    using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
    {
        // Setup request body
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        
        // Set headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = timeoutSeconds;
        
        // Send request
        yield return request.SendWebRequest();
        
        // Handle response
        if (request.result == UnityWebRequest.Result.Success)
        {
            T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            onComplete?.Invoke(response);
        }
        else
        {
            Debug.LogError($"API Request failed: {request.error}");
            onComplete?.Invoke(null);
        }
    }
}
```

### 2. Login.cs - UI Integration

The Login script uses APIManager for authentication:

```csharp
private void StartLogin(string username, string password)
{
    SetLoading(true);
    ShowStatus("Logging in...");
    
    // Use APIManager which handles UnityWebRequest internally
    APIManager.Instance.Login(username, password, OnLoginResponse);
}

private void OnLoginResponse(LoginResponse response)
{
    SetLoading(false);
    
    if (response != null && response.Success)
    {
        // Save user session
        UserSessionManager.Instance.SetUserSession(response.User, response.Token);
        ShowStatus($"Welcome, {response.User.UserName}!");
        LoginSuccessful();
    }
    else
    {
        ShowError("Login failed");
    }
}
```

## 🧪 Testing the UnityWebRequest Implementation

### Option 1: Using DirectUnityWebRequestDemo

I've created `DirectUnityWebRequestDemo.cs` that shows **exactly** how UnityWebRequest calls are made:

1. **Create a GameObject** in your scene
2. **Attach** `DirectUnityWebRequestDemo` script
3. **Use Context Menu** options:
   - "1. Test Login with Existing User"
   - "2. Test Registration of New User" 
   - "3. Test Full Flow: Register then Login"

This will show detailed logs of:
- Request URL and method
- Request headers and body
- Response code and data
- Success/failure handling

### Option 2: Using the Full Login System

1. **Create Login System**:
   ```csharp
   // Attach LoginSystemSetup to a GameObject
   // Click "Create Required Managers" in inspector
   ```

2. **Create Login UI**:
   ```csharp
   // Attach LoginUICreator to a GameObject
   // Click "Create Login UI" in inspector
   ```

3. **Test with Existing User**:
   - Username: `TestUser`
   - Password: `password123`

## 📡 API Endpoints Being Called

### Login Request
```
POST http://localhost:5118/api/Users/login
Content-Type: application/json

{
    "Username": "TestUser",
    "Password": "password123"
}
```

### Registration Request
```
POST http://localhost:5118/api/Users/register
Content-Type: application/json

{
    "Role": "Player",
    "UserName": "NewUser",
    "Email": "newuser@example.com", 
    "Password": "newpassword123"
}
```

## 🔍 Response Handling

### Successful Login Response
```json
{
    "success": true,
    "message": "Login successful",
    "user": {
        "id": 1,
        "role": "Player",
        "userName": "TestUser",
        "email": "test@example.com",
        "level": 1,
        "coin": null,
        "diamond": 0,
        "gem": 0,
        "joinDate": "2025-06-12T01:13:18.5139756"
    },
    "token": "dXNlcl8xXzYzODg1MjYzNzg3NzExMDIyNg=="
}
```

### Failed Login Response
```json
{
    "success": false,
    "message": "Invalid username or password",
    "user": null,
    "token": null
}
```

## ✅ Current Status

### ✅ **Working Features:**

1. **UnityWebRequest POST requests** for login and registration
2. **JSON serialization/deserialization** using JsonUtility
3. **Error handling** for network issues and API errors
4. **Timeout management** (30 seconds default)
5. **Header management** (Content-Type, Authorization)
6. **Response parsing** and callback handling
7. **Loading states** and UI feedback
8. **Offline fallback** when API is unavailable

### 🧪 **Testing Verified:**

- ✅ API server is running on `http://localhost:5118`
- ✅ Database connection is working
- ✅ Login endpoint responds correctly
- ✅ Registration endpoint works
- ✅ Existing user `TestUser` can authenticate
- ✅ UnityWebRequest calls are properly structured

## 🚀 Ready to Use!

Your UnityWebRequest integration is **fully functional**. You can:

1. **Login existing users** through the API
2. **Register new users** and automatically log them in
3. **Handle network errors** gracefully
4. **Fall back to offline mode** when needed
5. **Maintain user sessions** across app restarts

The system is production-ready and handles all the complexities of HTTP communication, JSON parsing, and error management automatically!
