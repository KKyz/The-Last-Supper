using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject StartMenu;
    public void OpenMenu(GameObject MenuObj)
    {
        MenuObj.SetActive(true);
        StartMenu.SetActive(false);
    }

    public void CloseMenu(GameObject MenuObj)
    {
        MenuObj.SetActive(false);
        StartMenu.SetActive(true);
    }
}
