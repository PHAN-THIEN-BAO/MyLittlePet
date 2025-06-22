using TMPro;
using UnityEngine;


/// <summary>
/// This class is responsible for saving player information.
/// </summary>
public static class PlayerInfomation
{
    public static void SavePlayerInfo(User user)
    {
        // Serialize the user object to JSON and save it in PlayerPrefs
        string userJson = JsonUtility.ToJson(user);
        PlayerPrefs.SetString("SavedUser", userJson);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// Loads the player information from PlayerPrefs.
    /// </summary>
    /// <returns></returns>
    public static User LoadPlayerInfo()
    {
        if (PlayerPrefs.HasKey("SavedUser"))
        {
            string userJson = PlayerPrefs.GetString("SavedUser");
            return JsonUtility.FromJson<User>(userJson);
        }
        return null;
    }

    public static void ClearPlayerInfo()
    {
        // Clear the saved user information from PlayerPrefs
        PlayerPrefs.DeleteKey("SavedUser");
        PlayerPrefs.Save();
    }




    public static void UpdatePlayerInfo(System.Action<User> updateAction)
    {
        User user = LoadPlayerInfo();
        if (user != null && updateAction != null)
        {
            updateAction(user); // Allow the caller to modify the user object
            SavePlayerInfo(user);
        }
    }

}
