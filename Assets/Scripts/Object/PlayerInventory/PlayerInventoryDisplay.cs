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
    private GameObject _itemPrefabCache;
    private void Awake()
    {
        if (Item != null)
        {
            _itemPrefabCache = Item;
        }
    }
    public void DisplayPlayerInventory()
    {
        if (Item == null && _itemPrefabCache != null)
        {
            Item = _itemPrefabCache;
            Debug.Log("Restored Item prefab from cache");
        }
        if (Item == null)
        {
            Debug.LogError("Item prefab is missing! Please assign in Inspector.");
            return;
        }
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User information not found.");
            return;
        }
        List<PlayerInventory> playerInventory = APIPlayerInventory.GetPlayerInventory(user.id.ToString());
        if (playerInventory == null)
        {
            Debug.LogWarning("API returned null player inventory");
            playerInventory = new List<PlayerInventory>();
        }
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        name.Clear();
        quantity.Clear();
        productImages.Clear();
        shopProductId.Clear();
        adopButtons.Clear();
        playerId.Clear();
        nameInput.Clear();
        adopButton.Clear();
        openAdopButton.Clear();
        int count = playerInventory.Count;
        if (count == 0)
        {
            Debug.Log("Player inventory is empty");
            return;
        }
        for (int i = 0; i < count; i++)
        {
            GameObject newItem = Instantiate(Item, contentPanel);
            newItem.SetActive(true);
            TMP_Text nameText = newItem.transform.Find("Name_Item")?.GetComponent<TMP_Text>();
            TMP_Text quantityText = newItem.transform.Find("Quantity")?.GetComponent<TMP_Text>();
            Image itemImage = newItem.transform.Find("Item_Image")?.GetComponent<Image>();
            TMP_Text shopProductIdText = newItem.transform.Find("Shop_Product_Id")?.GetComponent<TMP_Text>();
            Button button = newItem.transform.Find("Open_Adop_Button")?.GetComponent<Button>();
            TMP_Text playerIdText = newItem.transform.Find("Player_Id")?.GetComponent<TMP_Text>();
            if (nameText == null || quantityText == null || itemImage == null ||
                shopProductIdText == null || button == null || playerIdText == null)
            {
                Debug.LogError("Some required components are missing in the Item prefab. Check hierarchy in the image.");
                continue;
            }
            name.Add(nameText);
            quantity.Add(quantityText);
            productImages.Add(itemImage);
            shopProductId.Add(shopProductIdText);
            adopButtons.Add(button);
            playerId.Add(playerIdText);
            ShopProduct product = APIShopProduct.GetShopProductById(playerInventory[i].shopProductID);
            if (product != null)
            {
                name[i].text = product.name;
                quantity[i].text = playerInventory[i].quantity.ToString();
                shopProductId[i].text = playerInventory[i].shopProductID.ToString();
                playerId[i].text = playerInventory[i].playerID.ToString();
                Transform itemTransform = name[i].transform.parent;
                if (product.petID == null)
                {
                    var adopBtnObj = itemTransform.Find("Adop_Button");
                    if (adopBtnObj != null) adopBtnObj.gameObject.SetActive(false);
                    var adopBtn = adopButtons[i];
                    if (adopBtn != null) adopBtn.gameObject.SetActive(false);
                    var nameInputObj = itemTransform.Find("Name_Input");
                    if (nameInputObj != null) nameInputObj.gameObject.SetActive(false);
                }
                else
                {
                    var adopBtnObj = itemTransform.Find("Adop_Button");
                    if (adopBtnObj != null) adopBtnObj.gameObject.SetActive(false);
                    var adopBtn = adopButtons[i];
                    if (adopBtn != null) adopBtn.gameObject.SetActive(true);
                    var nameInputObj = itemTransform.Find("Name_Input");
                    if (nameInputObj != null) nameInputObj.gameObject.SetActive(false);
                }
                bool isPet = IsPet(playerInventory[i]);
                adopButtons[i].gameObject.SetActive(isPet);
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
    }
    private IEnumerator LoadImageFromUrl(string url, Image targetImage)
    {
        if (targetImage == null)
        {
            Debug.LogError("Target image is null");
            yield break;
        }
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (targetImage == null)
        {
            Debug.LogWarning("Target image has been destroyed during loading");
            yield break;
        }
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
            List<ShopProduct> allPetProducts = APIShopProduct.GetAllShopProducts("Pet");
            ShopProduct product = APIShopProduct.GetShopProductById(playerInventory.shopProductID);
            if (product == null)
            {
                Debug.LogError($"Could not find shop product with ID: {playerInventory.shopProductID}");
                return false;
            }
            return product.petID != null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in IsPet check: {ex.Message}");
            return false;
        }
    }
}