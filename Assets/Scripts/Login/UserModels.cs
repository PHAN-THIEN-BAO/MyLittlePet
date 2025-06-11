using System;

[System.Serializable]
public class UserData
{
    public int ID;
    public string Role;
    public string UserName;
    public string Email;
    public int Level;
    public int? Coin;
    public int Diamond;
    public int Gem;
    public DateTime JoinDate;
}

[System.Serializable]
public class LoginRequest
{
    public string Username;
    public string Password;
}

[System.Serializable]
public class LoginResponse
{
    public bool Success;
    public string Message;
    public UserData User;
    public string Token;
}

[System.Serializable]
public class RegisterRequest
{
    public string Role;
    public string UserName;
    public string Email;
    public string Password;
}
