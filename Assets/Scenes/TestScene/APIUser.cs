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

    public static bool RegisterAPI(string userName, string password, string email)
    {
        try
        {
            // Create a request to the API endpoint with required parameters
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
                "https://localhost:7035/User/register?userName=" + userName +
                "&password=" + password +
                "&email=" + email);

            request.Method = "POST";

            // Get the response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Check if request was successful (status code 200-299)
            bool success = (int)response.StatusCode >= 200 && (int)response.StatusCode < 300;

            // Read and parse the response if needed
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();
            reader.Close();

            Debug.Log("Registration response: " + jsonResponse);

            return success;
        }
        catch (WebException ex)
        {
            // Log the error
            if (ex.Response != null)
            {
                using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    Debug.LogError("Registration error: " + reader.ReadToEnd());
                }
            }
            else
            {
                Debug.LogError("Registration error: " + ex.Message);
            }
            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Unexpected error during registration: " + ex.Message);
            return false;
        }
    }

    //public static User Register(string userName, string password, string confirmPassword, string email)
    //{
    //    // Validate passwords match
    //    if (password != confirmPassword)
    //    {
    //        Debug.LogError("Passwords do not match");
    //        return null;
    //    }

    //    try
    //    {
    //        // Create a request to the API endpoint
    //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
    //            "https://localhost:7035/User/register?userName=" + userName +
    //            "&password=" + password +
    //            "&email=" + email);

    //        request.Method = "POST";

    //        // Get the response
    //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

    //        // Read and parse the response
    //        StreamReader reader = new StreamReader(response.GetResponseStream());
    //        string jsonResponse = reader.ReadToEnd();
    //        reader.Close();

    //        // After successful registration, log in to get the full user object
    //        return LoginAPI(userName, password);
    //    }
    //    catch (WebException ex)
    //    {
    //        // Log the error
    //        if (ex.Response != null)
    //        {
    //            using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
    //            {
    //                Debug.LogError("Registration error: " + reader.ReadToEnd());
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogError("Registration error: " + ex.Message);
    //        }
    //        return null;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogError("Unexpected error during registration: " + ex.Message);
    //        return null;
    //    }
    //}
}
