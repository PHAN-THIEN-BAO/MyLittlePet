using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ChoosePet : MonoBehaviour
{
    [SerializeField] public TMP_Text petID;
    [SerializeField] public TMP_Text petDefaultName;
    [SerializeField] public TMP_Text petCustomName;
    [SerializeField] public TMP_Text petType;
    [SerializeField] public TMP_Text description;

    public void SetPetInfo(int petId)
    {
        StartCoroutine(APIPet.GetPetByIdCoroutine(petId, (result) =>
        {
            if (result != null)
            {
                petID.text = result.petID.ToString();
                petDefaultName.text = result.petDefaultName;
                petCustomName.text = result.petDefaultName; // Default to the default name
                petType.text = result.petType;
                description.text = result.description;
            }
            else
            {
                Debug.LogError("Failed to retrieve pet information.");
            }
        }));

    }

    public void ChooseAPet()
    {
        User user  = PlayerInfomation.LoadPlayerInfo();
        PlayerPet playerPet = new PlayerPet();
        playerPet.playerID = user.id;
        playerPet.petID = int.Parse(petID.text);
        playerPet.petCustomName = petCustomName.text;
        UpdatePlayerPetCoroutine(playerPet, (success) =>
        {
            if ((bool)success)
            {
                Debug.Log("Pet chosen successfully!");
                // Optionally, you can update the UI or notify the user
            }
            else
            {
                Debug.LogError("Failed to choose pet.");
            }
        });

    }
    // This method is a placeholder for the actual implementation of updating the player pet.
    private void UpdatePlayerPetCoroutine(PlayerPet playerPet, Action<object> value)
    {
        throw new NotImplementedException();
    }
}
