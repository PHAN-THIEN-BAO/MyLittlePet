//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

//public class FarmGameManager : MonoBehaviour
//{
//    public static FarmGameManager instance; // Singleton instance

//    public TileManager tileManager; // Reference to the TileManager

//    // Prefab cho vật phẩm thu hoạch
//    public GameObject harvestedItemPrefab;

//    private void Awake()
//    {
//        // Ensure only one instance of FarmGameManager exists
//        if (instance != null && instance != this)
//        {
//            Destroy(this.gameObject);
//        }
//        else
//        {
//            instance = this;
//        }

//        DontDestroyOnLoad(this.gameObject); // Keep FarmGameManager alive across scenes

//        tileManager = GetComponent<TileManager>();
//    }
//}

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FarmGameManager : MonoBehaviour
{
    public static FarmGameManager instance; // Singleton instance

    public TileManager tileManager; // Reference to the TileManager
    public GemSpawner gemSpawner; // Thêm tham chiếu tới GemSpawner

    // Prefab cho vật phẩm thu hoạch
    public GameObject harvestedItemPrefab;

    private void Awake()
    {
        // Ensure only one instance of FarmGameManager exists
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject); // Keep FarmGameManager alive across scenes

        tileManager = GetComponent<TileManager>();

        // Tìm GemSpawner nếu chưa được gán
        if (gemSpawner == null)
        {
            gemSpawner = FindObjectOfType<GemSpawner>();
        }
    }

    // Khởi tạo game
    private void Start()
    {
        // Spawn gem nếu có GemSpawner
        if (gemSpawner != null)
        {
            gemSpawner.SpawnRandomGems();
        }
    }

    // Phương thức để spawn lại gem theo yêu cầu
    public void RespawnGems(int amount = -1)
    {
        if (gemSpawner != null)
        {
            if (amount > 0)
            {
                // Spawn số lượng gem cụ thể
                gemSpawner.SpawnSpecificAmount(amount);
            }
            else
            {
                // Spawn số lượng ngẫu nhiên theo cài đặt
                gemSpawner.SpawnRandomGems();
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy GemSpawner!");
        }
    }// Thêm vật phẩm vào người chơi dựa trên loại
    public void CollectItem(Player player, CollectableItems.ItemType itemType)
    {
        switch (itemType)
        {
            case CollectableItems.ItemType.Gem:
                // Cập nhật số lượng Gem trong dữ liệu người dùng
                AddCurrency("gem", 5);
                Debug.Log("Đã thu thập 5 Gem");
                break;

            case CollectableItems.ItemType.CarrotSeed:
                player.numCarrotSeed++;
                Debug.Log("Đã thu thập hạt giống cà rốt. Tổng số: " + player.numCarrotSeed);
                break;

            // Bạn có thể thêm các loại vật phẩm khác ở đây
            case CollectableItems.ItemType.PotatoSeed:
                // Giả sử có biến numPotatoSeed trong lớp Player
                // player.numPotatoSeed++;
                Debug.Log("Đã thu thập hạt giống khoai tây");
                break;

            case CollectableItems.ItemType.TomatoSeed:
                // Giả sử có biến numTomatoSeed trong lớp Player
                // player.numTomatoSeed++;
                Debug.Log("Đã thu thập hạt giống cà chua");
                break;

            default:
                Debug.Log("Thu thập vật phẩm không xác định");
                break;
        }

        // Nếu là gem, cập nhật giao diện hiển thị tiền tệ
        if (itemType == CollectableItems.ItemType.Gem)
        {
            UpdateCurrencyUI();
            SaveUserData();
        }
    }

    // Cập nhật loại tiền tệ trong dữ liệu người dùng
    public void AddCurrency(string currencyType, int amount)
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
    public void UpdateCurrencyUI()
    {
        // Tìm PlayerInfoMainScene để cập nhật thông tin tiền tệ
        PlayerInfoMainScene playerInfoUI = FindObjectOfType<PlayerInfoMainScene>();
        if (playerInfoUI != null)
        {
            playerInfoUI.UpdateUI(); // Cập nhật hiển thị tiền tệ trên UI
        }
    }

    // Lưu dữ liệu người dùng vào database
    public void SaveUserData()
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
}