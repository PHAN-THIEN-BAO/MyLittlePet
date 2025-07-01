//using UnityEngine;
//using System.Collections.Generic;
//using System.Collections;

//public class GameManager : MonoBehaviour
//{
//    public static GameManager instance; // Singleton instance

//    public ItemManager itemManager; // Reference to the ItemManager

//    private void Awake()
//    {
//        // Ensure only one instance of GameManager exists
//        if (instance != null && instance != this)
//        {
//            Destroy(this.gameObject);
//        }
//        else
//        {
//            instance = this;
//        }

//        DontDestroyOnLoad(this.gameObject); // Keep GameManager alive across scenes

//        itemManager = GetComponent<ItemManager>();
//    }
//}
