using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Movement : MonoBehaviour
{
    public float speed; // speed of the player

    //get input from the player
    //apply movement to sprite

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector3 direction = new Vector3(horizontal, vertical);

        transform.position += direction * speed * Time.deltaTime;
    }

}
