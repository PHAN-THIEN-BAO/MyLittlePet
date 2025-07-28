//////using System.Collections;
//////using System.Collections.Generic;
//////using UnityEngine;

//////public class Player : MonoBehaviour
//////{
//////    public int numCarrotSeed = 0; // Example variable to track carrots collected

//////    private void Update()
//////    {
//////        // Example input handling to collect carrots
//////        if (Input.GetKeyDown(KeyCode.Space)) // Press 'space' to collect a carrot
//////        {
//////            Vector3Int position = new Vector3Int((int)transform.position.x,
//////                (int)transform.position.y, 0);

//////            if (FarmGameManager.instance.tileManager.IsInteractableTile(position))
//////            {
//////                Debug.Log("Tile is interactable");
//////            }
//////        }
//////    }
//////}

////using System.Collections;
////using System.Collections.Generic;
////using UnityEngine;

////public class Player : MonoBehaviour
////{
////    public int numCarrotSeed = 0; // Example variable to track carrots collected
////    public float interactionRadius = 1f; // Phạm vi tương tác với tile

////    //private void Update()
////    //{
////    //    // Sử dụng chuột trái để tương tác
////    //    if (Input.GetMouseButtonDown(0)) // 0 = chuột trái
////    //    {
////    //        // Chuyển đổi vị trí chuột từ màn hình sang tọa độ trong game
////    //        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
////    //        mouseWorldPos.z = 0; // Đảm bảo z = 0 cho 2D

////    //        // Chuyển đổi vị trí chuột thành vị trí tile (làm tròn xuống để lấy góc trái dưới của tile)
////    //        Vector3Int tilePosition = new Vector3Int(
////    //            Mathf.FloorToInt(mouseWorldPos.x),
////    //            Mathf.FloorToInt(mouseWorldPos.y),
////    //            0);

////    //        // Kiểm tra khoảng cách từ người chơi đến vị trí nhấp chuột
////    //        float distance = Vector2.Distance(transform.position, mouseWorldPos);

////    //        // Chỉ tương tác nếu tile đủ gần người chơi
////    //        if (distance <= interactionRadius)
////    //        {
////    //            // Kiểm tra xem FarmGameManager và tileManager có tồn tại không
////    //            if (FarmGameManager.instance != null && FarmGameManager.instance.tileManager != null)
////    //            {
////    //                // Kiểm tra xem tile có thể tương tác được không
////    //                if (FarmGameManager.instance.tileManager.IsInteractableTile(tilePosition))
////    //                {
////    //                    Debug.Log("Tile is interactable at position: " + tilePosition);
////    //                    // Thêm code xử lý tương tác tại đây
////    //                }
////    //                else
////    //                {
////    //                    Debug.Log("This tile is not interactable");
////    //                }
////    //            }
////    //        }
////    //        else
////    //        {
////    //            Debug.Log("Too far to interact: " + distance + " units away");
////    //        }
////    //    }
////    //}

////    private void Update()
////    {
////        // Sử dụng chuột trái để tương tác
////        if (Input.GetKeyDown(KeyCode.Space))
////        {
////            Vector3Int position = new Vector3Int((int)transform.position.x,
////                (int)transform.position.y, 0);

////            if (FarmGameManager.instance.tileManager.IsInteractableTile(position))
////            {
////                Debug.Log("Tile is interactable at position: " + position);
////                // Thêm code xử lý tương tác tại đây
////                FarmGameManager.instance.tileManager.SetTileInteractable(position); // Sửa thành phương thức đúng
////            }
////        }
////    }

////    private void OnDrawGizmosSelected()
////    {
////        Gizmos.color = Color.yellow;
////        Gizmos.DrawWireSphere(transform.position, interactionRadius);
////    }
////}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Player : MonoBehaviour
//{
//    public int numCarrotSeed = 0; // Example variable to track carrots collected
//    public float interactionRadius = 1f; // Phạm vi tương tác với tile

//    private void Update()
//    {
//        // Sử dụng chuột trái để tương tác
//        if (Input.GetMouseButtonDown(0)) // 0 = chuột trái
//        {
//            // Chuyển đổi vị trí chuột từ màn hình sang tọa độ trong game
//            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            mouseWorldPos.z = 0; // Đảm bảo z = 0 cho 2D

//            // Chuyển đổi vị trí chuột thành vị trí tile (làm tròn xuống để lấy góc trái dưới của tile)
//            Vector3Int tilePosition = new Vector3Int(
//                Mathf.FloorToInt(mouseWorldPos.x),
//                Mathf.FloorToInt(mouseWorldPos.y),
//                0);

//            // Kiểm tra khoảng cách từ người chơi đến vị trí nhấp chuột
//            float distance = Vector2.Distance(transform.position, mouseWorldPos);

//            // Chỉ tương tác nếu tile đủ gần người chơi
//            if (distance <= interactionRadius)
//            {
//                // Kiểm tra xem FarmGameManager và tileManager có tồn tại không
//                if (FarmGameManager.instance != null && FarmGameManager.instance.tileManager != null)
//                {
//                    // Kiểm tra xem tile có thể tương tác được không
//                    if (FarmGameManager.instance.tileManager.IsInteractableTile(tilePosition))
//                    {
//                        Debug.Log("Tile is interactable at position: " + tilePosition);
//                        // Thêm code xử lý tương tác tại đây
//                        FarmGameManager.instance.tileManager.SetTileInteractable(tilePosition);
//                    }
//                    else
//                    {
//                        Debug.Log("This tile is not interactable");
//                    }
//                }
//            }
//            else
//            {
//                Debug.Log("Too far to interact: " + distance + " units away");
//            }
//        }
//    }

//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.yellow;
//        Gizmos.DrawWireSphere(transform.position, interactionRadius);
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int numCarrotSeed = 1000; // Bắt đầu với 5 hạt giống
    public float interactionRadius = 1f; // Phạm vi tương tác với tile

    private void Update()
    {
        // Sử dụng chuột trái để tương tác
        if (Input.GetMouseButtonDown(0)) // 0 = chuột trái
        {
            // Chuyển đổi vị trí chuột từ màn hình sang tọa độ trong game
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0; // Đảm bảo z = 0 cho 2D

            // Chuyển đổi vị trí chuột thành vị trí tile
            Vector3Int tilePosition = new Vector3Int(
                Mathf.FloorToInt(mouseWorldPos.x),
                Mathf.FloorToInt(mouseWorldPos.y),
                0);

            // Kiểm tra khoảng cách từ người chơi đến vị trí nhấp chuột
            float distance = Vector2.Distance(transform.position, mouseWorldPos);

            // Chỉ tương tác nếu tile đủ gần người chơi
            if (distance <= interactionRadius)
            {
                // Kiểm tra xem FarmGameManager và tileManager có tồn tại không
                if (FarmGameManager.instance != null && FarmGameManager.instance.tileManager != null)
                {
                    // Kiểm tra xem tile có thể tương tác được không
                    if (FarmGameManager.instance.tileManager.IsInteractableTile(tilePosition))
                    {
                        // Tương tác với tile
                        FarmGameManager.instance.tileManager.InteractWithTile(tilePosition);
                    }
                }
            }
            else
            {
                Debug.Log("Quá xa để tương tác: " + distance + " đơn vị");
            }
        }
    }

    // Thêm phương thức này vào Player.cs
    public void AddCarrotSeed(int amount)
    {
        numCarrotSeed += amount;
        Debug.Log("Đã thêm " + amount + " hạt giống cà rốt. Tổng số: " + numCarrotSeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}