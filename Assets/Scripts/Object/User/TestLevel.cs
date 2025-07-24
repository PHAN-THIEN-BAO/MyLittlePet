using UnityEngine;

public class TestLevel : MonoBehaviour
{

    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerLevel playerLevel = GameObject.Find("Player").GetComponent<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddExp(60);
            }

            else
            {
                Debug.LogError("Không tìm th?y PlayerLevel trong scene!");
            }
        } 
    }
}