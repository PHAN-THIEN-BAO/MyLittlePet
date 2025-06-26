using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Linq;

public class PlayerInventoryDisplay : MonoBehaviour
{
    [SerializeField] public List<TMP_Text> name;
    [SerializeField] public List<TMP_Text> quantity;
    [SerializeField] public List<TMP_Text> shopProductId;
    [SerializeField] public List<TMP_Text> playerId;
    [SerializeField] public List<Image> productImages;
    [SerializeField] public GameObject Item;
    [SerializeField] public Transform contentPanel;
    [SerializeField] public List<Button> adopButtons;
    [SerializeField] public List<TMP_InputField> nameInput;
    [SerializeField] public List<Button> adopButton;
    [SerializeField] public List<Button> openAdopButton;


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
            TMP_Text shopProductIdText = newItem.transform.Find("Shop_Product_Id").GetComponent<TMP_Text>();
            Button button = newItem.transform.Find("Open_Adop_Button").GetComponent<Button>();
            TMP_Text playerIdText = newItem.transform.Find("Player_Id").GetComponent<TMP_Text>();
            name.Add(nameText);
            quantity.Add(quantityText);
            productImages.Add(itemImage);
            shopProductId.Add(shopProductIdText);
            adopButtons.Add(button);
            playerId.Add(playerIdText);

        }

        int count = playerInventory.Count;

        for (int i = 0; i < count; i++)
        {
            ShopProduct product = APIShopProduct.GetShopProductById(playerInventory[i].shopProductID);
            if (product != null)
            {
                name[i].text = product.name;
                quantity[i].text = playerInventory[i].quantity.ToString();
                shopProductId[i].text = playerInventory[i].shopProductID.ToString();
                playerId[i].text = playerInventory[i].playerID.ToString();


                // take the parent transform of the name[i] TextMeshPro component
                Transform itemTransform = name[i].transform.parent;

                // check if petID is null
                if (product.petID == null)
                {
                    // Hide Adop_Button and Name_Input if petID is null
                    var adopBtnObj = itemTransform.Find("Adop_Button");
                    if (adopBtnObj != null) adopBtnObj.gameObject.SetActive(false);

                    var adopBtn = adopButtons[i];
                    if (adopBtn != null) adopBtn.gameObject.SetActive(false);

                    var nameInputObj = itemTransform.Find("Name_Input");
                    if (nameInputObj != null) nameInputObj.gameObject.SetActive(false);
                }
                else
                {
                    // show Adop_Button and Name_Input if petID is not null
                    var adopBtnObj = itemTransform.Find("Adop_Button");
                    if (adopBtnObj != null) adopBtnObj.gameObject.SetActive(false);

                    var adopBtn = adopButtons[i];
                    if(adopBtn != null) adopBtn.gameObject.SetActive(true);

                    var nameInputObj = itemTransform.Find("Name_Input");
                    if (nameInputObj != null) nameInputObj.gameObject.SetActive(false);
                }


                // Check if item is a pet and show/hide UI elements accordingly
                bool isPet = IsPet(playerInventory[i]);
                adopButtons[i].gameObject.SetActive(isPet);


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

    private Boolean IsPet(PlayerInventory playerInventory)
    {
        try
        {
            // Get all pet products to check if this shopProductId belongs to a pet
            List<ShopProduct> allPetProducts = APIShopProduct.GetAllShopProducts("Pet");

            //// Check if the shopProductId exists in pet products
            //bool isPetProduct = allPetProducts.Any(p => p.shopProductID == shopProductId);

            //if (!isPetProduct)
            //{
            //    return false; // Not a pet product
            //}

            // Get the specific shop product to check petId
            ShopProduct product = APIShopProduct.GetShopProductById(playerInventory.shopProductID);
            if (product == null)
            {
                Debug.LogError($"Could not find shop product with ID: {playerInventory.shopProductID}");
                return false;
            }

            // Check if petId is null
            return product.petID != null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in IsPet check: {ex.Message}");
            return false;
        }
    }

}
