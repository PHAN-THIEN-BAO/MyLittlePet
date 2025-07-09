using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FarmGameManager : MonoBehaviour  // Đổi tên từ GameManager thành FarmGameManager
{
    public static FarmGameManager instance; // Singleton instance

    public TileManager tileManager; // Reference to the TileManager

    //public ItemManager itemManager; // Reference to the ItemManager

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

        //itemManager = GetComponent<ItemManager>();
        tileManager = GetComponent<TileManager>();
    }
}