//using System.Collections.Generic;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class TileManager : MonoBehaviour
//{
//    [SerializeField] private Tilemap interactableMap; // Tilemap để quản lý các tile

//    // Các tile cho từng giai đoạn
//    [SerializeField] private Tile hiddenInteractableTile; // Tile ẩn ban đầu (đất chưa cày)
//    [SerializeField] private Tile interactedTile; // Tile đất đã cày
//    [SerializeField] private Tile plantedTile; // Tile đất đã trồng cây
//    [SerializeField] private Tile growingTile; // Tile cây đang lớn
//    [SerializeField] private Tile harvestReadyTile; // Tile cây đã sẵn sàng thu hoạch

//    // Thêm tilemap riêng cho highlight
//    [SerializeField] private Tilemap highlightMap; // Tilemap để hiển thị highlight
//    [SerializeField] private Tile highlightTile; // Tile dùng để hiển thị highlight

//    [Header("Highlight Settings")]
//    [SerializeField] private float highlightRadius = 2f;
//    [SerializeField] private Color defaultColor = Color.white;
//    [SerializeField] private Color soilColor = new Color(2f, 2f, 0f, 0.5f); // Màu vàng cho đất ban đầu
//    [SerializeField] private Color plowedColor = new Color(0f, 2f, 0f, 0.5f); // Màu xanh lá cho đất đã cày
//    [SerializeField] private Color harvestColor = new Color(2f, 0f, 0f, 0.5f); // Màu đỏ cho cây sẵn sàng thu hoạch
//    [SerializeField] private bool enableHighlight = true; // Bật/tắt hiệu ứng highlight

//    // Từ điển để theo dõi trạng thái của mỗi ô đất
//    private Dictionary<Vector3Int, int> tileStages = new Dictionary<Vector3Int, int>();
//    // Trạng thái: 0 = đất ban đầu, 1 = đất đã cày, 2 = đã trồng, 3 = đang lớn, 4 = sẵn sàng thu hoạch

//    private Player player;
//    private bool debugMode = true;

//    void Start()
//    {
//        // In ra tên của các tile để debug
//        Debug.Log("TileManager Start");
//        Debug.Log("Hidden Interactable Tile name: " + (hiddenInteractableTile != null ? hiddenInteractableTile.name : "NULL"));
//        Debug.Log("Interacted Tile name: " + (interactedTile != null ? interactedTile.name : "NULL"));
//        Debug.Log("Highlight Tile name: " + (highlightTile != null ? highlightTile.name : "NULL"));

//        // Kiểm tra các component cần thiết
//        if (interactableMap == null)
//        {
//            Debug.LogError("interactableMap không được gán!");
//        }
//        if (highlightMap == null)
//        {
//            Debug.LogError("highlightMap không được gán! Tạo một Tilemap mới và gán vào Inspector.");
//        }
//        if (highlightTile == null)
//        {
//            Debug.LogError("highlightTile không được gán! Tạo một Tile mới và gán vào Inspector.");
//        }

//        // Thay thế tất cả các tile ban đầu trên map bằng hiddenInteractableTile
//        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
//        {
//            TileBase tile = interactableMap.GetTile(position);

//            // Kiểm tra nếu có tile tại vị trí này
//            if (tile != null)
//            {
//                // Thay thế tất cả các tile có sẵn trên map bằng hiddenInteractableTile
//                interactableMap.SetTile(position, hiddenInteractableTile);
//                tileStages[position] = 0; // Đặt trạng thái ban đầu
//            }
//        }

//        // Tìm player trong scene
//        player = FindObjectOfType<Player>();
//        if (player == null)
//        {
//            Debug.LogWarning("Không tìm thấy Player trong scene!");
//        }

//        // Test highlight một số tile để kiểm tra
//        StartCoroutine(TestHighlightAfterDelay());
//    }

