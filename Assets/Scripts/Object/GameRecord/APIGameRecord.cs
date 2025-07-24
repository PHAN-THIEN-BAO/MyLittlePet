using System.Net;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class APIGameRecord : MonoBehaviour
{
    public static List<GameRecord> GetGameRecordByPlayerID(int playerID)
    {
        string url = $"https://localhost:7035/GameRecord/Player/{playerID}";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<GameRecord>>(json);
            }
        }
        catch (WebException ex)
        {
            Debug.LogError($"HTTP Error: {ex.Message}");
            return null;
        }
    }


    public static void SendGameRecord(string method, int playerId, int minigameId, int score)
    {
        string url = $"https://localhost:7035/GameRecord?playerId={playerId}&minigameId={minigameId}&score={score}";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToUpper();
            request.ContentLength = 0;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //can check response status code if needed
            }
        }
        catch (WebException ex)
        {
            Debug.LogError($"HTTP Error: {ex.Message}");
        }
    }


}