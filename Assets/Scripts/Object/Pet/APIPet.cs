using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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
}
