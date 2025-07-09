using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{

    [SerializeField] private Tilemap interactableMap; // Tilemap để quản lý các tile

    [SerializeField] private Tile hiddenInteractableTile; // Tile ẩn khi không tương tác

    [SerializeField] private Tile interactedTile; // Tile hiển thị khi tương tác


    void Start()
    {
        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
        {
            interactableMap.SetTile(position, hiddenInteractableTile); // Đặt tất cả các tile thành tile ẩn
        }
    }

    public bool IsInteractableTile(Vector3Int position)
    {
        // Kiểm tra xem tile tại vị trí có phải là tile tương tác không
        TileBase tile = interactableMap.GetTile(position);
        
        if(tile != null)
        {
            if(tile.name == "FarmInteractable")
            {
                return true; // Nếu là tile tương tác, trả về true
            }
        }
        return false; // Nếu không phải là tile tương tác, trả về false
    }

    public void SetTileInteractable(Vector3Int position)
    {
        interactableMap.SetTile(position, interactedTile); // Đặt tile tại vị trí thành tile tương tác
    }

}
