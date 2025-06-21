using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class APIPlayerInventory : MonoBehaviour
{

    public static List<PlayerInventory> GetPlayerInventory(string playerId)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/PlayerInventory/Player/" + playerId);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        // Read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        // Parse the JSON response into a list of PlayerInventory objects
        return JsonConvert.DeserializeObject<List<PlayerInventory>>(jsonResponse);
    }

}
