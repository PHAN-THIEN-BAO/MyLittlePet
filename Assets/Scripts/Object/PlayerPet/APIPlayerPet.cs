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
        Debug.Log("Goi API voi playerPetId: " + playerPetId);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerPet/{playerPetId}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        return JsonConvert.DeserializeObject<PlayerPet>(jsonResponse);
    }

    public static IEnumerator UpdatePlayerPetCoroutine(PlayerPet playerPet, System.Action<bool> callback)
    {
        string safeCustomName = playerPet.petCustomName ?? "";
        string safeStatus = playerPet.status ?? "50%2550%2550";
        string url = $"https://localhost:7035/PlayerPet/{playerPet.playerPetID}?petCustomName={Uri.EscapeDataString(safeCustomName)}&status={Uri.EscapeDataString(safeStatus)}";
        
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

    public class AddPlayerPetResponse
    {
        public string message { get; set; }
        public PlayerPet playerPet { get; set; }
    }

    public static IEnumerator AddPlayerPetCoroutine(PlayerPet playerPet, System.Action<PlayerPet> callback)
    {
        string url = $"https://localhost:7035/PlayerPet?playerId={playerPet.playerID}&petId={playerPet.petID}&petCustomName={Uri.EscapeDataString(playerPet.petCustomName)}&status=50%2550%2550";

        WWWForm form = new WWWForm();
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Add response: " + request.downloadHandler.text);
            
            try
            {
                var response = JsonConvert.DeserializeObject<AddPlayerPetResponse>(request.downloadHandler.text);
                if (response != null && response.playerPet != null)
                {
                    callback?.Invoke(response.playerPet);
                }
                else
                {
                    Debug.LogError("Invalid response format or missing playerPet data");
                    callback?.Invoke(null);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error parsing AddPlayerPet response: " + ex.Message);
                callback?.Invoke(null);
            }
        }
        else
        {
            Debug.LogError("Error adding player pet: " + request.error);
            callback?.Invoke(null);
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



    //public class AddPlayerPetResponse


    public static bool AddPlayerPet(PlayerPet playerPet)
    {
        try
        {
            string url = $"https://localhost:7035/PlayerPet?playerId={playerPet.playerID}&petId={playerPet.petID}&petCustomName={Uri.EscapeDataString(playerPet.petCustomName)}&status=50%2550%2550";
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

    public static List<PlayerPet> GetPlayerPetByPlayerId(int userId)
    {
        string url = $"https://localhost:7035/PlayerPet/Player/{userId}";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log($"Retrieved {userId}'s pets: " + jsonResponse);

                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            Error = (sender, args) => args.ErrorContext.Handled = true
                        };

                        return JsonConvert.DeserializeObject<List<PlayerPet>>(jsonResponse, settings);
                    }
                }
                else
                {
                    Debug.LogError("Error getting player pets: " + response.StatusDescription);
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
                    Debug.LogError("GetPlayerPetByPlayerId error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("GetPlayerPetByPlayerId error: " + ex.Message);
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Unexpected error during GetPlayerPetByPlayerId: " + ex.Message);
            return null;
        }
    }
}