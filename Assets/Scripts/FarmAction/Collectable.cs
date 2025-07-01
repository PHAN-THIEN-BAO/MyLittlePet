using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    //player walks into collectable
    //add collectable to player
    //delete collectable from scene

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if(player)
        {
            player.numCarrot += 1; // Increment the carrot count
            Destroy(this.gameObject); // Destroy the collectable object
        }    
    }
}
