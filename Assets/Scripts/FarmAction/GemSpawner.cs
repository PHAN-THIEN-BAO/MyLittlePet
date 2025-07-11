using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    [Header("Cài đặt Gem")]
    [SerializeField] private GameObject gemPrefab; // Prefab của gem sẽ spawn
    [SerializeField] private int minGemAmount = 30; // Tăng số lượng gem tối thiểu lên 15 (từ 5)
    [SerializeField] private int maxGemAmount = 50; // Tăng số lượng gem tối đa lên 30 (từ 15)

    [Header("Khu vực Spawn")]
    [SerializeField] private float minX = -5f; // Thu nhỏ phạm vi
    [SerializeField] private float maxX = 5f;  // Thu nhỏ phạm vi
    [SerializeField] private float minY = -5f; // Thu nhỏ phạm vi
    [SerializeField] private float maxY = 5f;  // Thu nhỏ phạm vi

    [Header("Tùy chọn nâng cao")]
    [SerializeField] private float minDistanceFromPlayer = 1f; // Giảm khoảng cách tối thiểu xuống 1f (từ 2f)
    [SerializeField] private float minDistanceBetweenGems = 0.8f; // Giảm khoảng cách giữa các gem xuống 0.8f (từ 1f)
    [SerializeField] private bool spawnOnAwake = true; // Tự động spawn khi khởi tạo
    [SerializeField] private bool respawnWhenCollected = true; // Bật tính năng respawn khi gem được thu thập
    [SerializeField] private float respawnDelay = 15f; // Giảm thời gian delay xuống 15 giây (từ 30 giây)

    private List<Vector3> spawnedPositions = new List<Vector3>(); // Danh sách vị trí đã spawn gem
    private int currentGemCount; // Số lượng gem hiện tại trên bản đồ
    private int targetGemCount; // Số lượng gem cần spawn

    private void Awake()
    {
        if (spawnOnAwake)
        {
            SpawnRandomGems();
        }
    }

    // Thêm phương thức Update để có thể test lại việc spawn gem với phím R
    private void Update()
    {
        // Nhấn phím R để respawn tất cả gem (để test)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Đang spawn lại gem...");
            SpawnRandomGems();
        }
    }

    // Các phương thức còn lại giữ nguyên...
    // Spawn số lượng gem ngẫu nhiên trong khoảng minGemAmount đến maxGemAmount
    public void SpawnRandomGems()
    {
        // Kiểm tra prefab
        if (gemPrefab == null)
        {
            Debug.LogError("Gem Prefab chưa được gán! Vui lòng gán prefab vào GemSpawner.");
            return;
        }

        Debug.Log("Bắt đầu spawn gem...");

        // Xóa tất cả gem hiện có nếu cần
        ClearAllGems();

        // Xác định số lượng gem cần spawn
        targetGemCount = Random.Range(minGemAmount, maxGemAmount + 1);
        Debug.Log($"Đang spawn {targetGemCount} gem trên bản đồ");

        // Spawn từng gem một
        for (int i = 0; i < targetGemCount; i++)
        {
            SpawnSingleGem();
        }

        currentGemCount = targetGemCount;
    }

    // Spawn một gem tại vị trí ngẫu nhiên
    private void SpawnSingleGem()
    {
        Vector3 spawnPos;
        int maxAttempts = 50; // Giới hạn số lần thử để tránh vòng lặp vô tận
        int attempts = 0;

        do
        {
            // Tạo vị trí ngẫu nhiên
            spawnPos = new Vector3(
                Random.Range(minX, maxX),
                Random.Range(minY, maxY),
                0f
            );
            attempts++;

            // Nếu đã thử quá nhiều lần mà không tìm được vị trí phù hợp, thoát vòng lặp
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Không thể tìm vị trí phù hợp để spawn gem sau " + maxAttempts + " lần thử");
                return;
            }
        }
        // Kiểm tra khoảng cách từ người chơi và các gem khác
        while (!IsValidSpawnPosition(spawnPos));

        // Tạo gem tại vị trí đã chọn
        GameObject newGem = Instantiate(gemPrefab, spawnPos, Quaternion.identity);
        newGem.transform.parent = this.transform; // Đặt gem làm con của spawner để quản lý dễ dàng

        Debug.Log($"Đã spawn gem tại vị trí: {spawnPos}");

        // Theo dõi vị trí đã spawn
        spawnedPositions.Add(spawnPos);

        // Đăng ký sự kiện khi gem bị destroy (nếu cần respawn)
        if (respawnWhenCollected)
        {
            CollectableItems collectableComponent = newGem.GetComponent<CollectableItems>();
            if (collectableComponent != null)
            {
                // Đăng ký theo dõi khi gem bị destroy
                StartCoroutine(WaitForGemDestroy(newGem));
            }
        }
    }

    // Kiểm tra xem vị trí có phù hợp để spawn gem không
    private bool IsValidSpawnPosition(Vector3 position)
    {
        // Kiểm tra khoảng cách từ người chơi
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            if (Vector3.Distance(position, player.transform.position) < minDistanceFromPlayer)
            {
                return false;
            }
        }

        // Kiểm tra khoảng cách từ các gem khác
        foreach (Vector3 existingPos in spawnedPositions)
        {
            if (Vector3.Distance(position, existingPos) < minDistanceBetweenGems)
            {
                return false;
            }
        }

        return true;
    }

    // Theo dõi khi gem bị destroy để respawn nếu cần
    private IEnumerator WaitForGemDestroy(GameObject gem)
    {
        // Chờ cho đến khi gem bị destroy
        while (gem != null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // Gem đã bị destroy, giảm số lượng hiện tại
        currentGemCount--;

        // Chờ thời gian delay trước khi respawn
        yield return new WaitForSeconds(respawnDelay);

        // Respawn gem nếu cần
        if (currentGemCount < targetGemCount)
        {
            SpawnSingleGem();
            currentGemCount++;
        }
    }

    // Xóa tất cả gem hiện có
    public void ClearAllGems()
    {
        // Xóa tất cả gem là con của spawner này
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        spawnedPositions.Clear();
        currentGemCount = 0;
    }

    // Phương thức này có thể được gọi từ Inspector hoặc các script khác
    public void RespawnAllGems()
    {
        SpawnRandomGems();
    }

    // Phương thức để spawn một lượng gem cụ thể
    public void SpawnSpecificAmount(int amount)
    {
        minGemAmount = amount;
        maxGemAmount = amount;
        SpawnRandomGems();
    }
}