using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ControllActiveUI : MonoBehaviour
{

    [SerializeField] public List<GameObject> activeUIList; // List to hold active UI elements
    [SerializeField] public List<GameObject> inactiveUIList; // List to hold inactive UI elements


    public void ControllUI()
    {
        SetActiveUI(activeUIList);
        SetInactiveUI(inactiveUIList);
    }

    /// <summary>
    /// Sets the active state of UI elements in the provided list to true.
    /// </summary>
    /// <param name="UIList"></param>
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


    /// <summary>
    /// Sets the active state of UI elements in the provided list to false.
    /// </summary>
    /// <param name="UIList"></param>
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