//    // Test highlight sau một khoảng thời gian ngắn
//    private IEnumerator TestHighlightAfterDelay()
//    {
//        yield return new WaitForSeconds(1f);
//        TestHighlight();
//    }

//    // Test highlight để kiểm tra system
//    private void TestHighlight()
//    {
//        if (highlightMap == null || highlightTile == null) return;

//        Debug.Log("Test highlight...");
//        // Xóa highlight cũ
//        highlightMap.ClearAllTiles();

//        // Test highlight ở vị trí (0,0)
//        Vector3Int center = Vector3Int.zero;
//        highlightMap.SetTile(center, highlightTile);
//        highlightMap.SetColor(center, Color.red);
//        Debug.Log("Đã đặt highlight test tại " + center);

//        // Test highlight ở một vùng 3x3 xung quanh người chơi nếu có
//        if (player != null)
//        {
//            Vector3Int playerPos = interactableMap.WorldToCell(player.transform.position);
//            Debug.Log("Player position: " + player.transform.position + ", Cell: " + playerPos);

//            for (int x = -1; x <= 1; x++)
//            {
//                for (int y = -1; y <= 1; y++)
//                {
//                    Vector3Int pos = playerPos + new Vector3Int(x, y, 0);
//                    highlightMap.SetTile(pos, highlightTile);
//                    highlightMap.SetColor(pos, Color.yellow);
//                }
//            }
//            Debug.Log("Đã đặt highlight test quanh người chơi");
//        }
//    }

//    void Update()
//    {
//        // Tìm player nếu chưa có
//        if (player == null)
//        {
//            player = FindObjectOfType<Player>();
//            if (player == null) return;
//        }

//        // Test key để debug
//        if (Input.GetKeyDown(KeyCode.T))
//        {
//            TestHighlight();
//        }

//        if (Input.GetKeyDown(KeyCode.Y))
//        {
//            enableHighlight = !enableHighlight;
//            Debug.Log("Highlight " + (enableHighlight ? "bật" : "tắt"));
//            if (!enableHighlight)
//            {
//                highlightMap.ClearAllTiles();
//            }
//        }

//        // Chỉ cập nhật highlight nếu được bật
//        if (enableHighlight && highlightMap != null && highlightTile != null)
//        {
//            UpdateHighlight();
//        }
//    }

//    // Cập nhật highlight cho các tile gần người chơi
//    private void UpdateHighlight()
//    {
//        if (player == null) return;

//        // Xóa tất cả highlight hiện tại
//        highlightMap.ClearAllTiles();

//        Vector3Int playerCell = interactableMap.WorldToCell(player.transform.position);
//        if (debugMode)
//        {
//            Debug.Log("Player position: " + player.transform.position + ", Cell: " + playerCell);
//        }

//        int highlightCount = 0;

//        // Tìm và highlight các tile xung quanh người chơi
//        for (int x = -Mathf.FloorToInt(highlightRadius); x <= Mathf.FloorToInt(highlightRadius); x++)
//        {
//            for (int y = -Mathf.FloorToInt(highlightRadius); y <= Mathf.FloorToInt(highlightRadius); y++)
//            {
//                Vector3Int cellPos = playerCell + new Vector3Int(x, y, 0);

//                // Chỉ highlight các ô trong phạm vi hình tròn
//                if (Vector2.Distance(Vector2Int.zero, new Vector2Int(x, y)) <= highlightRadius)
//                {
//                    // Kiểm tra nếu tile tại vị trí này có thể tương tác
//                    if (IsInteractableTile(cellPos))
//                    {
//                        // Lấy trạng thái của tile
//                        int stage = 0;
//                        tileStages.TryGetValue(cellPos, out stage);

//                        // Chỉ highlight các tile có thể tương tác (giai đoạn 0, 1, hoặc 4)
//                        if (stage == 0 || stage == 1 || stage == 4)
//                        {
//                            // Đặt highlight tile
//                            highlightMap.SetTile(cellPos, highlightTile);

