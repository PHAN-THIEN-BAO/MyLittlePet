using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ChoosePet : MonoBehaviour
{
    [SerializeField] public TMP_Text petID;
    [SerializeField] public TMP_Text petDefaultName;
    [SerializeField] public TMP_InputField petCustomName;
    [SerializeField] public TMP_Text petType;
    [SerializeField] public TMP_Text description;
    [SerializeField] public GameObject successPanel;
    [SerializeField] public GameObject failPanel;

    public void ChooseAPet()
    {
        User user = PlayerInfomation.LoadPlayerInfo();
        PlayerPet playerPet = new PlayerPet();
        playerPet.playerID = user.id;
        Debug.Log("Player ID: " + playerPet.playerID);
        playerPet.petID = int.Parse(petID.text);
        Debug.Log("Pet ID: " + playerPet.petID);
        playerPet.petCustomName = petCustomName.text;
        Debug.Log("Pet Custom Name: " + playerPet.petCustomName);
        if (APIPlayerPet.AddPlayerPet(playerPet))
        {
            Debug.Log("Pet added successfully!");
            successPanel.SetActive(true);

            // Optionally, you can update the player's pet information in the UI or elsewhere
            // UpdatePlayerPetUI(playerPet);
        }
        else
        {
            failPanel.SetActive(true);
            Debug.LogError("Failed to add pet.");
        }

    }
}
