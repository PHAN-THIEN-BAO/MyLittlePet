using UnityEngine;
using System.Net;
using System.IO;

public static class APIUser
{
  public static User GetUser()
    {   //create a request to the API endpoint
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/5");
        //set the method to GET 
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //read the response stream and convert it to a string
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        //reader.Close();
        //return the deserialized User object
        return JsonUtility.FromJson<User>(jsonResponse);
    }
    public static User LoginAPI(string userName, string password)
    {
        //create a request to the API endpoint
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://localhost:7035/User/login?UserName="+ userName + "&Password=" + password);

        //get the response
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        //return the deserialized User object
        return JsonUtility.FromJson<User>(jsonResponse);
    }
}
