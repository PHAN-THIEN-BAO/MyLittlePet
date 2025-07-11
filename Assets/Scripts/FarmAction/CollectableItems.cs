using UnityEngine;

[RequireComponent(typeof(Item))]
public class CollectableItems : MonoBehaviour
{
    [Header("Loại Vật Phẩm")]
    [SerializeField] private ItemType itemType = ItemType.Gem;

    [Header("Hiệu Ứng")]
    [SerializeField] private bool playSound = true;
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private float destroyDelay = 0.2f;

    // Enum định nghĩa các loại vật phẩm có thể thu thập
    public enum ItemType
    {
        Gem,
        CarrotSeed,
        PotatoSeed,
        TomatoSeed,
        // Thêm các loại vật phẩm khác tại đây
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là người chơi không
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            // Thêm vật phẩm vào túi đồ của người chơi
            CollectItem(player);

            // Phát hiệu ứng nếu được cài đặt
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }

            // Phát âm thanh nếu được bật
            if (playSound && collectSound != null)
            {
                // Tạo GameObject tạm thời để phát âm thanh với âm lượng lớn hơn
                GameObject tempGO = new GameObject("TempAudio");
                tempGO.transform.position = transform.position;
                AudioSource audioSource = tempGO.AddComponent<AudioSource>();
                audioSource.clip = collectSound;
                audioSource.volume = 1.0f; // Đặt âm lượng tối đa
                audioSource.spatialBlend = 0f; // 0 = 2D sound (không gian hóa)
                audioSource.Play();

                // Xóa GameObject tạm thời sau khi âm thanh phát xong
                Destroy(tempGO, collectSound.length);
            }

            // Xóa vật phẩm sau một khoảng thời gian ngắn
            Destroy(gameObject, destroyDelay);
        }
    }

    // Thêm vật phẩm vào người chơi dựa trên loại
    private void CollectItem(Player player)
    {
        switch (itemType)
        {
            case ItemType.Gem:
                // Cập nhật số lượng Gem trong dữ liệu người dùng
                AddCurrency("gem", 5);
                Debug.Log("Đã thu thập 5 Gem");
                break;

            case ItemType.CarrotSeed:
                player.numCarrotSeed++;
                Debug.Log("Đã thu thập hạt giống cà rốt. Tổng số: " + player.numCarrotSeed);
                break;

            // Bạn có thể thêm các loại vật phẩm khác ở đây
            // case ItemType.PotatoSeed:
            //     player.numPotatoSeed++;
            //     Debug.Log("Đã thu thập hạt giống khoai tây. Tổng số: " + player.numPotatoSeed);
            //     break;

            default:
                Debug.Log("Thu thập vật phẩm không xác định");
                break;
        }

        // Nếu là gem, cập nhật giao diện hiển thị tiền tệ
        if (itemType == ItemType.Gem)
        {
            UpdateCurrencyUI();
            SaveUserData();
        }
    }

    // Cập nhật loại tiền tệ trong dữ liệu người dùng
    private void AddCurrency(string currencyType, int amount)
    {
        PlayerInfomation.UpdatePlayerInfo(user => {
            // Cập nhật loại tiền tương ứng
            switch (currencyType.ToLower())
            {
                case "gem":
                    user.gem += amount;
                    Debug.Log($"Đã cập nhật Gem: {user.gem}");
                    break;
                case "diamond":
                    user.diamond += amount;
                    Debug.Log($"Đã cập nhật Diamond: {user.diamond}");
                    break;
                case "coin":
                    user.coin += amount;
                    Debug.Log($"Đã cập nhật Coin: {user.coin}");
                    break;
            }
        });
    }

    // Cập nhật giao diện hiển thị tiền tệ
    private void UpdateCurrencyUI()
    {
        // Tìm PlayerInfoMainScene để cập nhật thông tin tiền tệ
        PlayerInfoMainScene playerInfoUI = FindObjectOfType<PlayerInfoMainScene>();
        if (playerInfoUI != null)
        {
            playerInfoUI.UpdateUI(); // Cập nhật hiển thị tiền tệ trên UI
        }
    }

    // Lưu dữ liệu người dùng vào database
    private void SaveUserData()
    {
        bool success = APIUser.UpdateUser();
        if (success)
        {
            Debug.Log("Đã lưu dữ liệu người dùng vào database thành công");
        }
        else
        {
            Debug.LogWarning("Không thể lưu dữ liệu người dùng vào database");
        }
    }

    // Phương thức này có thể được gọi từ các sự kiện khác (như nhấp chuột)
    public void Collect(Player player)
    {
        if (player != null)
        {
            CollectItem(player);
            Destroy(gameObject);
        }
    }
}