using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class APIAchievement : MonoBehaviour
{
    public static List<Achievement> GetAllAchievements()
    {
        // Create a request to the API endpoint
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/Achievement");
        // Set the method to GET
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();

        // Parse the JSON response into a list of PlayerPet objects
        return JsonConvert.DeserializeObject<List<Achievement>>(jsonResponse);
    }

}
