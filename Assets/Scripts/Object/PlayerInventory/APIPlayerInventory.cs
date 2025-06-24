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

}
