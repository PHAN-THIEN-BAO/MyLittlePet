using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class APIPlayerPet : MonoBehaviour
{
    public static List<PlayerPet> GetPetsByPlayerId(int playerId)
    {
        // Create a request to the API endpoint with the player ID
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerPet/Player/{playerId}");
        // Set the method to GET
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        // Parse the JSON response into a list of PlayerPet objects
        return JsonConvert.DeserializeObject<List<PlayerPet>>(jsonResponse);
    }

    public static PlayerPet GetPlayerPetById(int playerPetId)
    {
        // Create a request to the API endpoint with the playerPet ID
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerPet/{playerPetId}");
        // Set the method to GET
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        // Parse the JSON response into a PlayerPet object
        return JsonConvert.DeserializeObject<PlayerPet>(jsonResponse);
    }
}