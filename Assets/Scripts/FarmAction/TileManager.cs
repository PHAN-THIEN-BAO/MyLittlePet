//using System.Collections.Generic;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class TileManager : MonoBehaviour
//{
//    [SerializeField] private Tilemap interactableMap; // Tilemap để quản lý các tile
//    [SerializeField] private Tile hiddenInteractableTile; // Tile ẩn khi không tương tác
//    [SerializeField] private Tile interactedTile; // Tile hiển thị khi tương tác

//    void Start()
//    {
//        // In ra tên của các tile để debug
//        Debug.Log("Hidden Interactable Tile name: " + hiddenInteractableTile.name);
//        Debug.Log("Interacted Tile name: " + interactedTile.name);

//        // Thay thế tất cả các tile ban đầu trên map bằng hiddenInteractableTile
//        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
//        {
//            TileBase tile = interactableMap.GetTile(position);

//            // Kiểm tra nếu có tile tại vị trí này
//            if (tile != null)
//            {
//                // Thay thế tất cả các tile có sẵn trên map bằng hiddenInteractableTile
//                interactableMap.SetTile(position, hiddenInteractableTile);
//            }
//        }
//    }

//    public bool IsInteractableTile(Vector3Int position)
//    {
//        // Kiểm tra xem tile tại vị trí có phải là hiddenInteractableTile không
//        TileBase tile = interactableMap.GetTile(position);

//        if (tile != null)
//        {
//            // Thay vì dựa vào tên, so sánh trực tiếp với object hiddenInteractableTile
//            if (tile == hiddenInteractableTile)
//            {
//                return true; // Nếu là tile tương tác, trả về true
//            }
//        }
//        return false; // Nếu không phải là tile tương tác, trả về false
//    }

//    public void SetTileInteractable(Vector3Int position)
//    {
//        interactableMap.SetTile(position, interactedTile); // Đặt tile tại vị trí thành tile tương tác
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

    // Từ điển để theo dõi trạng thái của mỗi ô đất
    private Dictionary<Vector3Int, int> tileStages = new Dictionary<Vector3Int, int>();
    // Trạng thái: 0 = đất ban đầu, 1 = đất đã cày, 2 = đã trồng, 3 = đang lớn, 4 = sẵn sàng thu hoạch

    void Start()
    {
        // In ra tên của các tile để debug
        Debug.Log("Hidden Interactable Tile name: " + hiddenInteractableTile.name);
        Debug.Log("Interacted Tile name: " + interactedTile.name);

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
                tileStages[position] = 1;
                Debug.Log("Đã thu hoạch thành công!");
                break;

            default:
                Debug.Log("Cây đang phát triển, hãy đợi thêm...");
                break;
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
    }

    // Phương thức để sinh ra vật phẩm thu hoạch
    private void SpawnHarvestedItem(Vector3Int position)
    {
        // Chuyển đổi từ vị trí tile sang vị trí thế giới
        Vector3 worldPos = interactableMap.GetCellCenterWorld(position);

        // Kiểm tra xem có prefab vật phẩm trong FarmGameManager không
        if (FarmGameManager.instance != null && FarmGameManager.instance.harvestedItemPrefab != null)
        {
            Instantiate(FarmGameManager.instance.harvestedItemPrefab, worldPos, Quaternion.identity);
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

    // Giữ lại phương thức cũ để tương thích ngược
    public void SetTileInteractable(Vector3Int position)
    {
        InteractWithTile(position);
    }
}