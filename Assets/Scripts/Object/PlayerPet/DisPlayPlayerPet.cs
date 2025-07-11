using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisPlayPlayerPet : MonoBehaviour
{
    [SerializeField] public TMP_Text playerId;
    [SerializeField] public GameObject pet;
    [SerializeField] public Transform tranformPet;
    [SerializeField] private string defaultImageUrl = "https://drive.google.com/uc?id=1fsJXvABMVtfGSPJz7E-_yhqv0H7Fo8oS";

    public void Start()
    {
        // Kiểm tra các tham chiếu cần thiết khi khởi tạo
        if (pet == null)
            Debug.LogError("Prefab pet chưa được gán trong Inspector");

        if (tranformPet == null)
            Debug.LogError("Transform parent chưa được gán trong Inspector");

        if (playerId == null)
            Debug.LogError("Player ID Text chưa được gán trong Inspector");
    }

    public void DisplayListPet()
    {
        // Kiểm tra tham chiếu trước khi thực hiện
        if (pet == null || tranformPet == null || playerId == null)
        {
            Debug.LogError("Thiếu tham chiếu cần thiết trong DisPlayPlayerPet");
            return;
        }

        // 1. Extract numeric playerId
        string[] parts = playerId.text.Split(':');
        if (parts.Length < 2)
        {
            Debug.LogError("Định dạng Player ID không đúng. Cần có dạng 'Id:123'");
            return;
        }

        string idStr = parts[1].Trim();
        Debug.Log("Player ID: " + idStr);
        int playerIdValue;

        if (!int.TryParse(idStr, out playerIdValue))
        {
            Debug.LogError("Không thể chuyển đổi ID thành số: " + idStr);
            return;
        }

        // 2. Get player pet list from API
        List<PlayerPet> playerPets = APIPlayerPet.GetPlayerPetByPlayerId(playerIdValue);
        if (playerPets == null || playerPets.Count == 0)
        {
            Debug.LogWarning("Không tìm thấy pet nào cho player ID: " + idStr);
            return;
        }

        Debug.Log($"Tìm thấy {playerPets.Count} pet cho player ID {idStr}");

        // 3. Remove old pets if needed
        ClearOldPets();

        // 4. Create new pets
        CreatePetObjects(playerPets);
    }

    private void ClearOldPets()
    {
        if (tranformPet == null) return;

        int childCount = tranformPet.childCount;
        Debug.Log($"Xóa {childCount} pet cũ");

        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(tranformPet.GetChild(i).gameObject);
        }
    }

    private void CreatePetObjects(List<PlayerPet> playerPets)
    {
        foreach (var playerPet in playerPets)
        {
            // Clone pet GameObject
            GameObject petObj = Instantiate(pet, tranformPet);
            petObj.SetActive(true);

            Debug.Log($"Tạo pet: {playerPet.petCustomName}, Pet ID: {playerPet.petID}, Level: {playerPet.level}");

            // Thiết lập thông tin pet
            SetupPetUI(petObj, playerPet);
        }
    }

    private void SetupPetUI(GameObject petObj, PlayerPet playerPet)
    {
        // Đặt tên cho GameObject để dễ debug
        petObj.name = $"Pet_{playerPet.petCustomName}_{playerPet.playerPetID}";

        // Thiết lập Pet Name
        var nameTextObj = petObj.transform.Find("Name_Player_Pet");
        if (nameTextObj != null)
        {
            nameTextObj.gameObject.SetActive(true);
            var nameText = nameTextObj.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.enabled = true; // Đảm bảo component TMP_Text được enabled
                nameText.text = playerPet.petCustomName;
                Debug.Log($"Đã thiết lập tên: {playerPet.petCustomName}");
            }
            else
            {
                Debug.LogWarning($"GameObject Name_Player_Pet không có component TMP_Text");
            }
        }
        else
        {
            Debug.LogError($"Không tìm thấy GameObject 'Name_Player_Pet' trong prefab pet");
        }

        // Thiết lập Level
        var levelTextObj = petObj.transform.Find("Level");
        if (levelTextObj != null)
        {
            levelTextObj.gameObject.SetActive(true);
            var levelText = levelTextObj.GetComponent<TMP_Text>();
            if (levelText != null)
            {
                levelText.enabled = true; // Đảm bảo component TMP_Text được enabled
                levelText.text = playerPet.level.ToString();
                Debug.Log($"Đã thiết lập level: {playerPet.level}");
            }
            else
            {
                Debug.LogWarning($"GameObject Level không có component TMP_Text");
            }
        }
        else
        {
            Debug.LogError($"Không tìm thấy GameObject 'Level' trong prefab pet");
        }

        // Thiết lập Frame_Avatar (nếu có)
        var frameAvatarObj = petObj.transform.Find("Frame_Avatar");
        if (frameAvatarObj != null)
        {
            frameAvatarObj.gameObject.SetActive(true);
            var frameImage = frameAvatarObj.GetComponent<Image>();
            if (frameImage != null)
            {
                frameImage.enabled = true; // Đảm bảo component Image được enabled
            }
        }

        // Thiết lập Avatar
        var avatarObj = petObj.transform.Find("Avatar");
        if (avatarObj != null)
        {
            avatarObj.gameObject.SetActive(true);
            var avatarImage = avatarObj.GetComponent<Image>();
            if (avatarImage != null)
            {
                avatarImage.enabled = true; // Đảm bảo component Image được enabled
                LoadPetAvatar(playerPet, avatarImage);
            }
            else
            {
                Debug.LogWarning($"GameObject Avatar không có component Image");
                // Thử thêm component Image nếu không có
                avatarImage = avatarObj.gameObject.AddComponent<Image>();
                if (avatarImage != null)
                {
                    Debug.Log("Đã thêm component Image vào Avatar");
                    LoadPetAvatar(playerPet, avatarImage);
                }
            }
        }
        else
        {
            Debug.LogError($"Không tìm thấy GameObject 'Avatar' trong prefab pet");
        }
    }



    private void LoadPetAvatar(PlayerPet playerPet, Image avatarImage)
    {
        // Trước tiên thử lấy từ ShopProduct
        ShopProduct shopProduct = APIShopProduct.GetShopProductByIdPet(playerPet.petID);

        // Sử dụng URL từ ShopProduct nếu có, nếu không dùng mặc định
        string imageUrl = defaultImageUrl;

        if (shopProduct != null && !string.IsNullOrEmpty(shopProduct.imageUrl))
        {
            imageUrl = shopProduct.imageUrl.Trim();
            Debug.Log($"Pet ID {playerPet.petID}: Dùng URL từ API: {imageUrl}");
        }
        else
        {
            Debug.Log($"Pet ID {playerPet.petID}: Dùng URL mặc định: {imageUrl}");
        }

        // Đảm bảo URL bắt đầu bằng http:// hoặc https://
        if (!imageUrl.StartsWith("http://") && !imageUrl.StartsWith("https://"))
        {
            imageUrl = "https://" + imageUrl;
        }

        Debug.Log($"Pet ID {playerPet.petID}: URL cuối cùng: {imageUrl}");

        // Đặt placeholder trước khi tải
        avatarImage.color = Color.white;

        // Tải hình ảnh
        StartCoroutine(LoadImageSafe(imageUrl, avatarImage, playerPet.petID));
    }

    // Helper method to load image safely
    private System.Collections.IEnumerator LoadImageSafe(string url, Image image, int petId)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError($"Pet ID {petId}: URL trống, không thể tải hình ảnh");
            yield break;
        }

        Debug.Log($"Pet ID {petId}: Bắt đầu tải hình ảnh từ {url}");

        using (var www = new UnityEngine.Networking.UnityWebRequest(url))
        {
            www.downloadHandler = new UnityEngine.Networking.DownloadHandlerTexture();
            yield return www.SendWebRequest();

            if (www.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                var texture = ((UnityEngine.Networking.DownloadHandlerTexture)www.downloadHandler).texture;
                if (texture != null)
                {
                    // Tạo và thiết lập sprite
                    image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                    image.preserveAspect = true; // Giữ tỉ lệ hình ảnh
                    Debug.Log($"Pet ID {petId}: Tải hình ảnh thành công");
                }
                else
                {
                    Debug.LogError($"Pet ID {petId}: Texture null sau khi tải");
                    TryLoadDefaultImage(image, petId);
                }
            }
            else
            {
                Debug.LogError($"Pet ID {petId}: Lỗi khi tải hình ảnh: {www.error}");
                TryLoadDefaultImage(image, petId);
            }
        }
    }

    private void TryLoadDefaultImage(Image image, int petId)
    {
        // Chỉ thử tải ảnh mặc định nếu URL hiện tại không phải URL mặc định
        if (defaultImageUrl != null && defaultImageUrl != "")
        {
            Debug.Log($"Pet ID {petId}: Thử lại với URL mặc định");
            StartCoroutine(LoadImageSafe(defaultImageUrl, image, petId));
        }
        else
        {
            Debug.LogError($"Pet ID {petId}: Không có URL mặc định để thử lại");
        }
    }

    // Phương thức để kiểm tra các component của prefab pet
    public void ValidatePetPrefab()
    {
        if (pet == null)
        {
            Debug.LogError("Pet prefab chưa được gán!");
            return;
        }

        Debug.Log($"Đang kiểm tra prefab: {pet.name}");

        // Kiểm tra Name_Player_Pet
        var nameObj = pet.transform.Find("Name_Player_Pet");
        if (nameObj == null)
            Debug.LogError("Prefab không có GameObject con 'Name_Player_Pet'");
        else if (nameObj.GetComponent<TMP_Text>() == null)
            Debug.LogError("GameObject 'Name_Player_Pet' không có component TMP_Text");

        // Kiểm tra Level
        var levelObj = pet.transform.Find("Level");
        if (levelObj == null)
            Debug.LogError("Prefab không có GameObject con 'Level'");
        else if (levelObj.GetComponent<TMP_Text>() == null)
            Debug.LogError("GameObject 'Level' không có component TMP_Text");

        // Kiểm tra Avatar
        var avatarObj = pet.transform.Find("Avatar");
        if (avatarObj == null)
            Debug.LogError("Prefab không có GameObject con 'Avatar'");
        else if (avatarObj.GetComponent<Image>() == null)
            Debug.LogError("GameObject 'Avatar' không có component Image");

        Debug.Log("Kiểm tra prefab pet hoàn tất");
    }
}
