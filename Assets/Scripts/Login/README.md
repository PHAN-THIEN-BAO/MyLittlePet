# MyLittlePet Login System

This login system provides comprehensive user authentication for the MyLittlePet Unity game, integrating with the backend API.

## Components

### Core Scripts

1. **UserModels.cs** - Data models for user information and API requests/responses
2. **UserSessionManager.cs** - Manages user session state and persistence
3. **APIManager.cs** - Handles HTTP communication with the backend API
4. **Login.cs** - Main login UI controller (located in Assets/Scripts/UI/)
5. **LoginManager.cs** - Scene management and login flow control
6. **LoginSystemSetup.cs** - Helper script for initial setup
7. **APITestManager.cs** - Testing utility for API connection

### Setup Instructions

#### 1. Backend API Setup
First, ensure your API is running:

```powershell
cd "d:\SWP\MyLittlePet\API\MyLittlePetAPI"
dotnet run
```

The API should be accessible at `https://localhost:7024` by default.

#### 2. Unity Setup

1. **Create Login System Managers**:
   - Create an empty GameObject in your scene
   - Name it "LoginSystemSetup"
   - Attach the `LoginSystemSetup` script
   - In the inspector, set the API Base URL (default: https://localhost:7024/api)
   - Click "Create Required Managers" in the inspector

2. **Setup Login UI**:
   - Create a Canvas if you don't have one
   - Add the following UI elements:
     - Username Input Field (TMP_InputField)
     - Password Input Field (TMP_InputField)
     - Login Button
     - Register Button (optional)
     - Error Text (TextMeshPro)
     - Status Text (TextMeshPro)
     - Loading Panel (GameObject with UI elements)

3. **Configure Login Script**:
   - Attach the `Login` script to a GameObject in your login scene
   - Assign all UI references in the inspector
   - Set your main scene name

#### 3. API Endpoints

The system uses these API endpoints:

- `POST /api/Users/login` - User authentication
- `POST /api/Users/register` - User registration
- `GET /api/Users/{id}` - Get user details

#### 4. Features

**Online Authentication**:
- Login with username/email and password
- User registration with validation
- Session persistence across app restarts
- Automatic token management

**Offline Mode**:
- Fallback when API is unavailable
- Configurable offline credentials
- Local session storage

**Security Features**:
- Password validation
- Email format validation
- Token-based authentication
- Secure session management

#### 5. Testing

Use the `APITestManager` script to test your API connection:

1. Create a GameObject and attach `APITestManager`
2. Create UI elements for testing (Button, InputField, Text)
3. Test the API connection before implementing full login

#### 6. Scene Flow

```
Game Start -> LoginSystemSetup -> Check Session
    |                                   |
    v                                   v
No Session                        Has Session
    |                                   |
    v                                   v
Login Scene                       Main Scene
    |
    v
Login Success -> Save Session -> Main Scene
```

### Usage Examples

#### Basic Login Flow
```csharp
// The Login script handles this automatically
private void LoginUser(string username, string password)
{
    APIManager.Instance.Login(username, password, OnLoginComplete);
}

private void OnLoginComplete(LoginResponse response)
{
    if (response.Success)
    {
        UserSessionManager.Instance.SetUserSession(response.User, response.Token);
        // Navigate to main scene
    }
}
```

#### Check Login Status
```csharp
if (UserSessionManager.Instance.IsLoggedIn)
{
    UserData currentUser = UserSessionManager.Instance.CurrentUser;
    Debug.Log($"Welcome back, {currentUser.UserName}!");
}
```

#### Logout
```csharp
UserSessionManager.Instance.ClearUserSession();
SceneManager.LoadScene("LoginScene");
```

### Configuration

#### API Manager Settings
- Base URL: Set your API server address
- Timeout: Request timeout in seconds
- Enable logging for debugging

#### Login Script Settings
- Main Scene Name: Scene to load after successful login
- Offline Mode: Enable/disable offline authentication
- Offline Credentials: Username/password for offline mode

### Troubleshooting

#### Common Issues

1. **"UserSessionManager does not exist"**
   - Ensure LoginSystemSetup has created the managers
   - Check that UserSessionManager script is properly compiled

2. **"Cannot connect to API"**
   - Verify API is running on correct port
   - Check firewall settings
   - Ensure correct API URL in APIManager

3. **Login fails with valid credentials**
   - Check API logs for errors
   - Verify database connection
   - Ensure User table has test data

4. **Session not persisting**
   - Check PlayerPrefs permissions
   - Verify UserSessionManager is marked as DontDestroyOnLoad

#### Debug Tips

1. Enable Unity Console logs
2. Use APITestManager to verify connection
3. Check Unity PlayerPrefs with external tools
4. Monitor API logs during authentication

### Security Notes

- Passwords are currently stored in plain text (for development)
- In production, implement proper password hashing
- Consider implementing JWT tokens for better security
- Add rate limiting to prevent brute force attacks

### Next Steps

1. Implement password hashing in the API
2. Add JWT token authentication
3. Implement password reset functionality
4. Add social login options
5. Create user profile management UI