//                            // Chọn màu highlight tùy theo trạng thái
//                            Color highlightColor;
//                            switch (stage)
//                            {
//                                case 0: // Đất ban đầu
//                                    highlightColor = soilColor;
//                                    break;
//                                case 1: // Đất đã cày, có thể trồng
//                                    highlightColor = plowedColor;
//                                    break;
//                                case 4: // Cây sẵn sàng thu hoạch
//                                    highlightColor = harvestColor;
//                                    break;
//                                default:
//                                    highlightColor = defaultColor;
//                                    break;
//                            }

//                            // Đặt màu cho highlight tile
//                            highlightMap.SetColor(cellPos, highlightColor);
//                            highlightCount++;
//                        }
//                    }
//                }
//            }
//        }

//        if (debugMode)
//        {
//            Debug.Log("Đã highlight " + highlightCount + " tiles");
//        }
//    }

//    public bool IsInteractableTile(Vector3Int position)
//    {
//        // Kiểm tra xem tile tại vị trí có thể tương tác không
//        TileBase tile = interactableMap.GetTile(position);

//        // Bất kỳ tile nào cũng có thể tương tác
//        return tile != null;
//    }

//    // Phương thức để tương tác với tile
//    public void InteractWithTile(Vector3Int position)
//    {
//        if (!tileStages.ContainsKey(position))
//        {
//            tileStages[position] = 0;
//        }

//        // Lấy trạng thái hiện tại của tile
//        int currentStage = tileStages[position];

//        // Chuyển đổi sang trạng thái tiếp theo
//        switch (currentStage)
//        {
//            case 0: // Đất ban đầu -> Đất đã cày
//                interactableMap.SetTile(position, interactedTile);
//                tileStages[position] = 1;
//                Debug.Log("Đất đã được cày xới");
//                break;

//            case 1: // Đất đã cày -> Đã trồng cây
//                // Kiểm tra xem người chơi có hạt giống không
//                Player player = GameObject.FindObjectOfType<Player>();
//                if (player != null && player.numCarrotSeed > 0)
//                {
//                    player.numCarrotSeed--;
//                    interactableMap.SetTile(position, plantedTile);
//                    tileStages[position] = 2;
//                    Debug.Log("Đã trồng cây. Hạt giống còn lại: " + player.numCarrotSeed);

//                    // Bắt đầu quá trình tăng trưởng
//                    StartCoroutine(GrowPlant(position));
//                }
//                else
//                {
//                    Debug.Log("Không có đủ hạt giống để trồng");
//                }
//                break;

//            case 4: // Thu hoạch
//                // Sinh ra vật phẩm thu hoạch
//                SpawnHarvestedItem(position);

//                // Đặt lại về đất đã cày
//                interactableMap.SetTile(position, interactedTile);
//                tileStages[position] = 1;
//                Debug.Log("Đã thu hoạch thành công!");
//                break;

//            default:
//                Debug.Log("Cây đang phát triển, hãy đợi thêm...");
//                break;
//        }

//        // Cập nhật highlight ngay lập tức sau khi tương tác
//        if (enableHighlight && highlightMap != null && highlightTile != null)
//        {
//            UpdateHighlight();
//        }
//    }

//    // Coroutine để mô phỏng quá trình tăng trưởng của cây
//    private IEnumerator GrowPlant(Vector3Int position)
//    {
//        // Đợi một khoảng thời gian cho cây lớn lên
//        yield return new WaitForSeconds(10f); // 10 giây

//        // Chuyển sang trạng thái cây đang lớn
//        interactableMap.SetTile(position, growingTile);
//        tileStages[position] = 3;
//        Debug.Log("Cây đang phát triển...");

//        // Đợi thêm một khoảng thời gian nữa
//        yield return new WaitForSeconds(15f); // 15 giây

//        // Chuyển sang trạng thái sẵn sàng thu hoạch
//        interactableMap.SetTile(position, harvestReadyTile);
//        tileStages[position] = 4;
//        Debug.Log("Cây đã sẵn sàng để thu hoạch!");

