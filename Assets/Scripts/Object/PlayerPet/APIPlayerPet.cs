using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using static System.Net.WebRequestMethods;

public class APIPlayerPet : MonoBehaviour
{
    public static List<PlayerPet> GetPetsByPlayerId(int playerId)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerPet/Player/{playerId}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        return JsonConvert.DeserializeObject<List<PlayerPet>>(jsonResponse);
    }

    public static PlayerPet GetPlayerPetById(int playerPetId)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerPet/{playerPetId}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        return JsonConvert.DeserializeObject<PlayerPet>(jsonResponse);
    }

    // Used IEnumerator to allow for asynchronous web requests in Unity
    public static IEnumerator UpdatePlayerPetCoroutine(PlayerPet playerPet, System.Action<bool> callback)
    {
        // The API expects a PUT request to /PlayerPet/{id} with query parameters
        string url = $"https://localhost:7035/PlayerPet/{playerPet.playerPetID}?petCustomName={Uri.EscapeDataString(playerPet.petCustomName)}&level={playerPet.level}&status={Uri.EscapeDataString(playerPet.status ?? "100%25100%25100")}";
        
        UnityWebRequest request = UnityWebRequest.Put(url, "");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Update response: " + request.downloadHandler.text);
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError("Error updating player pet: " + request.error);
            callback?.Invoke(false);
        }
    }


    public static IEnumerator AddPlayerPetCoroutine(PlayerPet playerPet, System.Action<bool> callback)
    {
        string url = $"https://localhost:7035/PlayerPet?playerId={playerPet.playerID}&petId={playerPet.petID}&petCustomName={Uri.EscapeDataString(playerPet.petCustomName)}&status=100%25100%25100";

        // create a WWWForm to send data
        WWWForm form = new WWWForm();

        // create form fields
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Add response: " + request.downloadHandler.text);
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError("Error adding player pet: " + request.error);
            callback?.Invoke(false);
        }
    }






    public static PlayerPet GetPlayerPetByPlayerIdAndPetId(int playerId, int petId)
    {
        try
        {
        
            string url = "https://localhost:7035/PlayerPet/ByPlayerAndPet?playerId="+ playerId + "&petId=" + petId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log("GetPlayerPetByPlayerIdAndPetId response: " + jsonResponse);
                        return JsonConvert.DeserializeObject<PlayerPet>(jsonResponse);
                    }
                }
                else
                {
                    Debug.LogError("Error getting player pet: " + response.StatusDescription);
                    return null;
                }
            }
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.LogError("GetPlayerPetByPlayerIdAndPetId error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("GetPlayerPetByPlayerIdAndPetId error: " + ex.Message);
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Unexpected error during GetPlayerPetByPlayerIdAndPetId: " + ex.Message);
            return null;
        }
    }



    //public static bool AddPlayerPet(PlayerPet playerPet)
    //{
    //    string url = $"https://localhost:7035/PlayerPet?playerId={playerPet.playerID}&petId={playerPet.petID}&petCustomName={Uri.EscapeDataString(playerPet.petCustomName)}&status=100%25100%25100";
    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    StreamReader reader = new StreamReader(response.GetResponseStream());
    //    string jsonResponse = reader.ReadToEnd();
    //    reader.Close();
    //    Debug.Log("AddPlayerPet response: " + jsonResponse);

    //    if (response.StatusCode == HttpStatusCode.OK)
    //    {
    //        // Deserialize to List<PlayerPet>
    //        var playerPets = JsonConvert.DeserializeObject<List<PlayerPet>>(jsonResponse);
    //        if (playerPets != null && playerPets.Count > 0)
    //        {
    //            // take the last player pet from the list
    //            var newPlayerPet = playerPets[playerPets.Count - 1];
    //            playerPet.playerPetID = newPlayerPet.playerPetID;
    //            return true;
    //        }
    //        else
    //        {
    //            Debug.LogError("No player pet returned from API.");
    //            return false;
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("Error adding player pet: " + response.StatusDescription);
    //        return false;
    //    }
    //}


    //public class AddPlayerPetResponse
    //{
    //    public string message { get; set; }
    //    public PlayerPet playerPet { get; set; }
    //}


    public static bool AddPlayerPet(PlayerPet playerPet)
    {
        try
        {
            string url = $"https://localhost:7035/PlayerPet?playerId={playerPet.playerID}&petId={playerPet.petID}&petCustomName={Uri.EscapeDataString(playerPet.petCustomName)}&status=100%25100%25100";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("AddPlayerPet response: " + jsonResponse);

            return success;
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.LogError("AddPlayerPet error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("AddPlayerPet error: " + ex.Message);
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError("Unexpected error during AddPlayerPet: " + ex.Message);
            return false;
        }
    }

}
