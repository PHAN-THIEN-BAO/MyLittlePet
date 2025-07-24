using UnityEngine;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public static class APIUser
{
    //public static User GetUser()
    public static User LoginAPI(string userName, string password)
    {
        //create a request to the API endpoint
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/login?UserName=" + userName + "&Password=" + password);

        //get the response
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        //return the deserialized User object
        return JsonUtility.FromJson<User>(jsonResponse);
    }
    public static bool RegisterAPI(string userName, string password, string email)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "https://localhost:7035/User/register?userName=" + userName +
                "&password=" + password +
                "&email=" + email);

            request.Method = "POST";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("Registration response: " + jsonResponse);

            return success;
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.LogError("Registration error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("Registration error: " + ex.Message);
            }
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Unexpected error during registration: " + ex.Message);
            return false;
        }
    }

    public static bool RegisterAPI(string userName, string password)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "https://localhost:7035/User/register?userName=" + userName +
                "&password=" + password);

            request.Method = "POST";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("Registration response: " + jsonResponse);

            return success;
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.LogError("Registration error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("Registration error: " + ex.Message);
            }
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Unexpected error during registration: " + ex.Message);
            return false;
        }
    }

    public static List<PlayerPet> GetPlayerPets(string playerId)
    {
        string url = "https://localhost:7035/PlayerPet/Player/" + playerId;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();

        return JsonConvert.DeserializeObject<List<PlayerPet>>(jsonResponse);
    }

    public static int GetPlayerPetCount(string playerId)
    {
        List<PlayerPet> playerPets = GetPlayerPets(playerId);
        return playerPets != null ? playerPets.Count : 0;
    }


    public static Boolean UpdateUser()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User information not found.");
            return false;
        }
        Debug.Log("Updating API user EXP: " + user.exp);
        try
        {
            int expValue = user.exp.GetValueOrDefault(0);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/" + user.id +
                "?role=" + user.role +
                "&userName=" + user.userName +
                "&password=" + user.password +
                "&email=" + user.email +
                "&level=" + user.level +
                "&coin=" + user.coin +
                "&diamond=" + user.diamond +
                "&gem=" + user.gem +
                "&exp=" + expValue);

            request.Method = "PUT";
            request.ContentType = "application/json";

            string jsonData = JsonUtility.ToJson(user);
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(jsonData);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("Update response: " + jsonResponse);
            return success;
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.LogError("Update error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("Update error: " + ex.Message);
            }
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Unexpected error during update: " + ex.Message);
            return false;
        }
    }



    public static List<User> SearchUser(string searchTerm)
    {
        string url = $"https://localhost:7035/User/search?searchTerm={searchTerm}";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string jsonResponse = reader.ReadToEnd();

                var result = JsonConvert.DeserializeObject<SearchResult>(jsonResponse);

                return result?.players ?? new List<User>();
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("SearchUser error: " + ex.Message);
            return new List<User>();
        }
    }

    [System.Serializable]
    public class SearchResult
    {
        public string message;
        public List<User> players;
    }



    public static User GetUserById(int userId)
    {
        string url = $"https://localhost:7035/User/{userId}";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string jsonResponse = reader.ReadToEnd();

                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    return JsonConvert.DeserializeObject<User>(jsonResponse);
                }
                else
                {
                    Debug.LogError($"GetUserById failed with status code: {response.StatusCode}");
                    return null;
                }
            }
        }
        catch (WebException ex)
        {
            if (ex.Response is HttpWebResponse errorResponse && errorResponse.StatusCode == HttpStatusCode.NotFound)
            {
                Debug.LogWarning($"User with ID {userId} not found.");
            }
            else
            {
                Debug.LogError($"GetUserById error: {ex.Message}");
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error in GetUserById: {ex.Message}");
            return null;
        }
    }




}