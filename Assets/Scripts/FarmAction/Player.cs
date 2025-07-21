using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour
{
    public int numCarrotSeed = 1000;
    public float interactionRadius = 1f;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;
            Vector3Int tilePosition = new Vector3Int(
                Mathf.FloorToInt(mouseWorldPos.x),
                Mathf.FloorToInt(mouseWorldPos.y),
                0);
            float distance = Vector2.Distance(transform.position, mouseWorldPos);
            if (distance <= interactionRadius)
            {
                if (FarmGameManager.instance != null && FarmGameManager.instance.tileManager != null)
                {
                    if (FarmGameManager.instance.tileManager.IsInteractableTile(tilePosition))
                    {
                        FarmGameManager.instance.tileManager.InteractWithTile(tilePosition);
                    }
                }
            }
            else
            {
                Debug.Log("Quá xa d? tuong tác: " + distance + " don v?");
            }
        }
    }
    public void AddCarrotSeed(int amount)
    {
        numCarrotSeed += amount;
        Debug.Log("Ðã thêm " + amount + " h?t gi?ng cà r?t. T?ng s?: " + numCarrotSeed);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}