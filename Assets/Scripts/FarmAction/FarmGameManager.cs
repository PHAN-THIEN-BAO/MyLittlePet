//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

//public class FarmGameManager : MonoBehaviour






using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FarmGameManager : MonoBehaviour
{
    public static FarmGameManager instance;

    public TileManager tileManager;
    public GemSpawner gemSpawner;

    public GameObject harvestedItemPrefab;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        tileManager = GetComponent<TileManager>();

        if (gemSpawner == null)
        {
            gemSpawner = FindObjectOfType<GemSpawner>();
        }
    }

    private void Start()
    {
        if (gemSpawner != null)
        {
            gemSpawner.SpawnRandomGems();
        }
    }

    public void RespawnGems(int amount = -1)
    {
        if (gemSpawner != null)
        {
            if (amount > 0)
            {
                gemSpawner.SpawnSpecificAmount(amount);
            }
            else
            {
                gemSpawner.SpawnRandomGems();
            }
        }
        else
        {
            Debug.LogWarning("Không tìm th?y GemSpawner!");
        }
    }
    public void CollectItem(Player player, CollectableItems.ItemType itemType)
    {
        switch (itemType)
        {
            case CollectableItems.ItemType.Gem:
                AddCurrency("gem", 5);
                Debug.Log("Ðã thu th?p 5 Gem");
                break;

            case CollectableItems.ItemType.CarrotSeed:
                player.numCarrotSeed++;
                Debug.Log("Ðã thu th?p h?t gi?ng cà r?t. T?ng s?: " + player.numCarrotSeed);
                break;

            case CollectableItems.ItemType.PotatoSeed:
                Debug.Log("Ðã thu th?p h?t gi?ng khoai tây");
                break;

            case CollectableItems.ItemType.TomatoSeed:
                Debug.Log("Ðã thu th?p h?t gi?ng cà chua");
                break;

            default:
                Debug.Log("Thu th?p v?t ph?m không xác d?nh");
                break;
        }

        if (itemType == CollectableItems.ItemType.Gem)
        {
            UpdateCurrencyUI();
            SaveUserData();
        }
    }

    public void AddCurrency(string currencyType, int amount)
    {
        PlayerInfomation.UpdatePlayerInfo(user => {
            switch (currencyType.ToLower())
            {
                case "gem":
                    user.gem += amount;
                    Debug.Log($"Ðã c?p nh?t Gem: {user.gem}");
                    break;
                case "diamond":
                    user.diamond += amount;
                    Debug.Log($"Ðã c?p nh?t Diamond: {user.diamond}");
                    break;
                case "coin":
                    user.coin += amount;
                    Debug.Log($"Ðã c?p nh?t Coin: {user.coin}");
                    break;
            }
        });
    }

    public void UpdateCurrencyUI()
    {
        PlayerInfoMainScene playerInfoUI = FindObjectOfType<PlayerInfoMainScene>();
        if (playerInfoUI != null)
        {
            playerInfoUI.UpdateUI();
        }
    }

    public void SaveUserData()
    {
        bool success = APIUser.UpdateUser();
        if (success)
        {
            Debug.Log("Ðã luu d? li?u ngu?i dùng vào database thành công");
        }
        else
        {
            Debug.LogWarning("Không th? luu d? li?u ngu?i dùng vào database");
        }
    }
}