using UnityEngine;
using TMPro;
public class BuyItem : MonoBehaviour
{
    [SerializeField] public GameObject notEnoughMoneyPanel;
    public void BuyProduct()
    {
        ShopValidate shopValidate = GetComponent<ShopValidate>();
        if (shopValidate != null && !shopValidate.CheckCanBuy(notEnoughMoneyPanel))
        {
            return;
        }
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
        Transform idPetTransform = transform.parent.Find("Pet_Id");
        if (idPetTransform == null)
        {
            Debug.LogError($"Pet_Id not found as sibling of {gameObject.name}");
            return;
        }
        int shopProductID = int.Parse(idText.text);
        ShopProduct product = APIShopProduct.GetShopProductById(shopProductID);
        if (product == null)
        {
            Debug.LogError("Product not found.");
            return;
        }
        int quantity = 1;
        int userCurrency = ChooseUserCurrencies(product.currencyType);
        bool canBuy = CurenciesValidation.ValidateCurrencies(userCurrency, product.price, quantity);
        if (!canBuy)
        {
            Debug.LogWarning("Not enough currency to buy this product.");
            return;
        }
        PlayerInfomation.UpdatePlayerInfo(u =>
        {
            if (product.currencyType == "Coin")
                u.coin -= product.price * quantity;
            else if (product.currencyType == "Diamond")
                u.diamond -= product.price * quantity;
            else if (product.currencyType == "Gem")
                u.gem -= product.price * quantity;
        });
        APIUser.UpdateUser();
        Debug.Log("Purchase successful!");
        PlayerInventory playerInventory = new PlayerInventory
        {
            playerID = user.id,
            shopProductID = shopProductID,
            quantity = 1
        };
        StartCoroutine(APIPlayerInventory.AddPlayerInventoryCoroutine(playerInventory, (success) =>
        {
            if (success)
                Debug.Log("Inventory updated!");
            else
                Debug.LogWarning("Failed to update inventory.");
        }));
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
            return user.coin;
        }
    }
}