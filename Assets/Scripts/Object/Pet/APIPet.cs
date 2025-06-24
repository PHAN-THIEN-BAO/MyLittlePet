using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Net;
using System.IO;

public class APIPet : MonoBehaviour
{
    // Takes a pet ID and returns the corresponding Pet object
    public static IEnumerator GetPetByIdCoroutine(int petId, System.Action<Pet> callback)
    {
        string url = "https://localhost:7035/Pet/" + petId;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Pet pet = JsonUtility.FromJson<Pet>(request.downloadHandler.text);
                callback?.Invoke(pet);
            }
            else
            {
                Debug.LogError("Error getting pet: " + request.error);
                callback?.Invoke(null);
            }
        }
    }


    public static Pet GetPetById(int petId)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://localhost:7035/Pet/{petId}");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        reader.Close();
        return JsonUtility.FromJson<Pet>(jsonResponse);
    }
}
