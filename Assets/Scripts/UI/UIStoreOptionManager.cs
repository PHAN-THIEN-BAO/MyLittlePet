using UnityEngine;

public class UIStoreOptionManager : MonoBehaviour
{
    //public GameObject optionItems;
    public GameObject optionPets;
    public GameObject optionConsumption;
    //public GameObject optionMedicine;
    //public GameObject optionOther;



    public void ShowPets()
    {
        HideAll();
        optionPets.SetActive(true);
    }

    public void ShowConsumption()
    {
        HideAll();
        optionConsumption.SetActive(true);
    }



    public void HideAll()
    {
        //optionItems.SetActive(false);
        optionPets.SetActive(false);
        optionConsumption.SetActive(false);
        //optionMedicine.SetActive(false);
        //optionOther.SetActive(false);
    }
}