using UnityEngine;
using TMPro;

public class BuyItem : MonoBehaviour
{


    public void BuyProduct()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User not found.");
            return;
        }

        foreach (Transform child in transform.parent)
        {
            Debug.Log("Sibling: " + child.name);
        }

        // 1. Take product ID from sibling Id_Item
        Transform idItemTransform = transform.parent.Find("Id_Item");
        if (idItemTransform == null)
        {
            Debug.LogError($"Id_Item not found as sibling of {gameObject.name}");
            return;
        }

        TMP_Text idText = idItemTransform.GetComponent<TMP_Text>();
        if (idText == null)
        {
            Debug.LogError("TMP_Text component not found on Id_Item");
            return;
        }

        int shopProductID = int.Parse(idText.text);

        // 2. Take product from database by ID
        ShopProduct product = APIShopProduct.GetShopProductById(shopProductID);
        if (product == null)
        {
            Debug.LogError("Product not found.");
            return;
        }

        // 3. check if user has enough currency to buy the product
        int quantity = 1;
        int userCurrency = ChooseUserCurrencies(product.currencyType);
        bool canBuy = CurenciesValidation.ValidateCurrencies(userCurrency, product.price, quantity);
        if (!canBuy)
        {
            Debug.LogWarning("Not enough currency to buy this product.");
            return;
        }

        // 4. Subtract currency from user
        PlayerInfomation.UpdatePlayerInfo(u =>
        {
            if (product.currencyType == "Coin")
                u.coin -= product.price * quantity;
            else if (product.currencyType == "Diamond")
                u.diamond -= product.price * quantity;
            else if (product.currencyType == "Gem")
                u.gem -= product.price * quantity;
        });

        // 5. Update user information in the database
        APIUser.UpdateUser();

        Debug.Log("Purchase successful!");
    }


    public int ChooseUserCurrencies(string currencyType)
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (currencyType.Equals("Coin"))
        {
            return user.coin;
        }
        else if (currencyType.Equals("Diamond"))
        {
            return user.diamond;
        }
        else if (currencyType.Equals("Gem"))
        {
            return user.gem;
        }
        else
        {
            return user.coin; // Default to coin if no match
        }

    }
}
