using UnityEngine;
public class UIStoreOptionManager : MonoBehaviour
{
    public GameObject optionPets;
    public GameObject optionConsumption;
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
        optionPets.SetActive(false);
        optionConsumption.SetActive(false);
    }
}