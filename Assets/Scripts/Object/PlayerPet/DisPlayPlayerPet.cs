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
        for (int i = tranformPet.childCount - 1; i >= 0; i--)
        {
            var child = tranformPet.GetChild(i).gameObject;
            if (child != pet)
                Destroy(child);
        }
        petClones.Clear();
        string[] parts = playerId.text.Split(':');
        if (parts.Length < 2) return;
        if (!int.TryParse(parts[1].Trim(), out int userId)) return;
        List<PlayerPet> playerPets = APIPlayerPet.GetPlayerPetByPlayerId(userId);
        if (playerPets != null)
            numberOfPet.text = "Pet Own: " + playerPets.Count.ToString();
        else
            numberOfPet.text = "Pet Own: 0";
        pet.SetActive(false);
        if (playerPets == null || playerPets.Count == 0) return;
        foreach (var playerPet in playerPets)
        {
            GameObject petObj = Instantiate(pet, tranformPet);
            petObj.SetActive(true);
            petClones.Add(petObj);
            var nameText = petObj.transform.Find("Name_Player_Pet")?.GetComponent<TMP_Text>();
            if (nameText != null) nameText.text = playerPet.petCustomName;
            var levelText = petObj.transform.Find("Level")?.GetComponent<TMP_Text>();
            if (levelText != null) levelText.text = "Lv:" + playerPet.level.ToString();
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
    private void ResetScrollPosition()
    {
        if (scrollListPet != null)
        {
            ScrollRect scrollRect = scrollListPet.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
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