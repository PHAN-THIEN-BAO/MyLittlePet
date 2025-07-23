using UnityEngine;

public class UIStoreOptionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject optionItems;
    public GameObject optionPets;
    public GameObject optionConsumption;
    public GameObject optionMedicine;
    public GameObject optionOther;


    public void ShowItems()
    {
        HideAll();
        optionItems.SetActive(true);
    }

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

    public void ShowMedicine()
    {
        HideAll();
        optionMedicine.SetActive(true);
    }
    public void ShowOther()
    {
        HideAll();
        optionOther.SetActive(true);
    }


    public void HideAll()
    {
        optionItems.SetActive(false);
        optionPets.SetActive(false);
        optionConsumption.SetActive(false);
        optionMedicine.SetActive(false);
        optionOther.SetActive(false);
    }
}
