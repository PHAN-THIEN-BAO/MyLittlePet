using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
public class ShowShopProduct : MonoBehaviour
{
    [SerializeField] public GameObject scrollViewObject;
    [SerializeField] public List<TMP_Text> name;
    [SerializeField] public List<TMP_Text> Id;
    [SerializeField] public List<TMP_Text> Pet_Id;
    [SerializeField] public List<TMP_Text> value;
    [SerializeField] public List<TMP_Text> description;
    [SerializeField] public List<GameObject> coinDisplay;
    [SerializeField] public List<GameObject> diamondDisplay;
    [SerializeField] public List<GameObject> gemDisplay;
    [SerializeField] public List<Image> productImages;
    [SerializeField] public List<Sprite> productSprites;
    [SerializeField] public GameObject Item;
    [SerializeField] public Transform contentPanel;
    public void GetProducts(string type)
    {
        List<ShopProduct> products = APIShopProduct.GetAllShopProducts(type);
        while (name.Count < products.Count)
        {
            GameObject newItem = Instantiate(Item, contentPanel);
            TMP_Text nameText = newItem.transform.Find("Name_Item").GetComponent<TMP_Text>();
            TMP_Text descriptionText = newItem.transform.Find("Description").GetComponent<TMP_Text>();
            TMP_Text valueText = newItem.transform.Find("Price").GetComponent<TMP_Text>();
            TMP_Text idText = newItem.transform.Find("Id_Item").GetComponent<TMP_Text>();
            TMP_Text petIdText = newItem.transform.Find("Pet_Id").GetComponent<TMP_Text>();
            Image itemImage = newItem.transform.Find("Item_Image").GetComponent<Image>();
            GameObject coinImg = newItem.transform.Find("Coin_Img").gameObject;
            GameObject diamondImg = newItem.transform.Find("Diamond_Img").gameObject;
            GameObject gemImg = newItem.transform.Find("Gem_Img").gameObject;
            name.Add(nameText);
            description.Add(descriptionText);
            value.Add(valueText);
            Id.Add(idText);
            Pet_Id.Add(petIdText);
            productImages.Add(itemImage);
            coinDisplay.Add(coinImg);
            diamondDisplay.Add(diamondImg);
            gemDisplay.Add(gemImg);
        }
        int count = products.Count;
        for (int i = 0; i < count; i++)
        {
            name[i].text = products[i].name;
            description[i].text = products[i].description;
            value[i].text = products[i].price.ToString();
            Pet_Id[i].text = products[i].petID.HasValue ? products[i].petID.Value.ToString() : "N/A";
            Id[i].text = products[i].shopProductID.ToString();
            Id[i].gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(products[i].imageUrl))
            {
                StartCoroutine(LoadImageFromUrl(products[i].imageUrl, productImages[i]));
            }
            else
            {
                productImages[i].gameObject.SetActive(false);
            }
            coinDisplay[i].SetActive(products[i].currencyType == "Coin");
            diamondDisplay[i].SetActive(products[i].currencyType == "Diamond");
            gemDisplay[i].SetActive(products[i].currencyType == "Gem");
        }
        for (int i = count; i < name.Count; i++)
        {
            name[i].transform.parent.gameObject.SetActive(false);
        }
        StartCoroutine(ScrollToTop());
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
    private IEnumerator ScrollToTop()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        if (scrollViewObject != null)
        {
            ScrollRect scrollRect = scrollViewObject.GetComponent<ScrollRect>();
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
            else
            {
                Debug.LogWarning("ScrollRect component not found on scrollViewObject");
            }
        }
        else
        {
            Debug.LogWarning("scrollViewObject is not assigned!");
        }
    }
}