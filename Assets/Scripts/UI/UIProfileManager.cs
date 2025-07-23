using UnityEngine;

public class UIProfileManager : MonoBehaviour
{
    public GameObject profileTemplate;


    public void OnClickProfile()
    {
        profileTemplate.SetActive(true);
    }
    public void OnClickBackProfile()
    {
        profileTemplate.SetActive(false);
    }


}
