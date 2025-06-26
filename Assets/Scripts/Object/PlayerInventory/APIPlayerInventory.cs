using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

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

    public static IEnumerator UpdatePlayerInventoryCoroutine(PlayerInventory playerInventory, System.Action<bool> callback)
    {
        string url = "https://localhost:7035/PlayerInventory?playerId=" + playerInventory.playerID
            + "&shopProductId=" + playerInventory.shopProductID
            + "&quantity=" + playerInventory.quantity;

        string jsonData = JsonUtility.ToJson(playerInventory);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError("Error updating player inventory: " + request.error);
            callback?.Invoke(false);
        }
    }



    public static bool DeletePlayerInventory(int playerId, int shopProductId)
    {
        try
        {
            string url = $"https://localhost:7035/PlayerInventory?playerId={playerId}&shopProductId={shopProductId}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("DeletePlayerInventory response: " + jsonResponse);

            return success;
        }
        catch (WebException ex)
        {
            if (ex.Response != null)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.LogError("DeletePlayerInventory error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("DeletePlayerInventory error: " + ex.Message);
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError("Unexpected error during DeletePlayerInventory: " + ex.Message);
            return false;
        }
    }

    // version using UnityWebRequest for coroutine
    public static IEnumerator DeletePlayerInventoryCoroutine(int playerId, int shopProductId, System.Action<bool> callback)
    {
        string url = $"https://localhost:7035/PlayerInventory?playerId={playerId}&shopProductId={shopProductId}";

        UnityWebRequest request = UnityWebRequest.Delete(url);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Delete response: " + request.downloadHandler.text);
            callback?.Invoke(true);
        }
        else
        {
            Debug.LogError("Error deleting player inventory: " + request.error);
            callback?.Invoke(false);
        }
    }


}
