//using UnityEngine;
//using System.Collections; // Ensure you have the correct namespace for MonoBehaviour
//using System.Collections.Generic; // Ensure you have the correct namespace for MonoBehaviour
//using UnityEngine.Tilemaps; // 


//public class TileManager : MonoBehaviour
//{
//    [SerializeField] private Tilemap interactableMap; // The tilemap to manage

//    [SerializeField] private Tile hiddenInteractableTile;
//    [SerializeField] private Tile interactedTile; // The tile to set for interactable tiles


//    void Start()
//    {
//        if (interactableMap == null)
//        {
//            Debug.LogError("interactableMap chưa được gán trong Inspector!");
//            return;
//        }

//        if (hiddenInteractableTile == null)
//        {
//            Debug.LogError("hiddenInteractableTile chưa được gán trong Inspector!");
//            return;
//        }

//        if (interactedTile == null)
//        {
//            Debug.LogError("interactedTile chưa được gán trong Inspector!");
//            return;
//        }

//        // In ra tên của tile để biết cần kiểm tra tên gì
//        Debug.Log("Hidden tile name: " + hiddenInteractableTile.name);

//        foreach (var position in interactableMap.cellBounds.allPositionsWithin)
//        {
//            interactableMap.SetTile(position, hiddenInteractableTile);
//        }
//    }

//    public bool IsInteractable(Vector3Int position)
//    {
//        if (interactableMap == null)
//        {
//            Debug.LogError("interactableMap là null!");
//            return false;
//        }

//        TileBase tile = interactableMap.GetTile(position);

//        if (tile != null)
//        {
//            // Kiểm tra bằng cách so sánh với object thay vì tên
//            if (tile == hiddenInteractableTile || tile.name == hiddenInteractableTile.name)
//            {
//                return true;
//            }

//            // In ra tên tile để debug
//            Debug.Log("Tile name: " + tile.name + ", Expected: " + hiddenInteractableTile.name);
//        }
//        return false;
//    }


//    //void Start()
//    //{
//    //    foreach (var positon in interactableMap.cellBounds.allPositionsWithin)
//    //    {
//    //        interactableMap.SetTile(positon, hiddenInteractableTile); // Set all tiles to the hidden interactable tile
//    //    }
//    //}

//    //public bool IsInteractable(Vector3Int position)
//    //{
//    //    TileBase tile = interactableMap.GetTile(position); // Get the tile at the specified position

//    //    if(tile != null)
//    //    {
//    //        if(tile.name == "FarmInteractable")
//    //        {
//    //            return true;
//    //        }    
//    //    }   
//    //    return false; // Return false if the tile is not interactable
//    //}

//    public void SetInteracted(Vector3Int position)
//    {
//        interactableMap.SetTile(position, interactedTile); // Set the tile at the specified position to the interactable tile
//    }

//}
