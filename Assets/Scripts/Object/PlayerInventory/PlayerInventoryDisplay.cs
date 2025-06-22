using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerInventoryDisplay : MonoBehaviour
{
    [SerializeField] public List<TMP_Text> name;
    [SerializeField] public List<TMP_Text> quantity;
    [SerializeField] public List<Image> productImages;
    [SerializeField] public GameObject Item;
    [SerializeField] public Transform contentPanel;

    public void DisplayPlayerInventory()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User information not found.");
            return;
        }

        List<PlayerInventory> playerInventory = APIPlayerInventory.GetPlayerInventory(user.id.ToString());

        // Automatically adjust the size of the lists to match the player inventory count
        while (name.Count < playerInventory.Count)
        {
            GameObject newItem = Instantiate(Item, contentPanel);

            TMP_Text nameText = newItem.transform.Find("Name_Item").GetComponent<TMP_Text>();
            TMP_Text quantityText = newItem.transform.Find("Quantity").GetComponent<TMP_Text>();
            Image itemImage = newItem.transform.Find("Item_Image").GetComponent<Image>();

            name.Add(nameText);
            quantity.Add(quantityText);
            productImages.Add(itemImage);
        }

        int count = playerInventory.Count;

        for (int i = 0; i < count; i++)
        {
            ShopProduct product = APIShopProduct.GetShopProductById(playerInventory[i].shopProductID);
            if (product != null)
            {
                name[i].text = product.name;
                quantity[i].text = playerInventory[i].quantity.ToString();

                // Load image from URL
                if (!string.IsNullOrEmpty(product.imageUrl))
                {
                    StartCoroutine(LoadImageFromUrl(product.imageUrl, productImages[i]));
                }
                else
                {
                    productImages[i].gameObject.SetActive(false);
                }
            }
            else
            {
                name[i].text = "Unknown";
                quantity[i].text = playerInventory[i].quantity.ToString();
                productImages[i].gameObject.SetActive(false);
            }
        }

        // hide remaining items if there are more items than inventory
        for (int i = count; i < name.Count; i++)
        {
            name[i].transform.parent.gameObject.SetActive(false);
        }
    }

    private IEnumerator LoadImageFromUrl(string url, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            targetImage.sprite = sprite;
            targetImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Failed to load image: " + url + " - " + request.error);
            targetImage.gameObject.SetActive(false);
        }
    }
}
