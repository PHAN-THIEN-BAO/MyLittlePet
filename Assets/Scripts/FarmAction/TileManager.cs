using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableMap;
    [SerializeField] private Tile hiddenInteractableTile;
    [SerializeField] private Tile interactedTile;
    [SerializeField] private Tile plantedTile;
    [SerializeField] private Tile growingTile;
    [SerializeField] private Tile harvestReadyTile;
    [SerializeField] private Tilemap highlightMap;
    [SerializeField] private Tile highlightTile;
    [Header("Highlight Settings")]
    [SerializeField] private float highlightRadius = 2f;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color soilColor = new Color(2f, 2f, 0f, 0.5f);
    [SerializeField] private Color plowedColor = new Color(0f, 2f, 0f, 0.5f);
    [SerializeField] private Color harvestColor = new Color(2f, 0f, 0f, 0.5f);
    [SerializeField] private bool enableHighlight = true;
    [Header("Harvesting Settings")]
    [SerializeField] private float harvestItemHeight = 0.5f;
    [SerializeField] private float harvestCooldown = 0.5f;
    [SerializeField] private int minItemCount = 1;
    [SerializeField] private int maxItemCount = 3;
    private Dictionary<Vector3Int, int> tileStages = new Dictionary<Vector3Int, int>();
    private Player player;
    private bool debugMode = true;
    void Start()
    {
        Debug.Log("TileManager Start");
        Debug.Log("Hidden Interactable Tile name: " + (hiddenInteractableTile != null ? hiddenInteractableTile.name : "NULL"));
        Debug.Log("Interacted Tile name: " + (interactedTile != null ? interactedTile.name : "NULL"));
        Debug.Log("Highlight Tile name: " + (highlightTile != null ? highlightTile.name : "NULL"));
        if (interactableMap == null)
        {
            Debug.LogError("interactableMap không du?c gán!");
        }
        if (highlightMap == null)
        {
            Debug.LogError("highlightMap không du?c gán! T?o m?t Tilemap m?i và gán vào Inspector.");
        }
        if (highlightTile == null)
        {
            Debug.LogError("highlightTile không du?c gán! T?o m?t Tile m?i và gán vào Inspector.");
        }
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableMap.GetTile(position);
            if (tile != null)
            {
                interactableMap.SetTile(position, hiddenInteractableTile);
                tileStages[position] = 0;
            }
        }
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Không tìm th?y Player trong scene!");
        }
        StartCoroutine(TestHighlightAfterDelay());
    }
    private IEnumerator TestHighlightAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        TestHighlight();
    }
    private void TestHighlight()
    {
        if (highlightMap == null || highlightTile == null) return;
        Debug.Log("Test highlight...");
        highlightMap.ClearAllTiles();
        Vector3Int center = Vector3Int.zero;
        highlightMap.SetTile(center, highlightTile);
        highlightMap.SetColor(center, Color.red);
        Debug.Log("Ðã d?t highlight test t?i " + center);
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
            Debug.Log("Ðã d?t highlight test quanh ngu?i choi");
        }
    }
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null) return;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestHighlight();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            enableHighlight = !enableHighlight;
            Debug.Log("Highlight " + (enableHighlight ? "b?t" : "t?t"));
            if (!enableHighlight)
            {
                highlightMap.ClearAllTiles();
            }
        }
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }
    private void UpdateHighlight()
    {
        if (player == null) return;
        highlightMap.ClearAllTiles();
        Vector3Int playerCell = interactableMap.WorldToCell(player.transform.position);
        if (debugMode)
        {
            Debug.Log("Player position: " + player.transform.position + ", Cell: " + playerCell);
        }
        int highlightCount = 0;
        for (int x = -Mathf.FloorToInt(highlightRadius); x <= Mathf.FloorToInt(highlightRadius); x++)
        {
            for (int y = -Mathf.FloorToInt(highlightRadius); y <= Mathf.FloorToInt(highlightRadius); y++)
            {
                Vector3Int cellPos = playerCell + new Vector3Int(x, y, 0);
                if (Vector2.Distance(Vector2Int.zero, new Vector2Int(x, y)) <= highlightRadius)
                {
                    if (IsInteractableTile(cellPos))
                    {
                        int stage = 0;
                        tileStages.TryGetValue(cellPos, out stage);
                        if ((stage == 0 || stage == 1 || stage == 4) && stage != -1)
                        {
                            highlightMap.SetTile(cellPos, highlightTile);
                            Color highlightColor;
                            switch (stage)
                            {
                                case 0:
                                    highlightColor = soilColor;
                                    break;
                                case 1:
                                    highlightColor = plowedColor;
                                    break;
                                case 4:
                                    highlightColor = harvestColor;
                                    break;
                                default:
                                    highlightColor = defaultColor;
                                    break;
                            }
                            highlightMap.SetColor(cellPos, highlightColor);
                            highlightCount++;
                        }
                    }
                }
            }
        }
        if (debugMode)
        {
            Debug.Log("Ðã highlight " + highlightCount + " tiles");
        }
    }
    public bool IsInteractableTile(Vector3Int position)
    {
        TileBase tile = interactableMap.GetTile(position);
        return tile != null;
    }
    public void InteractWithTile(Vector3Int position)
    {
        if (!tileStages.ContainsKey(position))
        {
            tileStages[position] = 0;
        }
        int currentStage = tileStages[position];
        if (currentStage == -1)
        {
            Debug.Log("Ð?t dang du?c x? lý, vui lòng d?i...");
            return;
        }
        switch (currentStage)
        {
            case 0:
                interactableMap.SetTile(position, interactedTile);
                tileStages[position] = 1;
                Debug.Log("Ð?t dã du?c cày x?i");
                break;
            case 1:
                Player player = GameObject.FindObjectOfType<Player>();
                if (player != null && player.numCarrotSeed > 0)
                {
                    player.numCarrotSeed--;
                    interactableMap.SetTile(position, plantedTile);
                    tileStages[position] = 2;
                    Debug.Log("Ðã tr?ng cây. H?t gi?ng còn l?i: " + player.numCarrotSeed);
                    StartCoroutine(GrowPlant(position));
                }
                else
                {
                    Debug.Log("Không có d? h?t gi?ng d? tr?ng");
                }
                break;
            case 4:
                SpawnHarvestedItem(position);
                interactableMap.SetTile(position, interactedTile);
                tileStages[position] = -1;
                StartCoroutine(ProtectTileAfterHarvest(position));
                Debug.Log("Ðã thu ho?ch thành công!");
                break;
            default:
                Debug.Log("Cây dang phát tri?n, hãy d?i thêm...");
                break;
        }
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }
    private IEnumerator GrowPlant(Vector3Int position)
    {
        yield return new WaitForSeconds(10f);
        interactableMap.SetTile(position, growingTile);
        tileStages[position] = 3;
        Debug.Log("Cây dang phát tri?n...");
        yield return new WaitForSeconds(15f);
        interactableMap.SetTile(position, harvestReadyTile);
        tileStages[position] = 4;
        Debug.Log("Cây dã s?n sàng d? thu ho?ch!");
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }
    private void SpawnHarvestedItem(Vector3Int position)
    {
        Vector3 worldPos = interactableMap.GetCellCenterWorld(position);
        worldPos.y += harvestItemHeight;
        if (FarmGameManager.instance != null && FarmGameManager.instance.harvestedItemPrefab != null)
        {
            int itemCount = Random.Range(minItemCount, maxItemCount + 1);
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 itemPos = worldPos;
                itemPos.x += Random.Range(-0.2f, 0.2f);
                itemPos.y += Random.Range(0, 0.2f);
                Instantiate(FarmGameManager.instance.harvestedItemPrefab, itemPos, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogWarning("Không có prefab v?t ph?m thu ho?ch!");
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null)
            {
                player.numCarrotSeed += 2;
                Debug.Log("Ðã thêm 2 h?t gi?ng vào túi d?. T?ng s?: " + player.numCarrotSeed);
            }
        }
    }
    private IEnumerator ProtectTileAfterHarvest(Vector3Int position)
    {
        yield return new WaitForSeconds(harvestCooldown);
        tileStages[position] = 1;
        if (enableHighlight && highlightMap != null && highlightTile != null)
        {
            UpdateHighlight();
        }
    }
    public void SetTileInteractable(Vector3Int position)
    {
        InteractWithTile(position);
    }
    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.transform.position, highlightRadius);
        }
    }
}