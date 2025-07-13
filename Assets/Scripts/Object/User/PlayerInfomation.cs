using Newtonsoft.Json;
using TMPro;
using UnityEngine;


/// <summary>
/// This class is responsible for saving player information.
/// </summary>
public static class PlayerInfomation
{
    public static void SavePlayerInfo(User user)
    {
        if (user == null)
        {
            Debug.LogError("User can not be null");
            return;
        }

        // Log to help debug the saving process
        Debug.Log("Saving player information - exp: " + (user.exp.HasValue ? user.exp.ToString() : "null"));

        // Serialize by using Newtonsoft.Json
        string userJson = JsonConvert.SerializeObject(user);
        PlayerPrefs.SetString("SavedUser", userJson);
        PlayerPrefs.Save();

        // check if the save was successful
        Debug.Log("SavePlayerInfo - JSON created: " + userJson);
    }

    public static User LoadPlayerInfo()
    {
        if (PlayerPrefs.HasKey("SavedUser"))
        {
            string userJson = PlayerPrefs.GetString("SavedUser");

            // Deserialize by using Newtonsoft.Json
            User user = JsonConvert.DeserializeObject<User>(userJson);

            Debug.Log("LoadPlayerInfo - Loaded exp value: " + (user.exp.HasValue ? user.exp.ToString() : "null"));
            return user;
        }
        return null;
    }

    //=====================================
    /*public static void SavePlayerInfo(User user)
    {
        Debug.Log("Saving player information...: " + user.exp);
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
    }*/
    //=====================================






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
