using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class AdopPet : MonoBehaviour
{
    [SerializeField] GameObject adopPetSuccessPanel;
    [SerializeField] GameObject adopPetFailPanel;
    public GameObject petPrefab; // Prefab to spawn upon successful adoption
    public PetController petController; // Kéo PetController trong scene vào Inspector

    public void IsAdopPetSuccess()
    {
        // Lấy gameObject cha (Item prefab)
        Transform itemTransform = transform.parent;

        // Lấy thông tin từ các Text components
        TMP_Text quantityText = itemTransform.Find("Quantity").GetComponent<TMP_Text>();
        TMP_Text shopProductIdText = itemTransform.Find("Shop_Product_Id").GetComponent<TMP_Text>();
        TMP_Text playerIdText = itemTransform.Find("Player_Id").GetComponent<TMP_Text>();
        TMP_InputField nameInput = itemTransform.Find("Name_Input").GetComponent<TMP_InputField>(); // Thêm dòng này

        // Chuyển đổi text thành số
        int quantity = int.Parse(quantityText.text);
        int actualShopProductId = int.Parse(shopProductIdText.text);
        int playerId = int.Parse(playerIdText.text);
        string customNamePet = nameInput.text; // Đúng

        // Lấy thông tin ShopProduct để có PetID
        ShopProduct shopProduct = APIShopProduct.GetShopProductById(actualShopProductId);
        if (shopProduct == null || !shopProduct.petID.HasValue)
        {
            adopPetFailPanel.SetActive(true);
            return;
        }
        // Tạo PlayerPet mới
        PlayerPet newPlayerPet = new PlayerPet
        {
            playerID = playerId,
            petID = shopProduct.petID.Value,
            petCustomName = customNamePet,
            status = "100%25100%25100",
            level = 1
        };


        // Thêm pet mới
        StartCoroutine(APIPlayerPet.AddPlayerPetCoroutine(newPlayerPet, (addResult) =>
        {
            if (addResult)
            {
                // Hiển thị panel thành công
                adopPetSuccessPanel.SetActive(true);

                // Spawn pet ngay lập tức
                if (petController != null)
                {
                    petController.SpawnPet(newPlayerPet);
                }

                // Tạo PlayerInventory object để update hoặc delete
                PlayerInventory playerInventory = new PlayerInventory
                {
                    playerID = playerId,
                    shopProductID = actualShopProductId,
                    quantity = quantity
                };

                if (quantity > 1)
                {
                    // Update số lượng
                    playerInventory.quantity = quantity - 1;
                    StartCoroutine(APIPlayerInventory.UpdatePlayerInventoryCoroutine(playerInventory, null));
                }
                else
                {
                    // Xóa item nếu quantity <= 1
                    StartCoroutine(APIPlayerInventory.DeletePlayerInventoryCoroutine(playerId, actualShopProductId, null));
                }
            }
            else
            {
                // Hiển thị panel thất bại
                adopPetFailPanel.SetActive(true);
            }
        }));

    }


    //// Spawn the pet GameObject
    //if (petPrefab != null)
    //{
    //    GameObject petObj = Instantiate(petPrefab, Vector3.zero, Quaternion.identity);
    //    PetController petController = petObj.GetComponent<PetController>();
    //    if (petController != null)
    //    {
    //        petController.playerPet = newPlayerPet;
    //    }
    //    // Lưu thông tin pet vào PlayerPrefs (ví dụ cho 1 pet)

    //    PlayerPrefs.SetFloat("SavedPetPosX", petObj.transform.position.x);
    //    PlayerPrefs.SetFloat("SavedPetPosY", petObj.transform.position.y);
    //    PlayerPrefs.SetFloat("SavedPetPosZ", petObj.transform.position.z);
    //    PlayerPrefs.Save();
    //}
}
