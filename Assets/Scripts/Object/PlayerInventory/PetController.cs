using UnityEngine;
using UnityEngine.UI;

public class PetController : MonoBehaviour
{
    public PlayerPet playerPet;
    public Sprite[] petSprites; // Kéo các sprite vào đây theo thứ tự petID
    public Image petImage; // Kéo Image component vào đây

    // You can add more logic here to initialize the pet based on playerPet info
    void Start()
    {
        if (playerPet != null)
        {
            // Example: set name, stats, etc.
            gameObject.name = playerPet.petCustomName;
            // Đặt sprite dựa trên petID
            if (petSprites != null && petImage != null && playerPet.petID >= 0 && playerPet.petID < petSprites.Length)
            {
                petImage.sprite = petSprites[playerPet.petID];
            }
        }
    }
}
