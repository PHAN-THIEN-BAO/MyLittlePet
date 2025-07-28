using UnityEngine;

public class TestLevel : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
       if(Input.GetKeyDown(KeyCode.Space))
        {
            // find PlayerLevel component in the scene
            PlayerLevel playerLevel = GameObject.Find("Player").GetComponent<PlayerLevel>();
            if (playerLevel != null)
            {
                playerLevel.AddExp(60); // add 60 experience points
            }

            else
            {
                Debug.LogError("Không tìm th?y PlayerLevel trong scene!");
            }
        } 
    }
}
