//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class OldPlayer : MonoBehaviour
//{
//    //Inventory

//    //public Inventory inventory; // Reference to the player's inventory

//    //private void Awake()
//    //{
//    //    inventory = new Inventory(); // Initialize the inventory
//    //}

//    public int numCarrot = 0; // Example variable to track carrots collected

//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            if (GameManager.instance == null)
//            {
//                Debug.LogError("GameManager.instance là null! Hãy đảm bảo GameManager được khởi tạo.");
//                return;
//            }

//            if (GameManager.instance.tileManager == null)
//            {
//                Debug.LogError("GameManager.instance.tileManager là null! Kiểm tra trong Inspector.");
//                return;
//            }

//            Vector3Int position = new Vector3Int((int)transform.position.x,
//                (int)transform.position.y, 0);

//            if (GameManager.instance.tileManager.IsInteractable(position))
//            {
//                Debug.Log("Tile is interactable");
//                GameManager.instance.tileManager.SetInteracted(position);
//            }
//            else
//            {
//                Debug.Log("Tile không thể tương tác tại vị trí: " + position);
//            }
//        }
//    }

//    //public void DropItem(Item item)
//    //{
//    //    Vector2 spawnLocation = transform.position; // Get the player's current position
//    //    Vector2 spawnOffset = Random.insideUnitCircle * 1.5f; // Random offset for item spawn

//    //    Collectable droppedItem = Instantiate(item, spawnLocation + spawnOffset,
//    //        Quaternion.identity); // Instantiate the item at the player's position with a random offset

//    //    droppedItem.rb2d.AddForce(spawnOffset * .2f, ForceMode2D.Impulse);
//    //}
//}