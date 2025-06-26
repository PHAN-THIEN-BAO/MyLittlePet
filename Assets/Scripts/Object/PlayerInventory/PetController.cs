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
            foreach (PlayerPet pet in playerPets)
            {
                // Tạo instance mới từ prefab
                GameObject petObj = Instantiate(petPrefab, petParent);

                SpriteRenderer sr = petObj.GetComponentInChildren<SpriteRenderer>();
                if (sr != null && petSprites != null && pet.petID >= 0 && pet.petID < petSprites.Length)
                {
                    sr.sprite = petSprites[pet.petID];
                }

                // Gán thông tin PetID, PetCustomName, Level, Status vào các Text (nếu có)
                // Giả sử prefab có các Text hoặc TMP_Text với tên tương ứng
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
