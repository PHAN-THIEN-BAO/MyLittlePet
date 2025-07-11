using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))] // Yêu cầu GameObject có Rigidbody2D
public class Item : MonoBehaviour
{
    public ItemData data; // Dữ liệu của item

    [HideInInspector] public Rigidbody2D rb2d; // Rigidbody2D để xử lý vật lý

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>(); // Lấy Rigidbody2D từ GameObject
    }
}