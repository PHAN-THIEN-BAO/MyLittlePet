using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using System;

public class APIPlayerPet : MonoBehaviour
{
    private static readonly string baseUrl = "https://localhost:7035";

    public static List<PlayerPet> GetPetsByPlayerId(int playerId)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{baseUrl}/PlayerPet/Player/{playerId}");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();
            return JsonConvert.DeserializeObject<List<PlayerPet>>(jsonResponse);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching pets for player {playerId}: {ex.Message}");
            return new List<PlayerPet>();
        }
    }

    public static PlayerPet GetPlayerPetById(int playerPetId)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{baseUrl}/PlayerPet/{playerPetId}");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();
            return JsonConvert.DeserializeObject<PlayerPet>(jsonResponse);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error fetching pet {playerPetId}: {ex.Message}");
            return null;
        }
    }

    public static bool UpdatePlayerPet(PlayerPet pet)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"{baseUrl}/PlayerPet/{pet.playerPetID}");
            request.Method = "PUT";
            request.ContentType = "application/json";

            string petJson = JsonConvert.SerializeObject(pet);
            byte[] data = Encoding.UTF8.GetBytes(petJson);

            request.ContentLength = data.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.NoContent;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error updating pet {pet.playerPetID}: {ex.Message}");
            return false;
        }
    }
}