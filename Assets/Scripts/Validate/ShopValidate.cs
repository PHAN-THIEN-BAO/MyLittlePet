using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopValidate : MonoBehaviour
{
    public bool CheckCanBuy(GameObject notEnoughMoneyPanel)
    {
        // take user information
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User not found.");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true);
            return false;
        }

        // take product ID from sibling Id_Item
        Transform idItemTransform = transform.parent.Find("Id_Item");
        if (idItemTransform == null)
        {
            Debug.LogError($"Id_Item not found as sibling of {gameObject.name}");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true);
            return false;
        }

        TMP_Text idText = idItemTransform.GetComponent<TMP_Text>();
        if (idText == null)
        {
            Debug.LogError("TMP_Text component not found on Id_Item");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true);
            return false;
        }

        int shopProductID = int.Parse(idText.text);

        // take product from database by ID
        ShopProduct product = APIShopProduct.GetShopProductById(shopProductID);
        if (product == null)
        {
            Debug.LogError("Product not found.");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true);
            return false;
        }

        // take user currency based on product's currency type
        int userCurrency = 0;
        if (product.currencyType == "Coin")
            userCurrency = user.coin;
        else if (product.currencyType == "Diamond")
            userCurrency = user.diamond;
        else if (product.currencyType == "Gem")
            userCurrency = user.gem;

        int quantity = 1;

        // check if user has enough currency to buy the product
        bool canBuy = CurenciesValidation.ValidateCurrencies(userCurrency, product.price, quantity);
        if (!canBuy)
        {
            Debug.LogWarning("Not enough money to buy!");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true); // show panel if not enough money
            return false;
        }
        else
        {
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(false); // hide panel if enough money
            return true;
        }
    }
    /// <summary>
    /// Check if the player can buy a pet based on their existing pets.
    /// </summary>
    /// <param name="OwnedPetPanel"></param>
    /// <param name="petId"></param>
    /// <returns></returns>
    public bool CheckCanBuyPet(GameObject OwnedPetPanel, int petId)
    {
        PlayerPet playerPet = APIPlayerPet.GetPlayerPetById(petId);
        if (playerPet == null)
        {
            Debug.Log("Player pet not found, ok can buy");

            return true;
        }
        else
        {
            Debug.Log("Player pet already exists, cannot buy");
            if (OwnedPetPanel != null)
                OwnedPetPanel.SetActive(true);
            return false;
        }
    }

}
