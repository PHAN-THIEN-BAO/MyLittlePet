using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShowShopProduct : MonoBehaviour
{
    [SerializeField] public List<TMP_Text> name;
    [SerializeField] public List<TMP_Text> value;
    [SerializeField] public List<GameObject> coinDisplay;
    [SerializeField] public List<GameObject> diamondDisplay;
    [SerializeField] public List<GameObject> gemDisplay;
    [SerializeField] public List<Image> productImages;
    [SerializeField] public List<Sprite> productSprites;



    public void GetProducts(string type)
    {
        List<ShopProduct> products = APIShopProduct.GetAllShopProducts(type);
        int count = Mathf.Min(name.Count, products.Count);

        for (int i = 0; i < count; i++)
        {
            name[i].text = products[i].name;
            value[i].text = products[i].price.ToString();
            if (i < productSprites.Count && i < productImages.Count)
            {
                productImages[i].sprite = productSprites[i];
                productImages[i].gameObject.SetActive(true);
            }
            // Set the active state of the currency displays based on the product's currency type
            if (products[i].currencyType == "Coin")
            {
                coinDisplay[i].SetActive(true);
                diamondDisplay[i].SetActive(false);
                gemDisplay[i].SetActive(false);
            }
            else if (products[i].currencyType == "Diamond")
            {
                coinDisplay[i].SetActive(false);
                diamondDisplay[i].SetActive(true);
                gemDisplay[i].SetActive(false);
            }
            else if (products[i].currencyType == "Gem")
            {
                coinDisplay[i].SetActive(false);
                diamondDisplay[i].SetActive(false);
                gemDisplay[i].SetActive(true);
            }
            else
            {
                // if currency type is not recognized, default to coin
                coinDisplay[i].SetActive(true);
                diamondDisplay[i].SetActive(false);
                gemDisplay[i].SetActive(false);
            }
        }

        // hide remaining displays if there are more displays than products
        for (int i = count; i < name.Count; i++)
        {
            coinDisplay[i].SetActive(false);
            diamondDisplay[i].SetActive(false);
            gemDisplay[i].SetActive(false);
        }
    }


}
