using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
        string url = "https://localhost:7035/PlayerPet?playerId=" + playerPet.playerID + "&petId=" + playerPet.petID +"&petCustomName=" + playerPet.petCustomName + "&status=100%25100%25100";
        string jsonData = JsonConvert.SerializeObject(playerPet);

        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

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
}
