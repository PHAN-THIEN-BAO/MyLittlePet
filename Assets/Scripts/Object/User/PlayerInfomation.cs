using Newtonsoft.Json;
using TMPro;
using UnityEngine;


public static class PlayerInfomation
{
    public static void SavePlayerInfo(User user)
    {
        if (user == null)
        {
            Debug.LogError("User can not be null");
            return;
        }

        Debug.Log("Saving player information - exp: " + (user.exp.HasValue ? user.exp.ToString() : "null"));

        string userJson = JsonConvert.SerializeObject(user);
        PlayerPrefs.SetString("SavedUser", userJson);
        PlayerPrefs.Save();

        Debug.Log("SavePlayerInfo - JSON created: " + userJson);
    }

    public static User LoadPlayerInfo()
    {
        if (PlayerPrefs.HasKey("SavedUser"))
        {
            string userJson = PlayerPrefs.GetString("SavedUser");

            User user = JsonConvert.DeserializeObject<User>(userJson);

            Debug.Log("LoadPlayerInfo - Loaded exp value: " + (user.exp.HasValue ? user.exp.ToString() : "null"));
            return user;
        }
        return null;
    }

    






    public static void ClearPlayerInfo()
    {
        PlayerPrefs.DeleteKey("SavedUser");
        PlayerPrefs.Save();
    }




    public static void UpdatePlayerInfo(System.Action<User> updateAction)
    {
        User user = LoadPlayerInfo();
        if (user != null && updateAction != null)
        {
            updateAction(user);
            SavePlayerInfo(user);
        }
    }

}