using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PetController : MonoBehaviour
{
    public GameObject petPrefab; // Kéo prefab vào đây
    public Transform petParent;  // Kéo một GameObject trống làm parent (nếu muốn)
    public GameObject[] petPrefabs; // Kéo các prefab vào đây theo thứ tự petID

    void Start()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        List<PlayerPet> playerPets = APIPlayerPet.GetPetsByPlayerId(user.id);

        if (playerPets != null && playerPets.Count > 0)
        {
            for (int i = 0; i < playerPets.Count; i++)
            {
                PlayerPet pet = playerPets[i];
                SpawnPet(pet);
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy pet nào cho user này.");
        }
    }

    public void SpawnPet(PlayerPet pet)
    {
        // Tạo vị trí ngẫu nhiên hoặc vị trí mặc định
        float randomX = Random.Range(-5f, 5f);
        float randomY = Random.Range(-2f, 2f);
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);

        // Chọn đúng prefab theo petID
        GameObject prefabToSpawn = (petPrefabs != null && pet.petID >= 0 && pet.petID < petPrefabs.Length)
            ? petPrefabs[pet.petID]
            : null;

        if (prefabToSpawn == null)
        {
            Debug.LogWarning($"Không tìm thấy prefab cho petID: {pet.petID}");
            return;
        }

        GameObject petObj = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, petParent);

        var clickHandler = petObj.GetComponent<PetClickHandler>();
        if (clickHandler != null)
        {
            clickHandler.uiManager = FindObjectOfType<PetInfoUIManager>();
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
    }
}
