using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;


public class APIPlayerAchievement : MonoBehaviour
{
    public static List<PlayerAchievement> GetAchievementByIdPlayer(int idPlayer)
    {
        // Create a request to the API endpoint with the player ID
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerAchievement/Player/" + idPlayer);
        // Set the method to GET
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        // Parse the JSON response into a list of PlayerPet objects
        return JsonConvert.DeserializeObject<List<PlayerAchievement>>(jsonResponse);
    }


}