//        // Cập nhật highlight ngay khi cây sẵn sàng thu hoạch
//        if (enableHighlight && highlightMap != null && highlightTile != null)
//        {
//            UpdateHighlight();
//        }
//    }

//    // Phương thức để sinh ra vật phẩm thu hoạch
//    private void SpawnHarvestedItem(Vector3Int position)
//    {
//        // Chuyển đổi từ vị trí tile sang vị trí thế giới
//        Vector3 worldPos = interactableMap.GetCellCenterWorld(position);

//        // Kiểm tra xem có prefab vật phẩm trong FarmGameManager không
//        if (FarmGameManager.instance != null && FarmGameManager.instance.harvestedItemPrefab != null)
//        {
//            Instantiate(FarmGameManager.instance.harvestedItemPrefab, worldPos, Quaternion.identity);
//        }
//        else
//        {
//            Debug.LogWarning("Không có prefab vật phẩm thu hoạch!");

//            // Tăng số lượng vật phẩm cho người chơi trực tiếp nếu không có prefab
//            Player player = GameObject.FindObjectOfType<Player>();
//            if (player != null)
//            {
//                player.numCarrotSeed += 2; // Thu được 2 hạt giống khi thu hoạch
//                Debug.Log("Đã thêm 2 hạt giống vào túi đồ. Tổng số: " + player.numCarrotSeed);
//            }
//        }
//    }

//    // Giữ lại phương thức cũ để tương thích ngược
//    public void SetTileInteractable(Vector3Int position)
//    {
//        InteractWithTile(position);
//    }

