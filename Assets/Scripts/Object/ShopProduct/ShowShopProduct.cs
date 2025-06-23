using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class ShowShopProduct : MonoBehaviour
{
    [SerializeField] public List<TMP_Text> name;
    [SerializeField] public List<TMP_Text> Id;
    [SerializeField] public List<TMP_Text> value;
    [SerializeField] public List<GameObject> coinDisplay;
    [SerializeField] public List<GameObject> diamondDisplay;
    [SerializeField] public List<GameObject> gemDisplay;
    [SerializeField] public List<Image> productImages;
    [SerializeField] public List<Sprite> productSprites;
    [SerializeField] public GameObject Item;
    [SerializeField] public Transform contentPanel; // pull content panel from the scene




    //public void GetProducts(string type)
    //{
    //    List<ShopProduct> products = APIShopProduct.GetAllShopProducts(type);
    //    int count = Mathf.Min(name.Count, products.Count);

    //    for (int i = 0; i < count; i++)
    //    {
    //        name[i].text = products[i].name;
    //        value[i].text = products[i].price.ToString();
    //        if (i < productSprites.Count && i < productImages.Count)
    //        {
    //            productImages[i].sprite = productSprites[i];
    //            productImages[i].gameObject.SetActive(true);
    //        }
    //        // Set the active state of the currency displays based on the product's currency type
    //        if (products[i].currencyType == "Coin")
    //        {
    //            coinDisplay[i].SetActive(true);
    //            diamondDisplay[i].SetActive(false);
    //            gemDisplay[i].SetActive(false);
    //        }
    //        else if (products[i].currencyType == "Diamond")
    //        {
    //            coinDisplay[i].SetActive(false);
    //            diamondDisplay[i].SetActive(true);
    //            gemDisplay[i].SetActive(false);
    //        }
    //        else if (products[i].currencyType == "Gem")
    //        {
    //            coinDisplay[i].SetActive(false);
    //            diamondDisplay[i].SetActive(false);
    //            gemDisplay[i].SetActive(true);
    //        }
    //        else
    //        {
    //            // if currency type is not recognized, default to coin
    //            coinDisplay[i].SetActive(true);
    //            diamondDisplay[i].SetActive(false);
    //            gemDisplay[i].SetActive(false);
    //        }
    //    }

    //    // hide remaining displays if there are more displays than products
    //    for (int i = count; i < name.Count; i++)
    //    {
    //        coinDisplay[i].SetActive(false);
    //        diamondDisplay[i].SetActive(false);
    //        gemDisplay[i].SetActive(false);
    //    }
    //}

    public void GetProducts(string type)
    {
        List<ShopProduct> products = APIShopProduct.GetAllShopProducts(type);

        // add sprites to the productSprites list if not already added
        while (name.Count < products.Count)
        {
            GameObject newItem = Instantiate(Item, contentPanel);

            TMP_Text nameText = newItem.transform.Find("Name_Item").GetComponent<TMP_Text>();
            TMP_Text valueText = newItem.transform.Find("Price").GetComponent<TMP_Text>();
            TMP_Text idText = newItem.transform.Find("Id_Item").GetComponent<TMP_Text>(); // Thêm dòng này
            Image itemImage = newItem.transform.Find("Item_Image").GetComponent<Image>();
            GameObject coinImg = newItem.transform.Find("Coin_Img").gameObject;
            GameObject diamondImg = newItem.transform.Find("Diamond_Img").gameObject;
            GameObject gemImg = newItem.transform.Find("Gem_Img").gameObject;

            name.Add(nameText);
            value.Add(valueText);
            Id.Add(idText); 
            productImages.Add(itemImage);
            coinDisplay.Add(coinImg);
            diamondDisplay.Add(diamondImg);
            gemDisplay.Add(gemImg);
        }

        int count = products.Count;

        for (int i = 0; i < count; i++)
        {
            name[i].text = products[i].name;
            value[i].text = products[i].price.ToString();

            // Set the Id text
            Id[i].text = products[i].shopProductID.ToString();
            Id[i].gameObject.SetActive(false);

            // Load image from URL
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



        // hide remaining displays if there are more displays than products
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
