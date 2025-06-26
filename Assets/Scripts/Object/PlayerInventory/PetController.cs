using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PetController : MonoBehaviour
{
    public GameObject petPrefab; // Kéo prefab vào đây
    public Transform petParent;  // Kéo một GameObject trống làm parent (nếu muốn)
    public Sprite[] petSprites;  // Kéo các sprite vào đây theo thứ tự petID

    void Start()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(user.id);

        if (playerPets != null && playerPets.Count > 0)
        {
            for (int i = 0; i < playerPets.Count; i++)
            {
                PlayerPet pet = playerPets[i];

                // Tạo vị trí ngẫu nhiên trong vùng x: [-5, 5], y: [-2, 2]
                float randomX = Random.Range(-5f, 5f);
                float randomY = Random.Range(-2f, 2f);
                Vector3 spawnPosition = new Vector3(randomX, randomY, 0);

                GameObject petObj = Instantiate(petPrefab, spawnPosition, Quaternion.identity, petParent);

                SpriteRenderer sr = petObj.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && petSprites != null && pet.petID >= 0 && pet.petID < petSprites.Length)
                {
                    sr.sprite = petSprites[pet.petID];
                }

                var dataHolder = petObj.GetComponent<PetDataHolder>();
                if (dataHolder != null)
                {
                    dataHolder.petData = pet;
                }

                var texts = petObj.GetComponentsInChildren<UnityEngine.UI.Text>();
                foreach (var t in texts)
                {
                    switch (t.gameObject.name)
                    {
                        case "PetIDText":
                            t.text = "ID: " + pet.petID;
                            break;
                        case "PetNameText":
                            t.text = pet.petCustomName;
                            break;
                        case "PetLevelText":
                            t.text = "Level: " + pet.level;
                            break;
                        case "PetStatusText":
                            t.text = "Status: " + pet.status;
                            break;
                    }
                }
                // Nếu dùng TMP_Text thì làm tương tự:
                // var tmps = petObj.GetComponentsInChildren<TMPro.TMP_Text>();
                // ...
            }
            Destroy(petPrefab);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy pet nào cho user này.");
        }
    }
}
