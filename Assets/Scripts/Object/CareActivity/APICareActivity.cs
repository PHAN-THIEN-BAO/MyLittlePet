using System.Collections.Generic;
using System.IO;
using System.Net;
using System;
using UnityEngine;
using Newtonsoft.Json;

public static class APICareActivity
{
    // Lấy tất cả các hoạt động chăm sóc
    public static List<CareActivity> GetAllCareActivities()
    {
        try
        {
            string url = "https://localhost:7035/CareActivity";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        Debug.Log("GetAllCareActivities response: " + jsonResponse);
                        return JsonConvert.DeserializeObject<List<CareActivity>>(jsonResponse);
                    }
                }
                else
                {
                    Debug.LogError("Error getting care activities: " + response.StatusDescription);
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in GetAllCareActivities: " + ex.Message);
            return null;
        }
    }
}
