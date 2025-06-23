using Newtonsoft.Json;
using System;
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

    public static Boolean UpdatePlayerInventory(PlayerInventory playerInventory)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/PlayerInventory?playerId=" + playerInventory.playerID +"&shopProductId=" + playerInventory.shopProductID + "&quantity=" + playerInventory.quantity);
        request.Method = "POST";
        request.ContentType = "application/json";
        // Serialize the PlayerInventory object to JSON
        string jsonData = JsonConvert.SerializeObject(playerInventory);
        using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
        {
            writer.Write(jsonData);
        }
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (WebException ex)
        {
            Debug.LogError("Error updating player inventory: " + ex.Message);
            return false;
        }
    }

}
