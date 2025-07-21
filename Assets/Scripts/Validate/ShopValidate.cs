using UnityEngine;
using TMPro;
using UnityEngine.UI;
using NUnit.Framework;
using System.Collections.Generic;
public class ShopValidate : MonoBehaviour
{
    public bool CheckCanBuy(GameObject notEnoughMoneyPanel)
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        if (user == null)
        {
            Debug.LogError("User not found.");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true);
            return false;
        }
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
        ShopProduct product = APIShopProduct.GetShopProductById(shopProductID);
        if (product == null)
        {
            Debug.LogError("Product not found.");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true);
            return false;
        }
        int userCurrency = 0;
        if (product.currencyType == "Coin")
            userCurrency = user.coin;
        else if (product.currencyType == "Diamond")
            userCurrency = user.diamond;
        else if (product.currencyType == "Gem")
            userCurrency = user.gem;
        int quantity = 1;
        bool canBuy = CurenciesValidation.ValidateCurrencies(userCurrency, product.price, quantity);
        if (!canBuy)
        {
            Debug.LogWarning("Not enough money to buy!");
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(true);
            return false;
        }
        else
        {
            if (notEnoughMoneyPanel != null)
                notEnoughMoneyPanel.SetActive(false);
            return true;
        }
    }
    public bool CheckCanBuyPet(GameObject OwnedPetPanel, int petId, int playerId)
    {
        PlayerPet playerPet = APIPlayerPet.GetPlayerPetByPlayerIdAndPetId(playerId ,petId);
        if (playerPet == null)
        {
            Debug.Log("Player pet not found, ok can buy");
            OwnedPetPanel.SetActive(false);
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