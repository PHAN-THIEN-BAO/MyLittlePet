using UnityEngine;

public class FakePlayerPet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPet playerPet = new PlayerPet
        {
            playerID = 1,
            petID = 2,
            petCustomName = "FakePet",
            status = "100%25100%25100"
        };
        if (APIPlayerPet.AddPlayerPet(playerPet)){
            Debug.Log("Fake Player Pet added successfully.");
        }
        else
        {
            Debug.LogError("Failed to add Fake Player Pet.");

        }

    }
}
