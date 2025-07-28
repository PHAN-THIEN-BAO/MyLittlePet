using TMPro;
using UnityEngine;

public class LoadChoosePet : MonoBehaviour
{
    [SerializeField] public TMP_Text petID;
    [SerializeField] public TMP_Text petDefaultName;
    [SerializeField] public TMP_Text description;
    [SerializeField] public TMP_Text petType;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetPetInfo(int.Parse(petID.text)); // Example pet ID, replace with actual ID as needed
        Debug.Log("LoadChoosePet Start called with petID: " + petID.text);
    }

    


    public void SetPetInfo(int petId)
    {
        Pet pet = APIPet.GetPetById(petId);
        if (pet != null)
        {
            petID.text = pet.petId.ToString();
            petDefaultName.text = pet.petDefaultName;
            description.text = pet.description;
            petType.text = pet.petType.ToString();
        }
        else
        {
            Debug.LogError("Failed to retrieve pet information.");
        }



    }
}
