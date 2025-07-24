using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DisPlayPlayerPet : MonoBehaviour
{
    [SerializeField] public TMP_Text playerId;
    [SerializeField] public GameObject pet;
    [SerializeField] public TMP_Text numberOfPet;
    [SerializeField] public Transform tranformPet;
    [SerializeField] public GameObject scrollListPet;

    private List<GameObject> petClones = new List<GameObject>();
    private const string defaultImageUrl = "https://drive.google.com/uc?id=1fsJXvABMVtfGSPJz7E-_yhqv0H7Fo8oS";

    public void DisplayListPet()
    {
        // 1. Delete all cloned pets except the original prefab
        for (int i = tranformPet.childCount - 1; i >= 0; i--)
        {
            var child = tranformPet.GetChild(i).gameObject;
            if (child != pet) // not the original pet prefab
                Destroy(child);
        }
        petClones.Clear();
        // 2. split playerId text to get userId
        string[] parts = playerId.text.Split(':');
        if (parts.Length < 2) return;
        if (!int.TryParse(parts[1].Trim(), out int userId)) return;

        // 3. call API to get player pets by userId
        List<PlayerPet> playerPets = APIPlayerPet.GetPlayerPetByPlayerId(userId);

        // 4. show number of pets
        if (playerPets != null)
            numberOfPet.text = "Pet Own: " + playerPets.Count.ToString();
        else
            numberOfPet.text = "Pet Own: 0";




        // 5. Hide the original pet prefab
        pet.SetActive(false);

        if (playerPets == null || playerPets.Count == 0) return;

        // 6. Clone pet and set data for each pet
        foreach (var playerPet in playerPets)
        {
            GameObject petObj = Instantiate(pet, tranformPet);
            petObj.SetActive(true);
            petClones.Add(petObj);

            // Set tên
            var nameText = petObj.transform.Find("Name_Player_Pet")?.GetComponent<TMP_Text>();
            if (nameText != null) nameText.text = playerPet.petCustomName;

            // Level functionality has been removed from the database schema
            // Set level display to show status instead
            var levelText = petObj.transform.Find("Level")?.GetComponent<TMP_Text>();
            if (levelText != null) levelText.text = "Status: " + (playerPet.status ?? "Active");

            // Set avatar
            var avatarImage = petObj.transform.Find("Avatar")?.GetComponent<Image>();
            if (avatarImage != null)
            {
                ShopProduct shopProduct = APIShopProduct.GetShopProductByIdPet(playerPet.petID);
                Debug.Log($"PetID: {playerPet.petID}, ShopProduct: {(shopProduct != null ? shopProduct.name : "null")}, imageUrl: {(shopProduct != null ? shopProduct.imageUrl : "null")}");
                string imageUrl = (shopProduct != null && !string.IsNullOrEmpty(shopProduct.imageUrl))
                    ? shopProduct.imageUrl.Trim()
                    : defaultImageUrl;
                if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://"))
                    imageUrl = "https://" + imageUrl;
                StartCoroutine(LoadImage(imageUrl, avatarImage));
            }
        }
        // 7. scroll list pet to the top
        ResetScrollPosition();
    }

    private IEnumerator LoadImage(string url, Image image)
    {
        using (var www = new UnityEngine.Networking.UnityWebRequest(url))
        {
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerTexture();
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var texture = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                image.preserveAspect = true;
            }
        }
    }

    /// <summary>
    /// Set scroll position of the scrollListPet to the top.
    /// </summary>
    private void ResetScrollPosition()
    {
        if (scrollListPet != null)
        {
            ScrollRect scrollRect = scrollListPet.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                // scroll to the top using normalized position
                scrollRect.normalizedPosition = new Vector2(0, 1);


            }
            else
            {
                Debug.LogWarning("not found component ScrollRect on scrollListPet");
            }
        }
        else
        {
            Debug.LogWarning("scrollListPet null at Inspector");
        }
    }
}
