using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System;

public class APICareHistory : MonoBehaviour
{
    // Get all care history records
    public static List<CareHistory> GetAllCareHistory()
    {
        try
        {
            string url = "https://localhost:7035/CareHistory";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log("GetAllCareHistory response: " + jsonResponse);
                        return JsonConvert.DeserializeObject<List<CareHistory>>(jsonResponse);
                    }
                }
                else
                {
                    Debug.LogError("Error getting care history: " + response.StatusDescription);
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in GetAllCareHistory: " + ex.Message);
            return null;
        }
    }

    // Get care history by ID
    public static CareHistory GetCareHistoryById(int careHistoryId)
    {
        try
        {
            string url = $"https://localhost:7035/CareHistory/{careHistoryId}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log("GetCareHistoryById response: " + jsonResponse);
                        return JsonConvert.DeserializeObject<CareHistory>(jsonResponse);
                    }
                }
                else
                {
                    Debug.LogError("Error getting care history by ID: " + response.StatusDescription);
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in GetCareHistoryById: " + ex.Message);
            return null;
        }
    }

    // Get care history for a specific player pet
    public static List<CareHistory> GetCareHistoryByPlayerPetId(int playerPetId)
    {
        try
        {
            string url = $"https://localhost:7035/CareHistory/PlayerPet/{playerPetId}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log("GetCareHistoryByPlayerPetId response: " + jsonResponse);
                        return JsonConvert.DeserializeObject<List<CareHistory>>(jsonResponse);
                    }
                }
                else
                {
                    Debug.LogError("Error getting care history by player pet ID: " + response.StatusDescription);
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in GetCareHistoryByPlayerPetId: " + ex.Message);
            return null;
        }
    }

    // Get care history for a specific player
    public static List<CareHistory> GetCareHistoryByPlayerId(int playerId)
    {
        try
        {
            string url = $"https://localhost:7035/CareHistory/Player/{playerId}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log("GetCareHistoryByPlayerId response: " + jsonResponse);
                        return JsonConvert.DeserializeObject<List<CareHistory>>(jsonResponse);
                    }
                }
                else
                {
                    Debug.LogError("Error getting care history by player ID: " + response.StatusDescription);
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in GetCareHistoryByPlayerId: " + ex.Message);
            return null;
        }
    }

    // Create a new care history record
    public static bool CreateCareHistory(int playerPetId, int playerId, int activityId)
    {
        try
        {
            string url = $"https://localhost:7035/CareHistory?playerPetId={playerPetId}&playerId={playerId}&activityId={activityId}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();
                    Debug.Log("CreateCareHistory response: " + jsonResponse);
                }

                return success;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in CreateCareHistory: " + ex.Message);
            return false;
        }
    }

    // Create care history using Coroutine (asynchronous)
    public static IEnumerator CreateCareHistoryCoroutine(int playerPetId, int playerId, int activityId, System.Action<bool> callback)
    {
        string url = $"https://localhost:7035/CareHistory?playerPetId={playerPetId}&playerId={playerId}&activityId={activityId}";

        WWWForm form = new WWWForm();
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("CreateCareHistory response: " + request.downloadHandler.text);
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError("Error creating care history: " + request.error);
            callback?.Invoke(false);
        }
    }

    // Delete a care history record
    public static bool DeleteCareHistory(int careHistoryId)
    {
        try
        {
            string url = $"https://localhost:7035/CareHistory/{careHistoryId}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();
                    Debug.Log("DeleteCareHistory response: " + jsonResponse);
                }

                return success;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in DeleteCareHistory: " + ex.Message);
            return false;
        }
    }

    // Delete care history using Coroutine (asynchronous)
    public static IEnumerator DeleteCareHistoryCoroutine(int careHistoryId, System.Action<bool> callback)
    {
        string url = $"https://localhost:7035/CareHistory/{careHistoryId}";

        UnityWebRequest request = UnityWebRequest.Delete(url);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("DeleteCareHistory response: " + request.downloadHandler.text);
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError("Error deleting care history: " + request.error);
            callback?.Invoke(false);
        }
    }
}