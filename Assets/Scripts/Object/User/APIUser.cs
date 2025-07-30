using UnityEngine;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

public static class APIUser
{
    //public static User GetUser()
    //{   //create a request to the API endpoint
    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/5");
    //    //set the method to GET 
    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    //read the response stream and convert it to a string
    //    StreamReader reader = new StreamReader(response.GetResponseStream());
    //    string jsonResponse = reader.ReadToEnd();
    //    //reader.Close();
    //    //return the deserialized User object
    //    return JsonUtility.FromJson<User>(jsonResponse);
    //}
    /// <summary>
    /// Login to the API with username and password
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Register a new user via the API
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    //public static bool RegisterAPI(string userName, string password, string email)
    //{
    //    try
    //    {
    //        // Create a request to the API endpoint with required parameters
    //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
    //            "https://localhost:7035/User/register?userName=" + userName +
    //            "&password=" + password +
    //            "&email=" + email);

    //        request.Method = "POST";

    //        // Get the response
    //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

    //        // Check if request was successful (status code 200-299)
    //        bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

    //        // Read and parse the response if needed
    //        StreamReader reader = new StreamReader(response.GetResponseStream());
    //        string jsonResponse = reader.ReadToEnd();
    //        reader.Close();

    //        Debug.Log("Registration response: " + jsonResponse);

    //        return success;
    //    }
    //    catch (WebException ex)
    //    {
    //        // Log the error
    //        if (ex.Response != null)
    //        {
    //            using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
    //            {
    //                Debug.LogError("Registration error: " + reader.ReadToEnd());
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("Registration error: " + ex.Message);
    //        }
    //        return false;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogError("Unexpected error during registration: " + ex.Message);
    //        return false;
    //    }
    //}

    /// <summary>
    /// Register a new user via the API (without email)
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static bool RegisterAPI(string userName, string password)
    {
        try
        {
            // Create a request to the API endpoint with required parameters (no email)
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "https://localhost:7035/User/register?userName=" + userName +
                "&password=" + password);

            request.Method = "POST";

            // Get the response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check if request was successful (status code 200-299)
            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            // Read and parse the response if needed
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("Registration response: " + jsonResponse);

            return success;
        }
        catch (WebException ex)
        {
            // Log the error
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

        // Parse the JSON response into a list of PlayerPet objects
        return JsonConvert.DeserializeObject<List<PlayerPet>>(jsonResponse);
    }

    public static int GetPlayerPetCount(string playerId)
    {
        List<PlayerPet> playerPets = GetPlayerPets(playerId);
        return playerPets != null ? playerPets.Count : 0;
    }


    public static Boolean UpdateUser()
    {
        // Load the user information from PlayerInfomation
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User information not found.");
            return false;
        }
        Debug.Log("Updating API user EXP: " + user.exp);
        try
        {
            // Ensure exp has a value (default to 0 if null)
            int expValue = user.exp.GetValueOrDefault(0);

            // Create a request to the API endpoint with required parameters including exp
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/" + user.id +
                "?role=" + user.role +
                "&userName=" + user.userName +
                "&password=" + user.password +
                "&email=" + user.email +
                "&level=" + user.level +
                "&coin=" + user.coin +
                "&diamond=" + user.diamond +
                "&gem=" + user.gem +
                "&exp=" + expValue);  // Added exp parameter

            request.Method = "PUT";
            request.ContentType = "application/json";

            // Serialize the user object to JSON
            string jsonData = JsonUtility.ToJson(user);
            using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(jsonData);
            }

            // Get the response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check if request was successful (status code 200-299)
            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            // Read and parse the response if needed
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("Update response: " + jsonResponse);
            return success;
        }
        catch (WebException ex)
        {
            // Log the error
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

                // parse the JSON response into a SearchResult object
                var result = JsonConvert.DeserializeObject<SearchResult>(jsonResponse);

                // return the list of players
                return result?.players ?? new List<User>();
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("SearchUser error: " + ex.Message);
            return new List<User>();
        }
    }

    // Create a class to match the JSON structure returned by the API
    [System.Serializable]
    public class SearchResult
    {
        public string message;
        public List<User> players;
    }



    /// <summary>
    /// Gets a user by their ID
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve</param>
    /// <returns>User object if found, null if not found or error occurs</returns>
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

                // Check if response is successful (status code 200-299)
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    // Parse the JSON response into a User object using JsonConvert
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
