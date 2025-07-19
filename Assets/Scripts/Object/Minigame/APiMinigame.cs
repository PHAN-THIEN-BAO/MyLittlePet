using System.IO;
using System.Net;
using UnityEngine;

public class APiMinigame : MonoBehaviour
{
    public static Minigame GetMinigameById(int minigameID)
    {
        string url = $"https://localhost:7035/Minigame/{minigameID}";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();
                Minigame minigame = JsonUtility.FromJson<Minigame>(json);
                return minigame;
            }
        }
        catch (WebException ex)
        {
            Debug.LogError($"HTTP Error: {ex.Message}");
            return null;
        }
    }
}
