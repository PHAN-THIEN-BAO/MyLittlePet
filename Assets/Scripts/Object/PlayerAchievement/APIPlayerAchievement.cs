using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;


public class APIPlayerAchievement : MonoBehaviour
{
    public static List<PlayerAchievement> GetAchievementByIdPlayer(int idPlayer)
    {
        
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/PlayerAchievement/Player/" + idPlayer);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        return JsonConvert.DeserializeObject<List<PlayerAchievement>>(jsonResponse);
    }


    public static bool AddAchievement(int achievementId)
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        string url = $"https://localhost:7035/PlayerAchievement?playerId={user.id}&achievementId={achievementId}&isCollected=false";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created;
            }
        }
        catch (WebException ex)
        {
            if (ex.Response is HttpWebResponse webResponse && webResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                Debug.Log("Aready had the achievement ");
            }
            else
            {
                Debug.LogError("AddAchievement failed: " + ex.Message);
            }
            return false;
        }
    }

    public static bool UpdatePlayerAchievement(int achievementId)
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User not found.");
            return false;
        }

        string url = $"https://localhost:7035/PlayerAchievement/{user.id}/{achievementId}/Collect";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/json";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent;
            }
        }
        catch (WebException ex)
        {
            Debug.LogError("UpdatePlayerAchievement failed: " + ex.Message);
            return false;
        }
    }



}