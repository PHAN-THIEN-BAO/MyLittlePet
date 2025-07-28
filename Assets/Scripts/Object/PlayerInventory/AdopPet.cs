using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class AdopPet : MonoBehaviour
{
    [SerializeField] GameObject adopPetSuccessPanel;
    [SerializeField] GameObject adopPetFailPanel;
    public GameObject petPrefab;
    public PetController petController;

    public void IsAdopPetSuccess()
    {
        Transform itemTransform = transform.parent;

        TMP_Text quantityText = itemTransform.Find("Quantity").GetComponent<TMP_Text>();
        TMP_Text shopProductIdText = itemTransform.Find("Shop_Product_Id").GetComponent<TMP_Text>();
        TMP_Text playerIdText = itemTransform.Find("Player_Id").GetComponent<TMP_Text>();
        TMP_InputField nameInput = itemTransform.Find("Name_Input").GetComponent<TMP_InputField>();

        int quantity = int.Parse(quantityText.text);
        int actualShopProductId = int.Parse(shopProductIdText.text);
        int playerId = int.Parse(playerIdText.text);
        string customNamePet = nameInput.text;

        ShopProduct shopProduct = APIShopProduct.GetShopProductById(actualShopProductId);
        if (shopProduct == null || !shopProduct.petID.HasValue)
        {
            adopPetFailPanel.SetActive(true);
            return;
        }
        PlayerPet newPlayerPet = new PlayerPet
        {
            playerID = playerId,
            petID = shopProduct.petID.Value,
            petCustomName = customNamePet,
            status = "50%2550%2550"
            
        };

        StartCoroutine(APIPlayerPet.AddPlayerPetCoroutine(newPlayerPet, (createdPet) =>
        {
            if (createdPet != null)
            {
                adopPetSuccessPanel.SetActive(true);
                
                if (petController != null)
                {
                    petController.SpawnPet(createdPet);
                }

                PlayerInventory playerInventory = new PlayerInventory
                {
                    playerID = playerId,
                    shopProductID = actualShopProductId,
                    quantity = quantity
                };

                if (quantity > 1)
                {
                    playerInventory.quantity = quantity - 1;
                    StartCoroutine(APIPlayerInventory.UpdatePlayerInventoryCoroutine(playerInventory, null));
                }
                else
                {
                    StartCoroutine(APIPlayerInventory.DeletePlayerInventoryCoroutine(playerId, actualShopProductId, null));
                }
            }
            else
            {
                adopPetFailPanel.SetActive(true);
            }
        }));
    }
}