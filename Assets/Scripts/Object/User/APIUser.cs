using UnityEngine;
using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

public static class APIUser
{
    /// <summary>
    /// test API call to get a user by ID
    /// </summary>
    /// <returns></returns>
    public static User GetUser()
    {   //create a request to the API endpoint
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/5");
        //set the method to GET 
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        //reader.Close();
        //return the deserialized User object
        return JsonUtility.FromJson<User>(jsonResponse);
    }
    /// <summary>
    /// Login to the API with username and password
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static User LoginAPI(string userName, string password)
    {
        //create a request to the API endpoint
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/login?UserName="+ userName + "&Password=" + password);

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
    public static bool RegisterAPI(string userName, string password, string email)
    {
        try
        {
            // Create a request to the API endpoint with required parameters
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "https://localhost:7035/User/register?userName=" + userName +
                "&password=" + password +
                "&email=" + email);

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
}