//    // Thêm để hiển thị phạm vi highlight trong Scene view
//    private void OnDrawGizmosSelected()
//    {
//        if (player != null)
//        {
//            Gizmos.color = Color.yellow;
//            Gizmos.DrawWireSphere(player.transform.position, highlightRadius);
//        }
//    }
//}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap; // Tilemap để quản lý các tile

    // Các tile cho từng giai đoạn
    [SerializeField] private Tile hiddenInteractableTile; // Tile ẩn ban đầu (đất chưa cày)
    [SerializeField] private Tile interactedTile; // Tile đất đã cày
    [SerializeField] private Tile plantedTile; // Tile đất đã trồng cây
    [SerializeField] private Tile growingTile; // Tile cây đang lớn
    [SerializeField] private Tile harvestReadyTile; // Tile cây đã sẵn sàng thu hoạch

    // Thêm tilemap riêng cho highlight
    [SerializeField] private Tilemap highlightMap; // Tilemap để hiển thị highlight
    [SerializeField] private Tile highlightTile; // Tile dùng để hiển thị highlight

    [Header("Highlight Settings")]
    [SerializeField] private float highlightRadius = 2f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color soilColor = new Color(2f, 2f, 0f, 0.5f); // Màu vàng cho đất ban đầu
    [SerializeField] private Color plowedColor = new Color(0f, 2f, 0f, 0.5f); // Màu xanh lá cho đất đã cày
    [SerializeField] private Color harvestColor = new Color(2f, 0f, 0f, 0.5f); // Màu đỏ cho cây sẵn sàng thu hoạch
    [SerializeField] private bool enableHighlight = true; // Bật/tắt hiệu ứng highlight

    [Header("Harvesting Settings")]
    [SerializeField] private float harvestItemHeight = 0.5f; // Chiều cao vật phẩm khi thu hoạch
    [SerializeField] private float harvestCooldown = 0.5f; // Thời gian chờ trước khi ô đất có thể tương tác lại
    [SerializeField] private int minItemCount = 1; // Số lượng vật phẩm tối thiểu khi thu hoạch
    [SerializeField] private int maxItemCount = 3; // Số lượng vật phẩm tối đa khi thu hoạch

    // Từ điển để theo dõi trạng thái của mỗi ô đất
    private Dictionary<Vector3Int, int> tileStages = new Dictionary<Vector3Int, int>();
    // Trạng thái: -1 = đang trong thời gian bảo vệ, 0 = đất ban đầu, 1 = đất đã cày, 
    // 2 = đã trồng, 3 = đang lớn, 4 = sẵn sàng thu hoạch

    private Player player;
    private bool debugMode = true;

    void Start()
    {
        // In ra tên của các tile để debug
        Debug.Log("TileManager Start");
        Debug.Log("Hidden Interactable Tile name: " + (hiddenInteractableTile != null ? hiddenInteractableTile.name : "NULL"));
        Debug.Log("Interacted Tile name: " + (interactedTile != null ? interactedTile.name : "NULL"));
        Debug.Log("Highlight Tile name: " + (highlightTile != null ? highlightTile.name : "NULL"));

        // Kiểm tra các component cần thiết
        if (interactableMap == null)
        {
            Debug.LogError("interactableMap không được gán!");
        }
        if (highlightMap == null)
        {
            Debug.LogError("highlightMap không được gán! Tạo một Tilemap mới và gán vào Inspector.");
        }
        if (highlightTile == null)
        {
            Debug.LogError("highlightTile không được gán! Tạo một Tile mới và gán vào Inspector.");
        }

        // Thay thế tất cả các tile ban đầu trên map bằng hiddenInteractableTile
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);

            // Kiểm tra nếu có tile tại vị trí này
            if (tile != null)
            {
                // Thay thế tất cả các tile có sẵn trên map bằng hiddenInteractableTile
                interactableMap.SetTile(position, hiddenInteractableTile);
                tileStages[position] = 0; // Đặt trạng thái ban đầu
            }
        }

        // Tìm player trong scene
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Không tìm thấy Player trong scene!");
        }

        // Test highlight một số tile để kiểm tra
        StartCoroutine(TestHighlightAfterDelay());
    }

    // Test highlight sau một khoảng thời gian ngắn
    private IEnumerator TestHighlightAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        TestHighlight();
    }

    // Test highlight để kiểm tra system
    private void TestHighlight()
    {
        if (highlightMap == null || highlightTile == null) return;

        Debug.Log("Test highlight...");
        // Xóa highlight cũ
        highlightMap.ClearAllTiles();

        // Test highlight ở vị trí (0,0)
        Vector3Int center = Vector3Int.zero;
        highlightMap.SetTile(center, highlightTile);
        highlightMap.SetColor(center, Color.red);
        Debug.Log("Đã đặt highlight test tại " + center);

        // Test highlight ở một vùng 3x3 xung quanh người chơi nếu có
        if (player != null)
        {
            Vector3Int playerPos = interactableMap.WorldToCell(player.transform.position);
            Debug.Log("Player position: " + player.transform.position + ", Cell: " + playerPos);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int pos = playerPos + new Vector3Int(x, y, 0);
                    highlightMap.SetTile(pos, highlightTile);
                    highlightMap.SetColor(pos, Color.yellow);
                }
            }
            Debug.Log("Đã đặt highlight test quanh người chơi");
        }
    }

    void Update()
    {
        // Tìm player nếu chưa có
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null) return;
        }

        // Test key để debug
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestHighlight();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            enableHighlight = !enableHighlight;
            Debug.Log("Highlight " + (enableHighlight ? "bật" : "tắt"));
            if (!enableHighlight)
            {
                highlightMap.ClearAllTiles();
            }
        }

        // Chỉ cập nhật highlight nếu được bật
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }

    // Cập nhật highlight cho các tile gần người chơi
    private void UpdateHighlight()
    {
        if (player == null) return;

        // Xóa tất cả highlight hiện tại
        highlightMap.ClearAllTiles();

        Vector3Int playerCell = interactableMap.WorldToCell(player.transform.position);
        if (debugMode)
        {
            Debug.Log("Player position: " + player.transform.position + ", Cell: " + playerCell);
        }

        int highlightCount = 0;

        // Tìm và highlight các tile xung quanh người chơi
        for (int x = -Mathf.FloorToInt(highlightRadius); x <= Mathf.FloorToInt(highlightRadius); x++)
        {
            for (int y = -Mathf.FloorToInt(highlightRadius); y <= Mathf.FloorToInt(highlightRadius); y++)
            {
                Vector3Int cellPos = playerCell + new Vector3Int(x, y, 0);

                // Chỉ highlight các ô trong phạm vi hình tròn
                if (Vector2.Distance(Vector2Int.zero, new Vector2Int(x, y)) <= highlightRadius)
                {
                    // Kiểm tra nếu tile tại vị trí này có thể tương tác
                    if (IsInteractableTile(cellPos))
                    {
                        // Lấy trạng thái của tile
                        int stage = 0;
                        tileStages.TryGetValue(cellPos, out stage);

                        // Chỉ highlight các tile có thể tương tác (giai đoạn 0, 1, hoặc 4)
                        // Bỏ qua các tile đang trong trạng thái bảo vệ (-1)
                        if ((stage == 0 || stage == 1 || stage == 4) && stage != -1)
                        {
                            // Đặt highlight tile
                            highlightMap.SetTile(cellPos, highlightTile);

                            // Chọn màu highlight tùy theo trạng thái
                            Color highlightColor;
                            switch (stage)
                            {
                                case 0: // Đất ban đầu
                                    highlightColor = soilColor;
                                    break;
                                case 1: // Đất đã cày, có thể trồng
                                    highlightColor = plowedColor;
                                    break;
                                case 4: // Cây sẵn sàng thu hoạch
                                    highlightColor = harvestColor;
                                    break;
                                default:
                                    highlightColor = defaultColor;
                                    break;
                            }

                            // Đặt màu cho highlight tile
                            highlightMap.SetColor(cellPos, highlightColor);
                            highlightCount++;
                        }
                    }
                }
            }
        }

        if (debugMode)
        {
            Debug.Log("Đã highlight " + highlightCount + " tiles");
        }
    }

    public bool IsInteractableTile(Vector3Int position)
    {
        // Kiểm tra xem tile tại vị trí có thể tương tác không
        TileBase tile = interactableMap.GetTile(position);

        // Bất kỳ tile nào cũng có thể tương tác
        return tile != null;
    }

    // Phương thức để tương tác với tile
    public void InteractWithTile(Vector3Int position)
    {
        if (!tileStages.ContainsKey(position))
        {
            tileStages[position] = 0;
        }

        // Lấy trạng thái hiện tại của tile
        int currentStage = tileStages[position];

        // Kiểm tra xem tile có đang trong thời gian bảo vệ không
        if (currentStage == -1)
        {
            Debug.Log("Đất đang được xử lý, vui lòng đợi...");
            return;
        }

        // Chuyển đổi sang trạng thái tiếp theo
        switch (currentStage)
        {
            case 0: // Đất ban đầu -> Đất đã cày
                interactableMap.SetTile(position, interactedTile);
                tileStages[position] = 1;
                Debug.Log("Đất đã được cày xới");
                break;

            case 1: // Đất đã cày -> Đã trồng cây
                // Kiểm tra xem người chơi có hạt giống không
                Player player = GameObject.FindObjectOfType<Player>();
                if (player != null && player.numCarrotSeed > 0)
                {
                    player.numCarrotSeed--;
                    interactableMap.SetTile(position, plantedTile);
                    tileStages[position] = 2;
                    Debug.Log("Đã trồng cây. Hạt giống còn lại: " + player.numCarrotSeed);

                    // Bắt đầu quá trình tăng trưởng
                    StartCoroutine(GrowPlant(position));
                }
                else
                {
                    Debug.Log("Không có đủ hạt giống để trồng");
                }
                break;

            case 4: // Thu hoạch
                // Sinh ra vật phẩm thu hoạch
                SpawnHarvestedItem(position);

                // Đặt lại về đất đã cày
                interactableMap.SetTile(position, interactedTile);

                // Đánh dấu đất đang trong thời gian bảo vệ
                tileStages[position] = -1;

                // Bắt đầu thời gian bảo vệ
                StartCoroutine(ProtectTileAfterHarvest(position));

                Debug.Log("Đã thu hoạch thành công!");
                break;

            default:
                Debug.Log("Cây đang phát triển, hãy đợi thêm...");
                break;
        }

        // Cập nhật highlight ngay lập tức sau khi tương tác
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }

    // Coroutine để mô phỏng quá trình tăng trưởng của cây
    private IEnumerator GrowPlant(Vector3Int position)
    {
        // Đợi một khoảng thời gian cho cây lớn lên
        yield return new WaitForSeconds(10f); // 10 giây

        // Chuyển sang trạng thái cây đang lớn
        interactableMap.SetTile(position, growingTile);
        tileStages[position] = 3;
        Debug.Log("Cây đang phát triển...");

        // Đợi thêm một khoảng thời gian nữa
        yield return new WaitForSeconds(15f); // 15 giây

        // Chuyển sang trạng thái sẵn sàng thu hoạch
        interactableMap.SetTile(position, harvestReadyTile);
        tileStages[position] = 4;
        Debug.Log("Cây đã sẵn sàng để thu hoạch!");

        // Cập nhật highlight ngay khi cây sẵn sàng thu hoạch
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }

    // Phương thức để sinh ra vật phẩm thu hoạch
    private void SpawnHarvestedItem(Vector3Int position)
    {
        // Chuyển đổi từ vị trí tile sang vị trí thế giới
        Vector3 worldPos = interactableMap.GetCellCenterWorld(position);

        // Đặt vật phẩm cao hơn một chút so với mặt đất để tránh va chạm
        worldPos.y += harvestItemHeight;

        // Kiểm tra xem có prefab vật phẩm trong FarmGameManager không
        if (FarmGameManager.instance != null && FarmGameManager.instance.harvestedItemPrefab != null)
        {
            // Tạo từ minItemCount-maxItemCount vật phẩm thay vì chỉ 1 vật phẩm
            int itemCount = Random.Range(minItemCount, maxItemCount + 1);

            for (int i = 0; i < itemCount; i++)
            {
                // Tạo vị trí ngẫu nhiên cho mỗi vật phẩm
                Vector3 itemPos = worldPos;
                itemPos.x += Random.Range(-0.2f, 0.2f);
                itemPos.y += Random.Range(0, 0.2f);

                // Sinh ra vật phẩm tại vị trí ngẫu nhiên
                Instantiate(FarmGameManager.instance.harvestedItemPrefab, itemPos, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("Không có prefab vật phẩm thu hoạch!");

            // Tăng số lượng vật phẩm cho người chơi trực tiếp nếu không có prefab
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null)
            {
                player.numCarrotSeed += 2; // Thu được 2 hạt giống khi thu hoạch
                Debug.Log("Đã thêm 2 hạt giống vào túi đồ. Tổng số: " + player.numCarrotSeed);
            }
        }
    }

    // Thêm coroutine mới để tạo thời gian bảo vệ sau khi thu hoạch
    private IEnumerator ProtectTileAfterHarvest(Vector3Int position)
    {
        // Đợi một khoảng thời gian ngắn
        yield return new WaitForSeconds(harvestCooldown);

        // Khôi phục trạng thái đất đã cày (giai đoạn 1)
        tileStages[position] = 1;

        // Cập nhật highlight
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }

    // Giữ lại phương thức cũ để tương thích ngược
    public void SetTileInteractable(Vector3Int position)
    {
        InteractWithTile(position);
    }

    // Thêm để hiển thị phạm vi highlight trong Scene view
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, highlightRadius);
        }
    }
}