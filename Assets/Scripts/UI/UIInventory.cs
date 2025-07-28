using UnityEngine;

public class UIInventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject optionItems;
    public GameObject optionPets;



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


    public void HideAll()
    {
        optionItems.SetActive(false);
        optionPets.SetActive(false);
    }
}
