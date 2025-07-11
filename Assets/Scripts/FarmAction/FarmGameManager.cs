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
    }
}