using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private GameObject blur;
    private Camera titleCam;
    private Transform startMenu;

    public void Start()
    {
        blur = transform.Find("Blur").gameObject;
        startMenu = transform.Find("Start");
        OpenSubMenu(startMenu);
        blur.SetActive(false);
        
        titleCam = Camera.main;
        Vector3 camGoalPos = titleCam.transform.position;
        Vector3 camStartPos = new Vector3(camGoalPos.x, camGoalPos.y, camGoalPos.z - 30f);
        titleCam.transform.position = camStartPos;
        LeanTween.moveZ(titleCam.gameObject, camGoalPos.z, 0.8f).setEaseOutSine();
    }

    public void OpenSubMenu(Transform subMenu)
    {
        if (subMenu != startMenu)
        {blur.SetActive(true);}
        
        foreach (Transform menuObj in subMenu)
        {
            menuObj.gameObject.SetActive(true);
        }
        
        foreach (Transform menuObj in subMenu)
        {
            menuObj.GetComponent<MenuAnimations>().EnterAnim();
        }
    }

    public void CloseSubMenu(Transform subMenu)
    {
        blur.SetActive(false);

        foreach (Transform menuObj in subMenu)
        {
            menuObj.GetComponent<MenuAnimations>().ExitAnim();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
