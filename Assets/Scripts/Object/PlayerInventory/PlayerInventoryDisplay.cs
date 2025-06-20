using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryDisplay : MonoBehaviour
{
    [SerializeField] public List<TMP_Text> name;
    [SerializeField] public List<TMP_Text> quantity;
    [SerializeField] public List<Image> productImages;
    [SerializeField] public List<Sprite> productSprites;

    /// <summary>
    /// Displays the player's inventory by fetching data from the API and updating the UI elements.
    /// </summary>
    public void DisplayPlayerInventory()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User information not found.");
            return;
        }

        List<PlayerInventory> playerInventory = APIPlayerInventory.GetPlayerInventory(user.id.ToString());
        int count = Mathf.Min(name.Count, quantity.Count, productImages.Count, playerInventory.Count);

        for (int i = 0; i < count; i++)
        {
            // get product information from API
            ShopProduct product = APIShopProduct.GetShopProductById(playerInventory[i].shopProductID);
            if (product != null)
            {
                name[i].text = product.name;
                quantity[i].text = playerInventory[i].quantity.ToString();

                // set product image if available
                if (i < productSprites.Count)
                {
                    productImages[i].sprite = productSprites[i];
                    productImages[i].gameObject.SetActive(true);
                }
            }
            else
            {
                name[i].text = "Unknown";
                quantity[i].text = playerInventory[i].quantity.ToString();
                if (i < productImages.Count)
                    productImages[i].gameObject.SetActive(false);
            }
        }

        // hide remaining displays if there are more displays than inventory items
        for (int i = count; i < name.Count; i++)
        {
            name[i].text = "";
            quantity[i].text = "";
            if (i < productImages.Count)
                productImages[i].gameObject.SetActive(false);
        }
    }
}
