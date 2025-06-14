# MyLittlePet Authentication System

This document explains how to set up and use the authentication system with the MyLittlePetAPI.

## Setup Instructions

### 1. Create Required Manager GameObjects

You'll need to set up the following prefabs:

1. **APIManager Prefab**:
   - Create an empty GameObject
   - Add the `APIManager.cs` script to it
   - Save it as a prefab in your project

2. **UserSessionManager Prefab**:
   - Create an empty GameObject
   - Add the `UserSessionManager.cs` script to it
   - Save it as a prefab in your project

### 2. Set Up Authentication Manager

1. Add the `AuthenticationManager.cs` script to a GameObject in your login scene
2. Assign the prefabs you created to the proper fields in the inspector
3. Configure the API URL:
   - Local API: `http://localhost:5187/api`
   - Remote API: Your deployed API URL

### 3. Update Your Login UI

Make sure your Login UI has all the necessary fields:
- Username and Password fields
- Login and Register buttons
- Error and Status text fields
- Registration panel with fields for:
  - Username
  - Email
  - Password
  - Confirm Password

## How It Works

1. **API Communication**:
   - `APIManager.cs` handles all communication with the MyLittlePetAPI
   - It provides methods for login, registration, and other API calls
   - It uses Unity's WebRequest system for HTTP communication

2. **Session Management**:
   - `UserSessionManager.cs` stores the current user's session
   - It maintains the authentication token for API calls
   - It can save/restore sessions between app launches

3. **Login Process**:
   - User enters credentials
   - `Login.cs` validates input
   - `APIManager` sends credentials to the API
   - On success, `UserSessionManager` saves the session
   - User is directed to the main game scene

## API Endpoints

The system uses these endpoints:

- **Login**: `POST /api/Auth/login`
  - Request: `{ email, password }`
  - Response: `{ token, user }`

- **Register**: `POST /api/Auth/register`
  - Request: `{ userName, email, password, role }`
  - Response: `{ token, user }`

- **Get Current User**: `GET /api/Auth/me`
  - Requires authentication token
  - Response: User data

## Troubleshooting

1. **Cannot Connect to API**:
   - Ensure the API is running
   - Check that the URL is correct in AuthenticationManager
   - If using a remote API, ensure CORS is properly configured

2. **Login Errors**:
   - Check the credentials being sent
   - Look for error messages in the Unity console
   - Verify the API response in the Network tab of browser dev tools

3. **Token Expiration**:
   - If you get authentication errors after some time, the token may have expired
   - Call `UserSessionManager.Instance.ClearSession()` to log out
   - Redirect the user to the login screen

## Using with Cloud-Hosted API

To use this system with a cloud-hosted API:

1. Update the `remoteApiUrl` in `AuthenticationManager`
2. Set `useLocalAPI` to false
3. Ensure your cloud API has proper CORS configuration
4. Test thoroughly with the cloud API

## Offline Mode

The system supports an offline mode when the API is unavailable:

1. Enable `enableOfflineMode` in the Login script
2. Configure offline credentials (username/password)
3. Users can login with these credentials when the API is not available
