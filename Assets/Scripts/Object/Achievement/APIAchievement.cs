using System.Net;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
public class APIAchievement : MonoBehaviour
{
    public static List<Achievement> GetAllAchievements()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/Achievement");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        return JsonConvert.DeserializeObject<List<Achievement>>(jsonResponse);
    }
}