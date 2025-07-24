using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ControllActiveUI : MonoBehaviour
{

    [SerializeField] public List<GameObject> activeUIList;
    [SerializeField] public List<GameObject> inactiveUIList;


    public void ControllUI()
    {
        SetActiveUI(activeUIList);
        SetInactiveUI(inactiveUIList);
    }

    public void SetActiveUI(List<GameObject> UIList)
    {
        if (UIList == null)
            return;

        foreach (GameObject uiElement in UIList)
        {
            if (uiElement != null)
                uiElement.SetActive(true);
        }
    }


    public void SetInactiveUI(List<GameObject> UIList)
    {
        if (UIList == null)
            return;

        foreach (GameObject uiElement in UIList)
        {
            if (uiElement != null)
                uiElement.SetActive(false);
        }
    }
}